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

    [Header("Movement Attributes")]
    public float m_gravityMult = 9.81f;
    public float m_moveSpeed = 5.0f;
    public float m_rollSpeed = 12.0f;
    float m_turnSmoothTime = 0.075f;
    float m_turnSmoothVelocity;
    public bool m_isRolling { get; private set; } = false;
    public bool m_isRollInvincible { get; private set; } = false;
    private Vector3 m_lastMoveDirection;

    private bool m_grounded = true;
    private float m_yVelocity = 0.0f;
    public bool m_isStunned { get; private set; } = false;
    private float m_stunTimer = 0.0f;
    private Vector3 m_knockbackVelocity = Vector3.zero;
    public float m_minKnockbackSpeed = 0.5f;

    [Header("Dodge Attributes")]
    public float m_shadowDuration = 1.0f;
    public GameObject m_adrenShadowPrefab;
    private float m_rollTimer = 0.0f;
    private float m_rollDuration = 0.2f;

    private float m_rollCDTimer = 0.0f;
    public float m_rollCD = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<Player_Controller>();
        characterController = GetComponent<CharacterController>();

        playerController.animator.SetFloat("RollSpeed", (m_rollSpeed / 8.0f));

        var animControllers = playerController.animator.runtimeAnimatorController;
        foreach (var clip in animControllers.animationClips)
        {
            if (clip.name == "knight dodge roll")
                m_rollDuration = clip.length / playerController.animator.GetFloat("RollSpeed");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Gravity physics
        m_grounded = characterController.isGrounded;
        if (m_grounded)
            m_yVelocity = -1.0f;
        else
            m_yVelocity -= m_gravityMult * Time.fixedDeltaTime;

        RollUpdate();
        StunUpdate();
    }

    /*******************
     * RollUpdate : Updates rolling movement if active.
     * @author : William de Beer
     */
    private void RollUpdate()
    {
        if (m_isRolling) // Check if the player is supposed to be rolling
        {
            // Move player in stored direction while roll is active
            characterController.Move(m_lastMoveDirection.normalized * m_rollSpeed * (playerController.playerStats.m_movementSpeed / 100.0f) * Time.fixedDeltaTime
                + transform.up * m_yVelocity * Time.fixedDeltaTime);
            RotateToFaceDirection(new Vector3(m_lastMoveDirection.x, 0, m_lastMoveDirection.z));

            if (playerController.playerAbilities.m_leftAbility != null)
                playerController.playerAbilities.m_leftAbility.AbilityWhileRolling();
            if (playerController.playerAbilities.m_rightAbility != null)
                playerController.playerAbilities.m_rightAbility.AbilityWhileRolling();

            m_rollTimer -= Time.fixedDeltaTime;
            if (m_rollTimer <= 0.0f)
            {
                m_isRolling = false;

                if (playerController.playerAbilities.m_leftAbility != null)
                    playerController.playerAbilities.m_leftAbility.AbilityOnEndRoll();
                if (playerController.playerAbilities.m_rightAbility != null)
                    playerController.playerAbilities.m_rightAbility.AbilityOnEndRoll();
            }

        }
        else
        {
            if (m_rollCDTimer > 0.0f)
                m_rollCDTimer -= Time.fixedDeltaTime;

            m_isRollInvincible = false;
        }
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

        m_isStunned = true;
        m_stunTimer = _stunDuration;
        m_knockbackVelocity = _knockbackVelocity;
        m_isRolling = false;
        m_isRollInvincible = false;
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
    /*******************
     * Move : Contains logic to move the player, aiming and starting roll.
     * @author : William de Beer
     * @param : (Vector2) Movement direction, (Vector2) Aiming direction, (bool) If roll should start
     * @return : (type) 
     */
    public void Move(Vector2 _move, Vector2 _aim, bool _roll, float _deltaTime)
    {
        _move *= (_aim.magnitude == 0.0f ? 1.0f : 1.0f);

        Vector3 movement = Vector3.zero;
        if (!m_isRolling && !m_isStunned) // If the player is rolling prevent other movement
        {
            if (_aim.magnitude != 0) // If the player is trying to aim...
            {
                // Make player model face aim direction
                Vector3 normalizedAim = Vector3.zero;
                normalizedAim += _aim.y * transform.forward;
                normalizedAim += _aim.x * transform.right;
                RotateToFaceDirection(new Vector3(normalizedAim.x, 0, normalizedAim.z));
            }

            float speed = m_moveSpeed * playerController.playerStats.m_movementSpeed / 100.0f; // Player movement speed
            playerController.animator.SetFloat("MovementSpeed", playerController.playerStats.m_movementSpeed / 100.0f);

            Vector3 normalizedMove = Vector3.zero;

            if (_move.magnitude != 0)
            {
                // Movement
                normalizedMove += _move.y * transform.forward;
                normalizedMove += _move.x * transform.right;

                // Apply movement
                movement = normalizedMove * speed * _deltaTime;

                // If player is not trying to aim, aim in direction of movement.
                if (_aim.magnitude == 0)
                    RotateToFaceDirection(new Vector3(normalizedMove.x, 0, normalizedMove.z));

                // Movement Animation control
                Vector3 rotationVector = new Vector3(0, 0, 0);

                rotationVector += normalizedMove.z * playerModel.transform.right;
                rotationVector += normalizedMove.x * playerModel.transform.forward;

                if (_aim.magnitude == 0)
                {
                    playerController.animator.SetFloat("Horizontal", 0.0f);
                    playerController.animator.SetFloat("Vertical", _move.magnitude);
                }
                else
                {
                    playerController.animator.SetFloat("Horizontal", rotationVector.z);
                    playerController.animator.SetFloat("Vertical", rotationVector.x);
                }
            }
            else
            {
                playerController.animator.SetFloat("Horizontal", 0);
                playerController.animator.SetFloat("Vertical", 0);
            }

            if (_roll && m_rollCDTimer <= 0.0f) // If roll input is triggered
            {
                //playerController.playerAudioAgent.PlayRoll(); // Audio

                if (playerController.playerAbilities.m_leftAbility != null)
                    playerController.playerAbilities.m_leftAbility.AbilityOnBeginRoll();
                if (playerController.playerAbilities.m_rightAbility != null)
                    playerController.playerAbilities.m_rightAbility.AbilityOnBeginRoll();

                playerController.animator.SetFloat("RollSpeed", (m_rollSpeed / 8.0f) * (playerController.playerStats.m_movementSpeed / 100.0f));
                var animControllers = playerController.animator.runtimeAnimatorController;
                foreach (var clip in animControllers.animationClips)
                {
                    if (clip.name == "dodge roll event")
                        m_rollDuration = clip.length / playerController.animator.GetFloat("RollSpeed");
                }

                m_rollCDTimer = m_rollCD;

                playerController.animator.SetTrigger("Roll");

                // Set roll to true
                m_isRolling = true;

                // Set roll duration
                m_rollTimer = m_rollDuration;

                // Create adrenaline provider
                if (m_adrenShadowPrefab != null)
                {
                    AdrenalineProvider provider = Instantiate(m_adrenShadowPrefab, transform.position, Quaternion.identity).GetComponent<AdrenalineProvider>();
                    provider.m_durationInSeconds = m_shadowDuration;
                    provider.m_playerRef = this;
                }

                if (normalizedMove.magnitude != 0.0)
                {
                    m_lastMoveDirection = normalizedMove;
                }
                else // Set last move direction to forward for rolling if player is not moving.
                {
                    m_lastMoveDirection = playerModel.transform.forward;
                }
            }
        }
        // Move
        characterController.Move(movement + transform.up * m_yVelocity * Time.fixedDeltaTime);
    }

    /*******************
     * RotateToFaceDirection : Rotates the player to face specified direction
     * @author : William de Beer
     * @param : (Vector3) Specified direction
     */
    private void RotateToFaceDirection(Vector3 _direction)
    {
        // Rotate player model
        if (_direction.magnitude >= 0.1f && playerModel != null)
        {
            float targetAngle = Mathf.Atan2(_direction.normalized.x, _direction.normalized.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(playerModel.transform.eulerAngles.y, targetAngle, ref m_turnSmoothVelocity, m_turnSmoothTime);
            playerModel.transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
        }
    }

    /*******************
     * GiveAdrenaline : Grants the player adrenaline when they perform a successful dodge.
     * @author : William de Beer
     * @param : (float) Value to add
     */
    public void GiveAdrenaline(float _val)
    {
        playerController.playerAudioAgent.PlayAdrenalineGain(); // Audio

        playerController.playerResources.ChangeAdrenaline(_val);

        GetComponent<Player_AudioAgent>().PlayAdrenalineGain();

        //Slow motion
        GameManager.instance.SlowTime(0.4f, _val);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + playerModel.transform.forward * 2.0f);
    }
}
