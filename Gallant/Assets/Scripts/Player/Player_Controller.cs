using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/****************
 * Player_Controller: Controls interactions between different player components and handles player input.
 * @author : William de Beer
 * @file : Player_Controller.cs
 * @year : 2021
 */
public class Player_Controller : MonoBehaviour
{
    public bool m_spawnWithAnimation = false;
    private bool m_spawning = false;
    public GameObject testObject;
    public Camera playerCamera { private set; get; }
    public Animator animator;
    public AvatarMask armsMask;
    public LayerMask m_mouseAimingRayLayer;
    public bool m_isDisabledInput = false;
    public bool m_isKneeling = false;
    public bool m_isNearDrop = false;
    public bool m_isDisabledAttacks = false;
    private bool m_hasRecentPickup = false;
    private bool m_hasRecentDialogue = false;
    public float m_standMoveWeightLerpSpeed = 0.5f;
    public float m_armsWeightLerpSpeed = 0.5f;
    public Hand m_lastAttackHand = Hand.NONE;
    public float m_controlReturnDelay = 1.0f;
    public ClassData m_inkmanClass { private set; get; }

    // Player components
    public Player_Movement playerMovement { private set; get; }
    public Player_Attack playerAttack { private set; get; }
    public Player_Abilities playerAbilities { private set; get; }
    public Player_Resources playerResources { private set; get; }
    public Player_Pickup playerPickup { private set; get; }
    public Player_Stats playerStats { private set; get; }
    public Player_AudioAgent playerAudioAgent { private set; get; }
    public Player_CombatAnimator playerCombatAnimator { private set; get; }
    public Player_ClassArmour playerClassArmour { private set; get; }
    public Player_Skills playerSkills { private set; get; }
    public StatusEffectContainer statusEffectContainer { private set; get; }

    [Header("Dual Wielding Stats")]
    public float m_dualWieldSpeed = 1.3f;
    public float m_dualWieldBonus { private set; get; } = 1.0f;

    [Header("Keyboard Movement")]
    private Vector3 m_currentVelocity = Vector3.zero;
    private Vector3 m_movementVelocity = Vector3.zero;
    private Vector2 m_lastAimDirection = Vector2.zero;

    private bool m_isAiming = false;
    private bool m_hasSwappedTarget = false;

    [HideInInspector] public UI_StatsMenu m_statsMenu;

    private bool m_godMode = false;
    [SerializeField] private GameObject m_damageVFXPrefab;
    [SerializeField] private GameObject m_stunVFXPrefab;

    [Header("Camera Target Focus")]
    private bool m_focusInputDisabled = false;
    private Vector3 m_defaultCameraPosition = Vector3.zero;
    private Transform m_cameraFocusTransform;
    private Vector3 m_lastCameraFocusPosition = Vector3.zero;
    private float m_cameraFocusLerp = 0.0f;
    private float m_cameraFocusLerpSpeed = 1.0f;

    // Zoom
    [Header("Camera Zoom")]
    private float m_zoomLerp = 0.0f;
    private bool m_zoomed = false;
    private float m_startZoom = 60.0f;
    public float m_maxZoom = 30.0f;
    public float m_zoomSpeed = 5.0f;

    public LayerMask m_waterLayer;

    [Header("Camera Shake")]
    private float m_shake = 0.0f;
    private float m_shakeIntensityMult = 1.0f;
    [SerializeField] private float m_shakeAmount = 0.7f;
    [SerializeField] private float m_shakeDecreaseSpeed = 1.0f;

    private CameraBounds m_cameraBounds;
    public bool m_cameraFreeze = false;
    private float m_walkRunLerp = 0.0f;
    private float m_walkRunLerpSpeed = 2.0f;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Rubble"));
        if (m_statsMenu == null)
            m_statsMenu = HUDManager.Instance.GetElement<UI_StatsMenu>("StatsMenu");

        playerMovement = GetComponent<Player_Movement>();
        playerAbilities = GetComponent<Player_Abilities>();
        playerAttack = GetComponent<Player_Attack>();
        playerResources = GetComponent<Player_Resources>();
        playerPickup = GetComponentInChildren<Player_Pickup>();
        playerStats = GetComponentInChildren<Player_Stats>();
        playerAudioAgent = GetComponent<Player_AudioAgent>();
        playerCombatAnimator = GetComponent<Player_CombatAnimator>();
        playerClassArmour = GetComponent<Player_ClassArmour>();
        playerSkills = GetComponent<Player_Skills>();
        statusEffectContainer = GetComponent<StatusEffectContainer>();

        m_cameraBounds = FindObjectOfType<CameraBounds>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        m_startZoom = playerCamera.fieldOfView;
        m_defaultCameraPosition = playerCamera.transform.parent.localPosition;

        LoadPlayerInfo();

        if (m_spawnWithAnimation)
        {
            m_spawning = true;
            animator.SetTrigger("Spawn");
        }
    }

    public void FinishSpawn()
    {
        m_spawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Set gamepad being used
        int gamepadID = InputManager.Instance.GetAnyGamePad();

        m_zoomLerp += Time.deltaTime * m_zoomSpeed * (m_zoomed ? 1.0f : -1.0f);
        m_zoomLerp = Mathf.Clamp01(m_zoomLerp);
        playerCamera.fieldOfView = Mathf.Lerp(m_startZoom, m_maxZoom, m_zoomLerp);

        if (playerAttack.m_isBlocking)
            playerResources.ChangeStamina(-10.0f * Time.deltaTime);

        if (UI_PauseMenu.isPaused || playerResources.m_dead || m_isDisabledInput || m_spawning || playerMovement.m_isStunned || m_focusInputDisabled || m_isKneeling)
        {
            animator.SetFloat("Horizontal", 0.0f);
            animator.SetFloat("Vertical", 0.0f);

            playerMovement.ForceGravityUpdate(Time.deltaTime);

            return;
        }

        // Camera zoom;
        if (InputManager.Instance.IsBindDown("Toggle_Zoom", gamepadID))
        {
            m_zoomed = !m_zoomed;
        }

        if (m_cameraBounds != null && !m_cameraFreeze)
        {
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.position = m_cameraBounds.RecalculateCameraLocation(playerCamera.transform.position);
        }


        // Set animation speeds based on stats
        //animator.SetFloat("MovementSpeed", playerStats.m_movementSpeed);
        animator.SetFloat("LeftAttackSpeed", m_dualWieldBonus * playerStats.m_attackSpeed * playerSkills.m_attackSpeedStatusBonus * (playerAttack.m_leftWeaponData == null ? 1.0f : playerAttack.m_leftWeaponData.m_speed * playerAttack.m_leftWeaponData.m_altSpeedMult));
        animator.SetFloat("RightAttackSpeed", m_dualWieldBonus * playerStats.m_attackSpeed * playerSkills.m_attackSpeedStatusBonus * (playerAttack.m_rightWeaponData == null ? 1.0f : playerAttack.m_rightWeaponData.m_speed));

        // Two handed attack speed
        if (playerAttack.m_rightWeaponData != null && playerAttack.m_rightWeaponData.isTwoHanded)
            animator.SetFloat("LeftAttackSpeed", m_dualWieldBonus * playerStats.m_attackSpeed * playerAttack.m_rightWeaponData.m_speed * playerAttack.m_rightWeaponData.m_altSpeedMult * playerSkills.m_attackSpeedStatusBonus);

        bool rightAttackHeld = InputManager.Instance.IsBindPressed("Right_Attack", gamepadID); 
        bool leftAttackHeld = InputManager.Instance.IsBindPressed("Left_Attack", gamepadID);

        animator.SetBool("RightAttackHeld", rightAttackHeld);
        animator.SetBool("LeftAttackHeld", leftAttackHeld);

        //float swordRunWeight = 0.0f;
        //if (playerAttack.m_leftWeaponData != null)
        //    swordRunWeight += playerAttack.m_leftWeapon.GetWeaponName() == "Sword" ? -1.0f : 0.0f;
        //if (playerAttack.m_rightWeaponData != null)
        //    swordRunWeight += playerAttack.m_rightWeapon.GetWeaponName() == "Sword" ? 1.0f : 0.0f;

        if ((!rightAttackHeld && !leftAttackHeld) || playerMovement.m_isRolling)
            playerAttack.ToggleBlock(false);

        

        // Move player
        playerMovement.Move(GetPlayerMovementVector(), GetPlayerAimVector(), InputManager.Instance.IsBindDown("Roll", gamepadID), Time.deltaTime);


        float armWeight = animator.GetLayerWeight(animator.GetLayerIndex("Arm"));
        float standArmWeight = animator.GetLayerWeight(animator.GetLayerIndex("StandArm"));
        float runArmWeight = animator.GetLayerWeight(animator.GetLayerIndex("RunArmL"));
        float idleWeight = animator.GetLayerWeight(animator.GetLayerIndex("IdleArmL"));
        // Set avatar mask to be used
        if (Mathf.Abs(animator.GetFloat("Horizontal")) > 0.05f || Mathf.Abs(animator.GetFloat("Vertical")) > 0.05f)
        {
            armWeight += Time.deltaTime * m_standMoveWeightLerpSpeed;
            standArmWeight -= Time.deltaTime * m_standMoveWeightLerpSpeed;
            animator.SetBool("IsMoving", true);
        }
        else
        {
            armWeight -= Time.deltaTime * m_standMoveWeightLerpSpeed;
            standArmWeight += Time.deltaTime * m_standMoveWeightLerpSpeed;
            animator.SetBool("IsMoving", false);
        }

        // Set avatar mask to be used
        //if (Mathf.Abs(animator.GetFloat("Horizontal")) > 0.7f || Mathf.Abs(animator.GetFloat("Vertical")) > 0.7f)
        if (animator.GetFloat("Vertical") > 0.5f)
        {
            runArmWeight += Time.deltaTime * m_armsWeightLerpSpeed;
            idleWeight -= Time.deltaTime * m_armsWeightLerpSpeed;
        }
        else
        {
            runArmWeight -= Time.deltaTime * m_armsWeightLerpSpeed;
            idleWeight += Time.deltaTime * m_armsWeightLerpSpeed;
        }

        armWeight = Mathf.Clamp(armWeight, 0.0f, 1.0f);
        standArmWeight = Mathf.Clamp(standArmWeight, 0.0f, 1.0f);
        runArmWeight = Mathf.Clamp(runArmWeight, 0.0f, 1.0f);
        idleWeight = Mathf.Clamp(idleWeight, 0.0f, 1.0f);
        //float armWeight = 0.0f;
        //float standArmWeight = 1.0f;

        animator.SetLayerWeight(animator.GetLayerIndex("IdleArmL"), (idleWeight));
        animator.SetLayerWeight(animator.GetLayerIndex("IdleArmR"), (idleWeight));

        animator.SetLayerWeight(animator.GetLayerIndex("RunArmL"), (runArmWeight));
        animator.SetLayerWeight(animator.GetLayerIndex("RunArmR"), (runArmWeight));

        animator.SetLayerWeight(animator.GetLayerIndex("Arm"), armWeight);
        animator.SetLayerWeight(animator.GetLayerIndex("StandArm"), standArmWeight);

        // Walk run lerp
        bool isWalkSpeed = animator.GetFloat("Vertical") <= 0.5f;
        m_walkRunLerp = Mathf.Clamp01(m_walkRunLerp + (isWalkSpeed ? -1.0f : 1.0f) * Time.deltaTime * m_walkRunLerpSpeed);
        if (isWalkSpeed)
        {
            animator.SetFloat("Vertical", animator.GetFloat("Vertical") * (1.0f + (1.0f - m_walkRunLerp)));
        }
        animator.SetFloat("WalkRunBlend", m_walkRunLerp);

        if (!playerMovement.m_isStunned && !playerMovement.m_isRolling) // Make sure player is not stunned
        {
            if (playerAttack.GetCurrentUsedHand() == Hand.NONE)
            {
                // Left hand pickup
                if (InputManager.Instance.IsBindPressed("Left_Pickup", gamepadID))
                {
                    DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
                    if (droppedWeapon != null)
                    {
                        if (droppedWeapon.m_pickupDisplay.UpdatePickupTimer(playerAttack.m_leftWeaponData, Hand.LEFT))
                        {
                            HandlePickupDrop(droppedWeapon, Hand.LEFT);
                        }
                    }
                }

                // Right hand pickup
                if (InputManager.Instance.IsBindPressed("Right_Pickup", gamepadID))
                {
                    DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
                    if (droppedWeapon != null)
                    {
                        if (droppedWeapon.m_pickupDisplay.UpdatePickupTimer(playerAttack.m_rightWeaponData, Hand.RIGHT))
                        {
                            HandlePickupDrop(droppedWeapon, Hand.RIGHT);
                        }
                    }
                }
            }

            bool rightWeaponAttack = InputManager.Instance.IsBindPressed("Right_Attack", gamepadID);
            bool leftWeaponAttack = InputManager.Instance.IsBindPressed("Left_Attack", gamepadID);

            if (playerAttack.IsDuelWielding() && rightWeaponAttack && leftWeaponAttack) // Dual attacking
            {
                animator.SetBool("DualAttacking", true);
                m_dualWieldBonus = m_dualWieldSpeed;
            }
            else
            {
                animator.SetBool("DualAttacking", false);
                m_dualWieldBonus = 1.0f;
            }

            if (!m_hasRecentPickup && !m_isDisabledAttacks && !m_isNearDrop)
            {
                if (InputManager.Instance.IsBindDown("Right_Attack", gamepadID) && animator.GetBool("UsingRight"))
                    animator.SetTrigger("RightTrigger");
                if (InputManager.Instance.IsBindDown("Left_Attack", gamepadID) && animator.GetBool("UsingLeft"))
                    animator.SetTrigger("LeftTrigger");

                // Weapon attacks
                if (playerAttack.GetCurrentAttackingHand() == Hand.NONE)
                {
                    if (rightWeaponAttack && leftWeaponAttack) // Dual attacking
                    {
                        if (m_lastAttackHand == Hand.RIGHT && playerAttack.m_leftWeapon != null && !playerAttack.m_leftWeapon.m_isInUse)
                        {
                            playerAttack.StartUsing(Hand.LEFT);
                        }
                        else
                        {
                            playerAttack.StartUsing(Hand.RIGHT);
                        }
                    }
                    else if (rightWeaponAttack)
                    {
                        playerAttack.StartUsing(Hand.RIGHT);
                    }
                    else if (leftWeaponAttack)
                    {
                        playerAttack.StartUsing(Hand.LEFT);
                    }
                }


                // Ability attacks
                if (InputManager.Instance.IsBindDown("Right_Ability", gamepadID))
                {
                    playerAbilities.StartUsing(Hand.RIGHT);
                }
                if (InputManager.Instance.IsBindDown("Left_Ability", gamepadID) && !playerAttack.IsTwoHanded())
                {
                    playerAbilities.StartUsing(Hand.LEFT);
                }
            }
        }

        if (InputManager.Instance.IsBindDown("Toggle_Lockon", gamepadID))
        {
            playerMovement.LockOnTarget();
        }

        EvaluateLockOn();

        if (InputManager.Instance.IsBindDown("Consume", gamepadID) && !animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Arm")).IsName("Heal"))
        {
            // Heal from adrenaline
            animator.SetTrigger("Heal");
            animator.SetBool("IsHealing", true);
        }

        if (InputManager.Instance.IsBindDown("Switch", gamepadID) && !playerAttack.m_isBlocking 
            && !(playerAttack.m_rightWeaponData != null && playerAttack.m_rightWeaponData.isTwoHanded)
            && playerAttack.GetCurrentUsedHand() == Hand.NONE
            && !m_hasRecentDialogue
            && !playerMovement.m_isRolling)
        {
            playerAttack.SwapWeapons();
        }

#if UNITY_EDITOR
        // Debug controls
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_ONE))
        {
            DamagePlayer(20.0f, CombatSystem.DamageType.True, testObject, false);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_TWO))
        {
            playerResources.ChangeAdrenaline(-1);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_THREE))
        {
            playerResources.ChangeAdrenaline(1);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_FOUR))
        {
            StunPlayer(2.0f, transform.up * 20.0f);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_FIVE))
        {
            CurrencyDrop.CreateCurrencyDropGroup(5, new Vector3(0, transform.position.y + 0.5f, 0));
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_ZERO))
        {
            if (testObject != null)
            {
                ChangeCameraFocus(testObject.transform, 2.0f, 3.0f, true);
            }
        }

        // Item debug
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_SIX))
        {
            //playerStats.AddEffect(ItemEffect.MAX_HEALTH_INCREASE);
            //StorePlayerInfo();

            if (playerAttack.m_rightWeaponData)
                playerAttack.m_rightWeaponData = WeaponData.UpgradeWeaponLevel(playerAttack.m_rightWeaponData);
            playerAttack.ApplyWeaponData(Hand.RIGHT);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_SEVEN))
        {
            playerStats.AddEffect(ItemEffect.ABILITY_CD);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_EIGHT))
        {
            playerStats.AddEffect(ItemEffect.ATTACK_SPEED);
        }
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_NINE))
        {
            playerStats.AddEffect(ItemEffect.MOVE_SPEED);
        }
#endif
    }

    public void InstantRunStop()
    {
        m_currentVelocity = Vector3.zero;
        m_walkRunLerp = 0.0f;

        animator.SetFloat("Horizontal", 0.0f);
        animator.SetFloat("Vertical", 0.0f);

        animator.SetLayerWeight(animator.GetLayerIndex("Arm"), 0.0f);
        animator.SetLayerWeight(animator.GetLayerIndex("StandArm"), 1.0f);
    }
    public void ForceZoom(bool _active)
    {
        m_zoomed = _active;
    }
    private void HandlePickupDrop(DroppedWeapon _drop, Hand _hand)
    {
        switch (_drop.m_dropType)
        {
            case DropSpawner.DropType.WEAPON:
                playerAttack.PickUpWeapon(_drop, _hand);
                break;
            case DropSpawner.DropType.UPGRADE:
                if (_hand == Hand.LEFT)
                {
                    if (playerAttack.m_leftWeaponData != null)
                    {
                        playerAttack.m_leftWeaponData = WeaponData.UpgradeWeaponLevel(playerAttack.m_leftWeaponData);
                        Destroy(_drop.gameObject);
                    }
                }
                else
                {
                    if (playerAttack.m_rightWeaponData != null)
                    {
                        playerAttack.m_rightWeaponData = WeaponData.UpgradeWeaponLevel(playerAttack.m_rightWeaponData);
                        Destroy(_drop.gameObject);
                    }
                }
                playerAttack.ApplyWeaponData(_hand);
                break;
            case DropSpawner.DropType.SPELLBOOK:
                if (_hand == Hand.LEFT)
                {
                    if (playerAttack.m_leftWeaponData != null)
                    {
                        WeaponData.AttemptAbilityUpgrade(playerAttack.m_leftWeaponData, _drop.m_abilityData);
                        Destroy(_drop.gameObject);
                    }
                }
                else
                {
                    if (playerAttack.m_rightWeaponData != null)
                    {
                        WeaponData.AttemptAbilityUpgrade(playerAttack.m_rightWeaponData, _drop.m_abilityData);
                        Destroy(_drop.gameObject);
                    }
                }
                playerAttack.ApplyWeaponData(_hand);
                break;
        }
        playerPickup.RemoveDropFromList(_drop);
        StartCoroutine(DelayAttackControl());
    }
    public IEnumerator DelayAttackControl()
    {
        m_hasRecentPickup = true;
        yield return new WaitForSeconds(m_controlReturnDelay);
        m_hasRecentPickup = false;
    }
    public IEnumerator DelaySwapControl()
    {
        m_hasRecentDialogue = true;
        yield return new WaitForSeconds(m_controlReturnDelay);
        m_hasRecentDialogue = false;
    }
    public void StartHeal() { animator.SetBool("IsHealing", true); }

    /*******************
     * StunPlayer : Calls playerMovement StunPlayer function.
     * @author : William de Beer
     * @param : (float) Stun duration, (Vector3) Knockback velocity
     */
    public void StunPlayer(float _stunDuration, Vector3 _knockbackVelocity, GameObject _attacker = null)
    {
        if (_attacker != null && IsInfrontOfPlayer(playerAttack.m_blockingAngle, _attacker.transform.position))
            return;
        playerMovement.StunPlayer(_stunDuration, _knockbackVelocity);
        

        if (m_stunVFXPrefab != null)
        {
            VFXTimerScript m_stunVFXTimer = Instantiate(m_stunVFXPrefab, transform).GetComponent<VFXTimerScript>();
            m_stunVFXTimer.m_timer = _stunDuration;
            m_stunVFXTimer.transform.position += transform.up * 2.0f;
            m_stunVFXTimer.transform.localScale *= 0.5f;
        }
    }
    public Vector2 GetPlayerMovementVector(bool _rawInput = false)
    {
        if (InputManager.Instance.isInGamepadMode) // If using gamepad
        {
            int gamepadID = InputManager.Instance.GetAnyGamePad();
            return InputManager.Instance.GetBindStick("Move", gamepadID);
        }
        else // If using keyboard
        {
            Vector2 movement = Vector2.zero;
            movement.x += (InputManager.Instance.IsBindPressed("Move_Right") ? 1.0f : 0.0f);
            movement.x -= (InputManager.Instance.IsBindPressed("Move_Left") ? 1.0f : 0.0f);
            movement.y += (InputManager.Instance.IsBindPressed("Move_Forward") ? 1.0f : 0.0f);
            movement.y -= (InputManager.Instance.IsBindPressed("Move_Backward") ? 1.0f : 0.0f);
            movement.Normalize();

            if (_rawInput)
                return movement;

            m_currentVelocity = Vector3.SmoothDamp(m_currentVelocity, movement, ref m_movementVelocity, 0.1f);
            return m_currentVelocity;
        }

    }

    private Vector2 GetPlayerAimVector()
    {
        if (InputManager.Instance.isInGamepadMode) // If using gamepad
        {
            int gamepadID = InputManager.Instance.GetAnyGamePad();
            return InputManager.Instance.GetBindStick("Aim", gamepadID);
        }
        else // If using mouse
        {
            //if(InputManager.Instance.IsBindDown("Toggle_Aim"))
            //{
            //    m_isAiming = !m_isAiming;
            //}

            // Raycast to find raycast point
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
            if (Physics.Raycast(ray, out hit, 1000, m_mouseAimingRayLayer))
            {
                // Return direction from player to hit point
                Vector3 aim = hit.point - transform.position;

                Vector3 normalizedAim = Vector3.zero;
                normalizedAim += aim.z * -transform.right;
                normalizedAim += aim.x * transform.forward;
                normalizedAim *= -1;
                m_lastAimDirection = new Vector2(normalizedAim.x, normalizedAim.z);
            }

            //m_lastAimDirection = new Vector2(0.0f, 0.0f);
            return m_lastAimDirection;
        }
    }
    public void EvaluateLockOn()
    {
        // Lock-on change
        if (playerMovement.m_currentTarget != null)
        {
            Vector2 aim = InputManager.Instance.isInGamepadMode ? GetPlayerAimVector() : InputManager.Instance.GetMouseDelta() * Time.deltaTime * 10.0f;

            if (InputManager.Instance.isInGamepadMode)
            {
                if (aim.magnitude >= 1.0f && !m_hasSwappedTarget)
                {
                    List<Actor> actors = GetActorsInfrontOfTransform(playerMovement.m_currentTarget.transform.position,
                        aim.y * transform.forward + aim.x * transform.right, 60.0f, 7.0f);

                    Actor closestActor = null;
                    float closestDistance = Mathf.Infinity;
                    foreach (var actor in actors)
                    {
                        if (actor == playerMovement.m_currentTarget)
                            continue;

                        float distance = Vector3.Distance(playerMovement.m_currentTarget.transform.position, actor.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestActor = actor;
                        }
                    }

                    if (closestActor != null)
                    {
                        playerMovement.m_currentTarget.m_myBrain.SetOutlineEnabled(false);
                        playerMovement.m_currentTarget = closestActor;
                        playerMovement.m_currentTarget.m_myBrain.SetOutlineEnabled(true);
                        m_hasSwappedTarget = true;

                        playerAudioAgent.PlayLockOn();
                    }
                }
                else if (aim.magnitude < 1.0f)
                {
                    m_hasSwappedTarget = false;
                }
            }
            else
            {
                if (aim.magnitude > 0.0f)
                {
                    RaycastHit hit;
                    Ray ray = playerCamera.ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
                    if (Physics.Raycast(ray, out hit, 1000, m_mouseAimingRayLayer))
                    {
                        // Find actors
                        Actor[] actors = FindObjectsOfType<Actor>();

                        Actor closestActor = null;
                        float closestDistance = Mathf.Infinity;
                        foreach (var actor in actors)
                        {
                            float distance = Vector3.Distance(hit.point, actor.transform.position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestActor = actor;
                            }
                        }

                        if (closestActor != null && closestActor != playerMovement.m_currentTarget)
                        {
                            playerMovement.m_currentTarget.m_myBrain.SetOutlineEnabled(false);
                            playerMovement.m_currentTarget = closestActor;
                            playerMovement.m_currentTarget.m_myBrain.SetOutlineEnabled(true);
                            m_hasSwappedTarget = true;

                            playerAudioAgent.PlayLockOn();
                        }
                    }
                }
            }
        }
        else
        {
            m_hasSwappedTarget = false;
        }
    }
    public List<Actor> GetActorsInfrontOfTransform(Vector3 _pos, Vector3 _forward, float _angle, float _distance)
    {
        Collider[] colliders = Physics.OverlapSphere(_pos, _distance, playerAttack.m_attackTargets);
        List<Actor> targets = new List<Actor>();

        foreach (var collider in colliders)
        {
            Actor actor = collider.GetComponentInParent<Actor>();
            if (actor == null)
                continue;

            Vector3 direction = (actor.transform.position - _pos).normalized;
            float dot = Vector3.Dot(direction, _forward);
            if (dot >= Mathf.Cos(_angle * Mathf.Deg2Rad))
            {
                targets.Add(actor);
            }
        }
        return targets;
    }
    public List<Actor> GetActorsInfrontOfPlayer(float _angle, float _distance)
    {
        return GetActorsInfrontOfTransform(transform.position, playerMovement.playerModel.transform.forward, _angle, _distance);

        //Collider[] colliders = Physics.OverlapSphere(transform.position, _distance, playerAttack.m_attackTargets);
        //List<Actor> targets = new List<Actor>();

        //foreach (var collider in colliders)
        //{
        //    Actor actor = collider.GetComponentInParent<Actor>();
        //    if (actor == null)
        //        continue;

        //    Vector3 direction = (actor.transform.position - transform.position).normalized;
        //    float dot = Vector3.Dot(direction, playerMovement.playerModel.transform.forward);
        //    if (dot >= Mathf.Cos(_angle * Mathf.Deg2Rad))
        //    {
        //        targets.Add(actor);
        //    }
        //}
        //return targets;
    }
    public List<Collider> GetCollidersInfrontOfPlayer(float _angle, float _distance, bool _useLayerMask = false)
    {
        Collider[] colliders;
        if (_useLayerMask)
            colliders = Physics.OverlapSphere(transform.position, _distance, playerAttack.m_attackTargets);
        else
            colliders = Physics.OverlapSphere(transform.position, _distance);

        List<Collider> targets = new List<Collider>();

        foreach (var collider in colliders)
        {
            Vector3 direction = (collider.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(direction, playerMovement.playerModel.transform.forward);
            if (dot >= Mathf.Cos(_angle * Mathf.Deg2Rad))
            {
                targets.Add(collider);
            }
        }
        return targets;
    }

    public bool IsInfrontOfPlayer(float _angle, Vector3 _position)
    {
        Vector3 direction = (_position - transform.position);
        direction.y = 0.0f;
        direction.Normalize();
        float dot = Vector3.Dot(direction, playerMovement.playerModel.transform.forward);

        return dot >= Mathf.Cos(_angle * Mathf.Deg2Rad);
    }
    public void DamagePlayer(float _damage, CombatSystem.DamageType _damageType, GameObject _attacker = null, bool _bypassInvincibility = false)
    {
        if (!_bypassInvincibility && playerMovement.m_isRollInvincible)
            return;

        float resistance = 0;
        if (_damageType == CombatSystem.DamageType.Ability)
        {
            resistance += playerSkills.m_magicalDefenceIncrease + playerStats.m_abilityDefence;
        }
        else if (_damageType == CombatSystem.DamageType.Physical)
        {
            resistance += playerSkills.m_physicalDefenceIncrease + playerStats.m_physicalDefence;
        }
        float actualDamage = _damage * (1.0f - CombatSystem.CalculateDamageNegated(_damageType, 0.5f, 0f));

        Weapon_Sword sword = GetComponent<Weapon_Sword>();
        if ((sword != null && sword.m_attackReady) || (playerAttack.m_isBlocking && _attacker != null))
        {
            if (IsInfrontOfPlayer(playerAttack.m_blockingAngle, _attacker.transform.position))
            {
                if (sword != null && sword.m_attackReady)
                {
                    // PLAY PARRY SOUND
                    Debug.Log("PARRY");
                    animator.SetTrigger("Parry");
                    playerAudioAgent.PlayOrbPickup();

                    StatusEffectContainer statusContainer = _attacker.GetComponentInParent<StatusEffectContainer>();
                    if (statusContainer != null)
                        statusContainer.AddStatusEffect(new StunStatus(1.0f));

                    return;
                }

                // PLAY BLOCK SOUND
                Debug.Log("BLOCK");
                if (playerAbilities.m_leftAbility)
                    playerAbilities.m_leftAbility.ReduceCooldown((_damage / 4.0f));
                if (playerAbilities.m_rightAbility)
                    playerAbilities.m_rightAbility.ReduceCooldown((_damage / 4.0f));

                if (playerResources.m_isExhausted)
                {
                    playerAudioAgent.PlayShieldBlock(); // Guard break audio
                    animator.SetTrigger("HitPlayer");
                    return;
                }
                else if (sword == null)
                {
                    animator.SetTrigger("BlockHit");
                }
                playerAudioAgent.PlayShieldBlock();

                return;
            }
            Debug.Log("MISSED BLOCK");
        }

        playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.HIT_RECIEVED, (_attacker != null) ? _attacker.gameObject : null, _damage);
        playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.HIT_RECIEVED, (_attacker != null) ? _attacker.gameObject : null, _damage);

        //Debug.Log($"Player is damaged: {_damage} points of health.");
        if (!m_godMode)
        {
            playerResources.ChangeHealth(-playerResources.ChangeBarrier(-_damage * (1.0f - playerStats.m_damageResistance)));
        }
        // Create VFX
        if (m_damageVFXPrefab != null && !animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Flinch")).IsName("Flinch"))
        {
            Instantiate(m_damageVFXPrefab, transform.position + transform.up, Quaternion.identity);
            playerAudioAgent.PlayHurt();
        }
        animator.SetTrigger("HitPlayer");

        //if (animatorCamera)
        //    animatorCamera.SetTrigger("Shake");
        ScreenShake(5.0f);
    }

    #region Camera Manipulation
    private void FixedUpdate()
    {
        if (!m_cameraFreeze)
        {
            if (m_shake > 0.0f)
            {
                playerCamera.transform.localPosition += Random.insideUnitSphere * m_shakeAmount * m_shakeIntensityMult * Time.fixedDeltaTime;
                m_shake -= Time.fixedDeltaTime * m_shakeDecreaseSpeed;

            }
            else
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.zero, 0.3f);
                m_shake = 0.0f;
            }
        }
    }
    private void LateUpdate()
    {
        // Camera focus lerp
        m_cameraFocusLerp += m_cameraFocusLerpSpeed * (m_cameraFocusTransform != null ? 1.0f : -1.0f) * Time.deltaTime;
        m_cameraFocusLerp = Mathf.Clamp01(m_cameraFocusLerp);

        playerCamera.transform.parent.localPosition = Vector3.Lerp(m_defaultCameraPosition,
           transform.InverseTransformPoint((m_cameraFocusTransform != null ? m_cameraFocusTransform.position : m_lastCameraFocusPosition)) + m_defaultCameraPosition, m_cameraFocusLerp);
    }
    public void ChangeCameraFocus(Transform _focusTarget, float _transitionSpeed, bool _disableInput = false)
    {
        m_cameraFocusTransform = _focusTarget;
        m_cameraFocusLerpSpeed = _transitionSpeed;

        m_focusInputDisabled = _disableInput;
    }
    public void ResetCameraFocus(float _transitionSpeed)
    {
        if (m_cameraFocusTransform)
            m_lastCameraFocusPosition = m_cameraFocusTransform.transform.position;

        m_cameraFocusTransform = null;
        m_cameraFocusLerpSpeed = _transitionSpeed;

        m_focusInputDisabled = false;
    }
    public void ChangeCameraFocus(Transform _focusTarget, float _transitionSpeed, float _duration, bool _disableInput = false)
    {
        ChangeCameraFocus(_focusTarget, _transitionSpeed, _disableInput);

        StartCoroutine(CameraFocusCoroutine(_transitionSpeed, _duration));
    }
    IEnumerator CameraFocusCoroutine(float _transitionSpeed, float _duration)
    {
        yield return new WaitForSeconds(_duration);
        ResetCameraFocus(_transitionSpeed);
    }

    public void ScreenShake(float _intensity, float _shake = 0.3f)
    {
        m_shakeIntensityMult = _intensity;
        m_shake = _shake;
    }
    #endregion

    public void StorePlayerInfo()
    {
        GameManager.StorePlayerInfo(playerAttack.m_leftWeaponData, playerAttack.m_rightWeaponData, playerStats.m_effects, 
            m_inkmanClass, playerAttack.m_leftWeaponEffect, playerAttack.m_rightWeaponEffect, playerResources.m_health, playerResources.m_adrenaline);
    }
    public void LoadPlayerInfo()
    {
        if (GameManager.m_containsPlayerInfo)
        {
            playerAttack.m_leftWeaponData = GameManager.RetrieveWeaponData(Hand.LEFT);
            playerAttack.m_rightWeaponData = GameManager.RetrieveWeaponData(Hand.RIGHT);

            //if (playerAttack.m_leftWeaponData)
            //{
            //    WeaponData.ApplyAbilityData(playerAttack.m_leftWeaponData, GameManager.RetrieveAbilityData(Hand.LEFT));
            //    if (playerAttack.m_leftWeaponData.abilityData)
            //        playerAttack.m_leftWeaponData.abilityData.droppedEnergyColor = GameManager.RetrieveOutlineColor(Hand.LEFT);
            //}
            //if (playerAttack.m_rightWeaponData)
            //{
            //    WeaponData.ApplyAbilityData(playerAttack.m_rightWeaponData, GameManager.RetrieveAbilityData(Hand.RIGHT));
            //    if (playerAttack.m_rightWeaponData.abilityData)
            //        playerAttack.m_rightWeaponData.abilityData.droppedEnergyColor = GameManager.RetrieveOutlineColor(Hand.RIGHT);
            //}

            playerStats.m_effects = GameManager.RetrieveEffectsDictionary();
            m_inkmanClass = GameManager.RetrieveClassData();

            playerStats.EvaluateEffects();

            if (GameManager.RetrieveHealth() <= 0.0f)
                playerResources.SetHealth(1.0f);
            else
                playerResources.SetHealth(GameManager.RetrieveHealth());

            playerResources.SetOrbCount(GameManager.RetrieveOrbCount());
        }
        else
        {
            if (playerAttack.m_leftWeaponData)
            {
                WeaponData clonedLeftData = ScriptableObject.CreateInstance<WeaponData>();
                clonedLeftData.Clone(playerAttack.m_leftWeaponData);
                playerAttack.m_leftWeaponData = clonedLeftData;
            }
            if (playerAttack.m_rightWeaponData)
            {
                WeaponData clonedRightData = ScriptableObject.CreateInstance<WeaponData>();
                clonedRightData.Clone(playerAttack.m_rightWeaponData);
                playerAttack.m_rightWeaponData = clonedRightData;
            }
        }
        playerSkills.EvaluateSkills();

        playerAttack.m_leftWeaponEffect = GameManager.RetrieveWeaponEffect(Hand.LEFT);
        playerAttack.m_rightWeaponEffect = GameManager.RetrieveWeaponEffect(Hand.RIGHT);

        playerAttack.ApplyWeaponData(Hand.LEFT);
        playerAttack.ApplyWeaponData(Hand.RIGHT);

        if (m_inkmanClass)
            playerClassArmour.SetClassArmour(m_inkmanClass.inkmanClass);
        else
            playerClassArmour.SetClassArmour(InkmanClass.GENERAL);

    }

    public void RespawnPlayerTo(Vector3 _position, bool _isFullHP = false)
    {
        playerMovement.characterController.enabled = false;
        transform.position = _position;
        playerMovement.characterController.enabled = true;

        if (_isFullHP)
        {
            playerResources.FullHeal();
        }

        animator.SetTrigger("RespawnPlayer");
        animator.SetFloat("Vertical", 0.0f);
        animator.SetFloat("Horizontal", 0.0f);
    }

    GameObject tempCameraTransformObject;
    public void KillPlaneDeath(bool _isFullHP = false)
    {
        if (tempCameraTransformObject != null)
            Destroy(tempCameraTransformObject);

        tempCameraTransformObject = new GameObject("TempCameraTransform");
        tempCameraTransformObject.transform.position = transform.position;
        
        m_cameraFocusLerp = 1.0f;

        ChangeCameraFocus(tempCameraTransformObject.transform, 1.0f, false);
        StartCoroutine(RespawnDelay(_isFullHP));
    }
    IEnumerator RespawnDelay(bool _isFullHP = false)
    {
        yield return new WaitForSeconds(2.0f);

        RespawnPlayerToGround(_isFullHP);

        ResetCameraFocus(2.0f);

        Destroy(tempCameraTransformObject);
    }
    public void RespawnPlayerToGround(bool _isFullHP = false)
    {
        Vector3 targetPosition = playerMovement.m_lastGroundedPosition/* - playerMovement.m_lastGroundedVelocity.normalized * 2.0f*/;
        RespawnPlayerTo(targetPosition, _isFullHP);
    }
    public void StartKneel(ClassData _class)
    {
        StartCoroutine(KneelRoutine(_class));
    }
    IEnumerator KneelRoutine(ClassData _class)
    {
        animator.SetTrigger("Kneel");
        m_isKneeling = true;
        yield return new WaitForSeconds(1.0f);
        SelectClass(_class);
        yield return new WaitForSeconds(1.0f);
        m_isKneeling = false;
    }
    public void SelectClass(ClassData _class)
    {
        m_inkmanClass = _class;

        if (_class.startWeapon != null)
        {
            WeaponData clonedWeapon = ScriptableObject.CreateInstance<WeaponData>();
            clonedWeapon.Clone(_class.startWeapon);
            if (playerAttack.m_leftWeaponData?.m_level == -1)
            {
                playerAttack.SetWeaponData(Hand.LEFT, clonedWeapon);
            }
            else if (playerAttack.m_rightWeaponData?.m_level == -1)
            {
                playerAttack.SetWeaponData(Hand.RIGHT, clonedWeapon);
            }
        }

        playerClassArmour.SetClassArmour(_class.inkmanClass);

        playerSkills.EvaluateSkills();

        playerStats.m_effects = new Dictionary<EffectData, int>();

        for (int i = 0; i < _class.movementSpeed; i++)
        {
            playerStats.AddEffect(ItemEffect.MOVE_SPEED);
        }

        for (int i = 0; i < _class.attackSpeed; i++)
        {
            playerStats.AddEffect(ItemEffect.ATTACK_SPEED);
        }

        for (int i = 0; i < _class.abilityCD; i++)
        {
            playerStats.AddEffect(ItemEffect.ABILITY_CD);
        }

        for (int i = 0; i < _class.maximumHealth; i++)
        {
            playerStats.AddEffect(ItemEffect.MAX_HEALTH_INCREASE);
        }

        for (int i = 0; i < _class.physicalDamage; i++)
        {
            playerStats.AddEffect(ItemEffect.PHYSICAL_DAMAGE);
        }

        for (int i = 0; i < _class.abilityDamage; i++)
        {
            playerStats.AddEffect(ItemEffect.ABILITY_DAMAGE);
        }

        for (int i = 0; i < _class.physicalDefence; i++)
        {
            playerStats.AddEffect(ItemEffect.PHYSICAL_DEFENCE);
        }

        for (int i = 0; i < _class.abilityDefence; i++)
        {
            playerStats.AddEffect(ItemEffect.ABILITY_DEFENCE);
        }
    }    

    public void UpgradeWeapon(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (playerAttack.m_leftWeaponData)
                    playerAttack.m_leftWeaponData = WeaponData.UpgradeWeaponLevel(playerAttack.m_leftWeaponData);
                playerAttack.ApplyWeaponData(Hand.LEFT);
                break;
            case Hand.RIGHT:
                if (playerAttack.m_rightWeaponData)
                    playerAttack.m_rightWeaponData = WeaponData.UpgradeWeaponLevel(playerAttack.m_rightWeaponData);
                playerAttack.ApplyWeaponData(Hand.RIGHT);
                break;
            case Hand.NONE:
                break;
            default:
                break;
        }
    }

    public void SetGodMode(bool _active)
    {
        m_godMode = _active;
    }

    public Vector3 GetFloorPosition()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 2.0f, Vector3.down, 2.0f, m_waterLayer);
        if (hits.Length > 0)
        {
            return hits[0].point + Vector3.up * 0.2f;
        }

        return transform.position;
    }
}
