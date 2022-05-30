using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Player_Movement: Contains logic for player movement such as running and dodging.
 * @author : William de Beer
 * @file : Player_Movement.cs
 * @year : 2021
 */
public class Player_Movement : MonoBehaviour
{
    public CharacterController characterController { private set; get; }
    public Player_Controller playerController { private set; get; }

    public GameObject playerModel;
    public SpriteRenderer m_aimingArrow;
    public float m_aimingArrowMaxAlpha = 0.5f;
    public float m_aimingArrowLerpSpeed = 5.0f;
    private float m_aimingArrowLerp = 0.0f;

    public float m_aimingArrowFadeDelay = 1.0f;
    public float m_aimingArrowFadeTimer = 0.0f;

    private float m_currentMoveSpeedLerp = 1.0f;

    [Header("Movement Attributes")]
    public float m_gravityMult = 9.81f;
    public float m_moveSpeed = 5.0f;
    public float m_rollSpeed = 12.0f;
    public float m_rollDistanceMult = 8.0f;
    public float m_attackMoveSpeed = 0.4f;
    public float m_healMoveSpeedMult = 0.5f;
    public float m_attackMoveSpeedLerpSpeed = 5.0f;
    public float m_rollCost = 35.0f;
    float m_turnSmoothTime = 0.075f;
    float m_fastTurnSmoothTime = 0.001f;
    float m_turnSmoothVelocity;
    float m_turnAnimationVelocity = 0.0f;
    public float m_turnAnimationTime = 0.075f;
    public float m_turnAnimationMult = 0.001f;
    public bool m_isRolling { get; private set; } = false;
    public bool m_isRollInvincible { get; private set; } = false;
    private Vector3 m_lastMoveDirection;

    private bool m_grounded = true;
    private float m_yVelocity = 0.0f;
    public bool m_isStunned { get; private set; } = false;
    private float m_stunTimer = 0.0f;
    private Vector3 m_knockbackVelocity = Vector3.zero;
    public float m_minKnockbackSpeed = 0.5f;
    public LayerMask m_groundLayerMask;

    [Header("Dodge Attributes")]
    public float m_shadowDuration = 1.0f;
    public GameObject m_adrenShadowPrefab;
    private float m_rollTimer = 0.0f;
    private float m_rollDuration = 0.2f;

    private float m_rollCDTimer = 0.0f;
    public float m_rollCD = 1.0f;

    [Header("Dodge Destructible")]
    public float m_detectDistance = 0.7f;
    public float m_explodeForce = 5.0f;

    [Header("Dash Movement")]
    private Vector3 m_dashVelocity = Vector3.zero;
    private float m_dashTimer = 0.0f;
    private Vector3 m_dashFaceDirection = Vector3.zero;

    [Header("Foot Transforms")]
    [SerializeField] private Transform m_leftFoot;
    [SerializeField] private Transform m_rightFoot;
    private bool m_isMoving = false;

    [Header("Targeting")]
    public Actor m_currentTarget;
    [SerializeField] private float m_maxAngle = 60.0f;
    [SerializeField] private float m_maxDistance = 20.0f;
    private UI_LockonTarget m_lockonTarget;

    [Header("Environmental Hazards")]
    [SerializeField] private float m_iceSlip = 1.0f;
    [SerializeField] private float m_bogSlow = 0.5f;
    [SerializeField] private float m_lavaDamage = 10.0f;
    [SerializeField] private float m_speedBoost = 2.0f;
    [SerializeField] private float m_jumpBounce = 5.0f;

    private bool m_wasOnIce = false;
    public List<GroundSurface.SurfaceType> m_touchedSurfaces = new List<GroundSurface.SurfaceType>();
    private Vector3 m_slideVelocity = Vector3.zero;
    private bool m_steppedThisFrame = false;

    //Respawn Code
    public Vector3 m_lastGroundedPosition { get; private set; }
    public Vector3 m_lastGroundedVelocity { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<Player_Controller>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Skill implementation.
        m_moveSpeed *= playerController.playerSkills.m_moveSpeedIncrease;
        m_rollDistanceMult *= playerController.playerSkills.m_rollDistanceIncrease;

        m_lockonTarget = HUDManager.Instance.GetElement<UI_LockonTarget>("LockonTarget");

        playerController.animator.SetFloat("RollSpeed", (m_rollSpeed / m_rollDistanceMult));

        var animControllers = playerController.animator.runtimeAnimatorController;
        foreach (var clip in animControllers.animationClips)
        {
            if (clip.name == "knight dodge roll")
            {
                m_rollDuration = clip.length / playerController.animator.GetFloat("RollSpeed");
                break;
            }
        }
    }
    private void Update()
    {
        // Player aiming arrow
        if (m_aimingArrow != null)
        {
            if (m_aimingArrowFadeTimer > 0.0f)
            {
                m_aimingArrowFadeTimer -= Time.deltaTime;
                m_aimingArrowLerp += Time.deltaTime * m_aimingArrowLerpSpeed;
            }
            else
            {
                m_aimingArrowLerp -= Time.deltaTime * m_aimingArrowLerpSpeed;
            }
            m_aimingArrowLerp = Mathf.Clamp01(m_aimingArrowLerp);
            m_aimingArrow.color = new Color(m_aimingArrow.color.r, m_aimingArrow.color.g, m_aimingArrow.color.b, m_aimingArrowLerp * m_aimingArrowMaxAlpha);

            m_aimingArrow.transform.position = playerController.GetFloorPosition();
        }

        m_steppedThisFrame = false;

        if (m_touchedSurfaces.Contains(GroundSurface.SurfaceType.LAVA))
        {
            playerController.DamagePlayer(Time.deltaTime * m_lavaDamage, CombatSystem.DamageType.True);
        }

        if (m_currentTarget != null && (m_currentTarget.m_myBrain.IsDead || Vector3.Distance(m_currentTarget.transform.position, transform.position) > m_maxDistance * 1.1f))
        {
            m_currentTarget.m_myBrain.SetOutlineEnabled(false);
            m_currentTarget = null;
        }

        if (m_lockonTarget != null)
            m_lockonTarget.UpdateTarget(m_currentTarget != null ? m_currentTarget.m_selfTargetTransform.gameObject : null);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Gravity physics
        m_grounded = characterController.isGrounded;
        if (m_grounded)
        {
            m_yVelocity = -1.0f;

            if (Physics.Raycast(transform.position + transform.forward * 0.5f, -Vector3.up, characterController.height * 0.5f + 0.1f, m_groundLayerMask) &&
                Physics.Raycast(transform.position - transform.forward * 0.5f, -Vector3.up, characterController.height * 0.5f + 0.1f, m_groundLayerMask) &&
                Physics.Raycast(transform.position + transform.right * 0.5f, -Vector3.up, characterController.height * 0.5f + 0.1f, m_groundLayerMask) &&
                Physics.Raycast(transform.position - transform.right * 0.5f, -Vector3.up, characterController.height * 0.5f + 0.1f, m_groundLayerMask))
            {
                //Debug.Log((transform.position - m_lastGroundedPosition).magnitude / Time.fixedDeltaTime);

                m_lastGroundedPosition = transform.position;
                m_lastGroundedVelocity = characterController.velocity;
            }
        }
        else if (m_knockbackVelocity.y <= 0.0f)
            m_yVelocity -= m_gravityMult * Time.fixedDeltaTime;

        EnemyHeadAvoision();
        RollUpdate();
        DashUpdate();
        StunUpdate();
    }

    /*******************
     * EnemyHeadAvoision : Checks if player is on enemy head and moves them off of it.
     * @author : William de Beer
     */
    private void EnemyHeadAvoision()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, characterController.radius, playerController.playerAttack.m_attackTargets);

        foreach (var item in targets)
        {
            Actor actor = item.GetComponentInParent<Actor>();
            if (actor != null)
            {
                Vector3 direction = transform.position - actor.transform.position;
                direction.y = 0.0f;
                characterController.Move(direction.normalized * 5.0f * Time.fixedDeltaTime);
            }
        }
    }

    /*******************
     * RollUpdate : Updates rolling movement if active.
     * @author : William de Beer
     */
    private void RollUpdate()
    {
        if (m_isRolling) // Check if the player is supposed to be rolling
        {
            if (m_lastMoveDirection.magnitude < 0.1f)
                m_lastMoveDirection = playerModel.transform.forward;

            // Move player in stored direction while roll is active
            characterController.Move(m_lastMoveDirection.normalized * m_rollSpeed * (playerController.playerStats.m_movementSpeed) * Time.fixedDeltaTime
                + transform.up * m_yVelocity * Time.fixedDeltaTime);
            RotateToFaceDirection(new Vector3(m_lastMoveDirection.x, 0, m_lastMoveDirection.z));

            playerController.playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.WHILE_ROLLING);
            playerController.playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.WHILE_ROLLING);

            m_rollTimer -= Time.fixedDeltaTime;
            if (m_rollTimer <= 0.0f)
            {
                m_isRolling = false;

                playerController.playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.END_ROLL);
                playerController.playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.END_ROLL);
            }

            DestructibleDetection();
        }
        else
        {
            if (m_rollCDTimer > 0.0f)
                m_rollCDTimer -= Time.fixedDeltaTime;

            m_isRollInvincible = false;
        }
    }


    /*******************
     * DashUpdate : Updates dash movement if active.
     * @author : William de Beer
     */
    private void DashUpdate()
    {
        if (m_dashTimer > 0.0f)
        {
            playerController.animator.SetFloat("Horizontal", 0.0f);
            playerController.animator.SetFloat("Vertical", 0.0f); 

            if (m_dashFaceDirection == Vector3.zero)
            {
                RotateToFaceDirection(new Vector3(m_dashFaceDirection.x, 0, m_dashFaceDirection.z));
            }

            // Move player in stored direction while dash is active
            characterController.Move(m_dashVelocity * Time.fixedDeltaTime);

            DestructibleDetection();

            m_dashTimer -= Time.fixedDeltaTime;
        }
    }

    public void DestructibleDetection()
    {
        Collider[] destruct = Physics.OverlapSphere(transform.position, m_detectDistance, playerController.playerAttack.m_attackTargets);

        foreach (var item in destruct)
        {
            Destructible dest = item.GetComponentInParent<Destructible>();
            if (dest != null && dest.m_letRollDestroy)
            {
                dest.ExplodeObject(transform.position, m_explodeForce, 20.0f);
            }
        }
    }

    /*******************
     * ApplyDashMovement : Apply dash movement to player.
     * @author : William de Beer
     */
    public void ApplyDashMovement(Vector3 _velocity, float _duration, Vector3 _facingDirection = default(Vector3))
    {
        m_dashVelocity = _velocity;
        m_dashTimer = _duration;
        m_dashFaceDirection = _facingDirection;
    }

    public void CancelDashMovement()
    {
        m_dashTimer = 0.0f;
    }

    public bool IsDashing()
    {
        return m_dashTimer > 0.0f;
    }

    /*******************
     * StunPlayer : Prevents the player from moving for a set duration and knocks them backwards.
     * @author : William de Beer
     * @param : (float) Duration of the stun, (Vector3) Knockback velocity
     */
    public void StunPlayer(float _stunDuration, Vector3 _knockbackVelocity, bool _bypassInvincibility = false)
    {
        if (!_bypassInvincibility && m_isRollInvincible)
            return;

        m_stunTimer = _stunDuration * (1 - playerController.playerSkills.m_stunDecrease);
        m_knockbackVelocity = _knockbackVelocity;
        m_knockbackVelocity.y = 0;
        m_yVelocity = _knockbackVelocity.y;
        m_dashTimer = 0.0f;

        if (_stunDuration != 0.0f)
        {
            m_isStunned = true;
            m_isRolling = false;
            m_isRollInvincible = false;

            playerController.playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.END_ROLL);

            playerController.playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.END_ROLL);
        }
    }
    /*******************
     * StunUpdate : Updates the players state of being stunned.
     * @author : William de Beer
     */
    private void StunUpdate()
    {
        if (m_isStunned) // Check if the player is stunned
        {
            m_stunTimer -= Time.fixedDeltaTime;
            if (m_stunTimer <= 0.0f)
                m_isStunned = false;
        }

        if (m_knockbackVelocity.magnitude > m_minKnockbackSpeed) // If knockback speed is low enough
        {
            // Apply knockback
            characterController.Move(m_knockbackVelocity * Time.deltaTime
                + transform.up * m_yVelocity * Time.deltaTime);
            m_knockbackVelocity = Vector3.Lerp(m_knockbackVelocity, Vector3.zero, 5 * Time.fixedDeltaTime); // Adjust knockback speed
        }
        else
        {
            m_knockbackVelocity = Vector3.zero; // Stop knockback
        }
    }

    public void IFramesActive(bool _active)
    {
        m_isRollInvincible = _active;
        Debug.Log("I Frames: " + _active);
    }
    public void StopRoll()
    {
        m_isRolling = false;
    }

    public void QuickSetAttackMoveSpeedLerp(float _lerp)
    {
        m_currentMoveSpeedLerp = _lerp;
    }

    /*******************
     * Move : Contains logic to move the player, aiming and starting roll.
     * @author : William de Beer
     * @param : (Vector2) Movement direction, (Vector2) Aiming direction, (bool) If roll should start
     * @return : (type) 
     */
    public void Move(Vector2 _move, Vector2 _aim, bool _roll, float _deltaTime)
    {
        if (!InputManager.Instance.isInGamepadMode && playerController.playerAttack.GetCurrentUsedHand() == Hand.NONE)
            _aim = Vector2.zero;

        // Move speed mult
        m_currentMoveSpeedLerp = Mathf.Clamp(m_currentMoveSpeedLerp + (playerController.playerAttack.GetCurrentUsedHand() != Hand.NONE ? -1.0f : 1.0f) * Time.deltaTime * m_attackMoveSpeedLerpSpeed, 0.0f, 1.0f);

        if (playerController.playerAttack.GetCurrentAttackingHand() == Hand.LEFT)
        {
            if (playerController.playerAttack.m_rightWeapon != null && playerController.playerAttack.m_rightWeapon.m_weaponData.isTwoHanded)
            {
                m_attackMoveSpeed = playerController.playerAttack.m_rightWeapon.m_weaponData.m_attackAltMoveSpeed;
            }
            else
            {
                m_attackMoveSpeed = playerController.playerAttack.m_leftWeapon.m_weaponData.m_attackAltMoveSpeed;
            }
        }
        else if (playerController.playerAttack.GetCurrentAttackingHand() == Hand.RIGHT)
        {
            m_attackMoveSpeed = playerController.playerAttack.m_rightWeapon.m_weaponData.m_attackMoveSpeed;
        }

        Vector2 baseMove = _move;
        _move *= (_aim.magnitude == 0.0f ? 1.0f : 1.0f) * Mathf.Lerp(m_attackMoveSpeed, 1.0f, m_currentMoveSpeedLerp)
            * (!playerController.animator.GetBool("IsHealing") ? 1.0f : m_healMoveSpeedMult * playerController.playerSkills.m_healMoveSpeedIncrease);
        m_isMoving = (_move.magnitude > 0.0f);

        Vector3 movement = Vector3.zero;
        if (m_dashTimer <= 0.0f && !m_isRolling && !m_isStunned) // If the player is rolling prevent other movement
        {
            if (m_currentTarget != null)
            {
                // Make player model face target direction
                Vector3 normalizedAim = (m_currentTarget.transform.position - transform.position).normalized;
                RotateToFaceDirection(new Vector3(normalizedAim.x, 0, normalizedAim.z));

                m_aimingArrowFadeTimer = m_aimingArrowFadeDelay;
            }
            else // If the player is trying to aim...
            {
                // Make player model face aim direction
                Vector3 normalizedAim = Vector3.zero;
                normalizedAim += _aim.y * transform.forward;
                normalizedAim += _aim.x * transform.right;
                RotateToFaceDirection(new Vector3(normalizedAim.x, 0, normalizedAim.z));

                if (_aim.magnitude > 0.0f)
                    m_aimingArrowFadeTimer = m_aimingArrowFadeDelay;
            }

            float speed = m_moveSpeed * playerController.playerSkills.m_movementSpeedStatusBonus * playerController.playerStats.m_movementSpeed; // Player movement speed
            if (m_touchedSurfaces.Contains(GroundSurface.SurfaceType.BOG)) // If the player is walking in bog.
            {
                speed *= m_bogSlow;
            }
            if (m_touchedSurfaces.Contains(GroundSurface.SurfaceType.SPEED)) // If the player is walking in speed.
            {
                speed *= m_speedBoost;
            }
            playerController.animator.SetFloat("MovementSpeed", speed / m_moveSpeed);

            Vector3 normalizedMove = Vector3.zero;

            // If player is not trying to aim, aim in direction of movement.
            if (baseMove.magnitude != 0 && _aim.magnitude == 0 && m_currentTarget == null)
            {
                Vector3 normalizedBaseMove = baseMove.y * transform.forward + baseMove.x * transform.right;
                RotateToFaceDirection(new Vector3(normalizedBaseMove.x, 0, normalizedBaseMove.z));
            }

            if (_move.magnitude != 0)
            {
                // Movement
                normalizedMove += _move.y * transform.forward;
                normalizedMove += _move.x * transform.right;

                // Apply movement
                movement = normalizedMove * speed * _deltaTime;

                //if (playerController.playerAttack.GetCurrentUsedHand() != Hand.NONE)
                //    movement *= m_attackMoveSpeed;


                if (_aim.magnitude == 0 && m_currentTarget == null)
                {
                    playerController.animator.SetFloat("Horizontal", 0.0f);
                    playerController.animator.SetFloat("Vertical", _move.magnitude); // Decrease if player is attacking.
                }
                else
                {
                    // Movement Animation control
                    Vector3 rotationVector = new Vector3(0, 0, 0);

                    rotationVector += normalizedMove.z * playerModel.transform.right;
                    rotationVector += normalizedMove.x * playerModel.transform.forward;

                    playerController.animator.SetFloat("Horizontal", rotationVector.z); // Decrease if player is attacking.

                    playerController.animator.SetFloat("Vertical", rotationVector.x); // Decrease if player is attacking.
                }
            }
            else
            {
                playerController.animator.SetFloat("Horizontal", 0);
                playerController.animator.SetFloat("Vertical", 0);
            }

            if (!playerController.playerResources.m_isExhausted && _roll && m_rollCDTimer <= 0.0f && playerController.playerAttack.GetCurrentUsedHand() == Hand.NONE) // If roll input is triggered
            {
                playerController.playerResources.ChangeStamina(-m_rollCost);
                playerController.playerAudioAgent.PlayRoll(); // Audio

                playerController.playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.BEGIN_ROLL);
                playerController.playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.BEGIN_ROLL);

                playerController.animator.SetFloat("RollSpeed", (m_rollSpeed / m_rollDistanceMult) * (playerController.playerStats.m_movementSpeed));
                var animControllers = playerController.animator.runtimeAnimatorController;
                foreach (var clip in animControllers.animationClips)
                {
                    if (clip.name == "knight dodge roll")
                        m_rollDuration = clip.length / playerController.animator.GetFloat("RollSpeed");
                }

                m_rollCDTimer = m_rollCD;

                playerController.animator.SetTrigger("Roll");

                // Set roll to true
                m_isRolling = true;

                // Set roll duration
                m_rollTimer = m_rollDuration;

                // Create adrenaline provider
                //if (m_adrenShadowPrefab != null)
                //{
                //    AdrenalineProvider provider = Instantiate(m_adrenShadowPrefab, transform.position, Quaternion.identity).GetComponent<AdrenalineProvider>();
                //    provider.m_durationInSeconds = m_shadowDuration;
                //    provider.m_playerRef = this;
                //}

                if (normalizedMove.magnitude > 0.1f)
                {
                    m_lastMoveDirection = normalizedMove;
                }
                else // Set last move direction to forward for rolling if player is not moving.
                {
                    m_lastMoveDirection = playerModel.transform.forward;
                }
            }
        }
        else
        {
            playerController.animator.SetFloat("Rotate", 0.5f);
        }

        Vector3 horizLastMove = characterController.velocity;
        horizLastMove.y = 0;

        if (m_touchedSurfaces.Contains(GroundSurface.SurfaceType.JUMP))
        {
            StunPlayer(0.0f, transform.up * m_jumpBounce);
        }
        if (m_touchedSurfaces.Contains(GroundSurface.SurfaceType.ICE)) // If the player is walking on ice.
        {
            if (!m_wasOnIce)
                m_slideVelocity = horizLastMove * _deltaTime;
            if (!m_isRolling)
            {
                m_slideVelocity -= m_slideVelocity * _deltaTime;
                m_slideVelocity += movement * m_iceSlip * _deltaTime;

                if (m_slideVelocity.magnitude > m_moveSpeed)
                {
                    m_slideVelocity = m_slideVelocity.normalized * m_moveSpeed;
                }
                characterController.Move(m_slideVelocity + transform.up * m_yVelocity * Time.fixedDeltaTime);
            }
            else
            {
                m_slideVelocity = horizLastMove * _deltaTime;
            }
            m_wasOnIce = true;
        }
        else
        {
            m_slideVelocity = Vector3.zero;
            characterController.Move(movement + transform.up * m_yVelocity * Time.fixedDeltaTime);

            m_wasOnIce = false;
        }


        //characterController.Move(movement + transform.up * m_yVelocity * Time.fixedDeltaTime);
        // Move
        //characterController.Move((m_onIce ? horizLastMove + movement * m_slip * _deltaTime : movement) + transform.up * m_yVelocity * _deltaTime);

    }

    /*******************
     * RotateToFaceDirection : Rotates the player to face specified direction
     * @author : William de Beer
     * @param : (Vector3) Specified direction
     */
    private void RotateToFaceDirection(Vector3 _direction)
    {
        if (!playerController.animator.GetBool("CanRotate"))
            return;

        float targetRotateAnim = 0.5f;

        // Rotate player model
        if (_direction.magnitude >= 0.1f && playerModel != null)
        {
            bool fastTurn = !InputManager.Instance.isInGamepadMode && playerController.playerAttack.GetCurrentUsedHand() != Hand.NONE;

            float targetAngle = Mathf.Atan2(_direction.normalized.x, _direction.normalized.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(playerModel.transform.eulerAngles.y, targetAngle, ref m_turnSmoothVelocity, fastTurn ? m_fastTurnSmoothTime : m_turnSmoothTime);
            playerModel.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            targetRotateAnim = Mathf.SmoothDampAngle(playerController.animator.GetFloat("Rotate"), 
                0.5f + m_turnSmoothVelocity * m_turnAnimationMult, ref m_turnAnimationVelocity, m_turnAnimationTime);
            //float rotateOffset = m_turnSmoothVelocity * m_turnAnimationMult;
            //if (Mathf.Abs(rotateOffset) <= 0.05f)
            //    rotateOffset = 0.0f;
            //playerController.animator.SetFloat("Rotate", 0.5f + rotateOffset);
        }
        else
        {
            targetRotateAnim = Mathf.SmoothDampAngle(playerController.animator.GetFloat("Rotate"),
                0.5f, ref m_turnAnimationVelocity, m_turnAnimationTime);
            //playerController.animator.SetFloat("Rotate", targetRotateAnim);
        }
        playerController.animator.SetFloat("Rotate", targetRotateAnim);
    }

    public void LockOnTarget()
    {
        if (m_currentTarget != null)
        {
            m_currentTarget.m_myBrain.SetOutlineEnabled(false);

            m_currentTarget = null;
            Debug.Log("Stopped targeting");
            return;
        }

        Actor closestTarget = null;
        if (InputManager.Instance.isInGamepadMode)
        {
            List<Actor> actors = playerController.GetActorsInfrontOfPlayer(m_maxAngle, m_maxDistance);

            float closestDistance = Mathf.Infinity;

            foreach (var actor in actors)
            {
                float distance = Vector3.Distance(actor.m_selfTargetTransform.transform.position, transform.position);
                bool canLockOn = actor.m_myBrain.m_canBeTarget;

                if (canLockOn && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = actor;
                }
            }
        }
        else
        {
            RaycastHit hit;
            Ray ray = playerController.playerCamera.ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
            if (Physics.Raycast(ray, out hit, 1000, playerController.m_mouseAimingRayLayer))
            {
                // Find actors
                Actor[] actors = FindObjectsOfType<Actor>();

                float closestDistance = Mathf.Infinity;
                foreach (var actor in actors)
                {
                    float distance = Vector3.Distance(hit.point, actor.transform.position);
                    if (distance < closestDistance && distance <= m_maxDistance)
                    {
                        closestDistance = distance;
                        closestTarget = actor;
                    }
                }
            }
        }

        if (closestTarget == null)
            Debug.Log("Could not find target");
        else
        {
            Debug.Log("Found target " + closestTarget.name);
            playerController.playerAudioAgent.PlayLockOn();
        }
        m_currentTarget = closestTarget;
        

        if (!m_currentTarget)
            return;

        m_currentTarget.m_myBrain.SetOutlineEnabled(true);
    }

    public void Footstep(bool _left)
    {
        if (m_steppedThisFrame || (playerController.GetPlayerMovementVector(true).magnitude <= 0.3f) || !characterController.isGrounded || (playerController.animator.GetFloat("Horizontal") == 0.0f && playerController.animator.GetFloat("Vertical") == 0.0f))
            return;

        m_steppedThisFrame = true;
        Vector3 footPosition = _left ? m_leftFoot.position : m_rightFoot.position;

        RaycastHit hit;
        Collider[] waterCheck = Physics.OverlapSphere(footPosition, 0.25f, 1 << LayerMask.NameToLayer("Water")); 
        if(waterCheck.Length > 0)
        {
            playerController.playerAudioAgent.PlayBasicFootstep(3);
            return;
        }
        if (Physics.Raycast(footPosition, Vector3.down, out hit, 1f, 1 << LayerMask.NameToLayer("Environment")))
        {
            if(hit.collider.tag == "Envir_stone")
            {
                playerController.playerAudioAgent.PlayBasicFootstep(1);
            }
            else if (hit.collider.tag == "Envir_dirt")
            {
                playerController.playerAudioAgent.PlayBasicFootstep(2);
            }
            else if (hit.collider.tag == "Envir_wood")
            {
                playerController.playerAudioAgent.PlayBasicFootstep(0);
            }
            else
            {
                playerController.playerAudioAgent.PlayBasicFootstep(0);
            }
            return;
        }
        else
        {
            playerController.playerAudioAgent.PlayBasicFootstep(0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + playerModel.transform.forward * 2.0f);
    }
}
