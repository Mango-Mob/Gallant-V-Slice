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
    private Camera playerCamera;
    public Animator animator;
    public AvatarMask armsMask;
    public LayerMask m_mouseAimingRayLayer;
    public bool m_isDisabledInput = false;
    public float m_standMoveWeightLerpSpeed = 0.5f;
    private Hand m_lastAttackHand = Hand.NONE;

    // Player components
    public Player_Movement playerMovement { private set; get; }
    public Player_Attack playerAttack { private set; get; }
    public Player_Abilities playerAbilities { private set; get; }
    public Player_Resources playerResources { private set; get; }
    public Player_Pickup playerPickup { private set; get; }
    public Player_Stats playerStats { private set; get; }
    public Player_AudioAgent playerAudioAgent { private set; get; }

    [Header("Dual Wielding Stats")]
    public float m_dualWieldSpeed = 1.3f;
    private float m_dualWieldBonus = 1.0f;

    [Header("Keyboard Movement")]
    private Vector3 m_currentVelocity = Vector3.zero;
    private Vector3 m_movementVelocity = Vector3.zero;
    private Vector2 m_lastAimDirection = Vector2.zero;

    private bool m_isAiming = false;

    private Animator animatorCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        animatorCamera = playerCamera.GetComponent<Animator>();

        playerMovement = GetComponent<Player_Movement>();
        playerAbilities = GetComponent<Player_Abilities>();
        playerAttack = GetComponent<Player_Attack>();
        playerResources = GetComponent<Player_Resources>();
        playerPickup = GetComponentInChildren<Player_Pickup>();
        playerStats = GetComponentInChildren<Player_Stats>();
        playerAudioAgent = GetComponent<Player_AudioAgent>();

        if (GameManager.m_containsPlayerInfo)
        {
            playerAttack.m_leftWeapon = GameManager.RetrieveWeaponData(Hand.LEFT);
            playerAttack.m_rightWeapon = GameManager.RetrieveWeaponData(Hand.RIGHT);

            playerStats.m_effects = GameManager.RetrieveEffectsDictionary();
            playerStats.EvaluateEffects();
        }

        playerAttack.ApplyWeaponData(Hand.LEFT);
        playerAttack.ApplyWeaponData(Hand.RIGHT);

    }

    // Update is called once per frame
    void Update()
    {
        if (UI_PauseMenu.isPaused || playerResources.m_dead || m_isDisabledInput)
            return;

        // Set gamepad being used
        int gamepadID = InputManager.instance.GetAnyGamePad();

        // Set animation speeds based on stats
        animator.SetFloat("MovementSpeed", playerStats.m_movementSpeed);
        animator.SetFloat("LeftAttackSpeed", m_dualWieldBonus * playerStats.m_attackSpeed * (playerAttack.m_leftWeapon == null ? 1.0f : playerAttack.m_leftWeapon.m_speed));
        animator.SetFloat("RightAttackSpeed", m_dualWieldBonus * playerStats.m_attackSpeed * (playerAttack.m_rightWeapon == null ? 1.0f : playerAttack.m_rightWeapon.m_speed));

        bool rightAttackHeld = InputManager.instance.IsBindPressed("Right_Attack", gamepadID); 
        bool leftAttackHeld = InputManager.instance.IsBindPressed("Left_Attack", gamepadID);

        animator.SetBool("RightAttackHeld", rightAttackHeld);
        animator.SetBool("LeftAttackHeld", leftAttackHeld);

        float swordRunWeight = 0.0f;
        if (playerAttack.m_leftWeapon != null)
            swordRunWeight += playerAttack.m_leftWeapon.weaponType == Weapon.SWORD ? -1.0f : 0.0f;
        if (playerAttack.m_rightWeapon != null)
            swordRunWeight += playerAttack.m_rightWeapon.weaponType == Weapon.SWORD ? 1.0f : 0.0f;

        animator.SetFloat("SwordRunWeight", swordRunWeight);

        if (!rightAttackHeld || playerMovement.m_isStunned || playerMovement.m_isRolling)
            playerAttack.StopBlock(Hand.RIGHT);
        if (!leftAttackHeld || playerMovement.m_isStunned || playerMovement.m_isRolling)
            playerAttack.StopBlock(Hand.LEFT);

        //float armWeight = animator.GetLayerWeight(animator.GetLayerIndex("Arm"));
        //float standArmWeight = animator.GetLayerWeight(animator.GetLayerIndex("StandArm"));
        //// Set avatar mask to be used
        //if (animator.GetFloat("Horizontal") != 0.0f || animator.GetFloat("Vertical") != 0.0f)
        //{
        //    armWeight += Time.deltaTime * m_standMoveWeightLerpSpeed;
        //    standArmWeight -= Time.deltaTime * m_standMoveWeightLerpSpeed;
        //}
        //else
        //{
        //    armWeight -= Time.deltaTime * m_standMoveWeightLerpSpeed;
        //    standArmWeight += Time.deltaTime * m_standMoveWeightLerpSpeed;
        //}

        float armWeight = 0.0f;
        float standArmWeight = 1.0f;

        animator.SetLayerWeight(animator.GetLayerIndex("Arm"), Mathf.Clamp(armWeight, 0.0f, 1.0f));
        animator.SetLayerWeight(animator.GetLayerIndex("StandArm"), Mathf.Clamp(standArmWeight, 0.0f, 0.9f));

        // Move player
        playerMovement.Move(GetPlayerMovementVector(), GetPlayerAimVector(), InputManager.instance.IsBindDown("Roll", gamepadID), Time.deltaTime);

        if (!playerMovement.m_isStunned && !playerMovement.m_isRolling) // Make sure player is not stunned
        {
            // Left hand pickup
            if (InputManager.instance.IsBindPressed("Left_Pickup", gamepadID))
            {
                DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
                if (droppedWeapon != null)
                {
                    if (droppedWeapon.m_pickupDisplay.UpdatePickupTimer(playerAttack.m_leftWeapon, Hand.LEFT))
                    {
                        playerAttack.PickUpWeapon(droppedWeapon, Hand.LEFT);
                        playerPickup.RemoveDropFromList(droppedWeapon);
                    }
                }
            }

            // Right hand pickup
            if (InputManager.instance.IsBindPressed("Right_Pickup", gamepadID))
            {
                DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
                if (droppedWeapon != null)
                {
                    if (droppedWeapon.m_pickupDisplay.UpdatePickupTimer(playerAttack.m_rightWeapon, Hand.RIGHT))
                    {
                        playerAttack.PickUpWeapon(droppedWeapon, Hand.RIGHT);
                        playerPickup.RemoveDropFromList(droppedWeapon);
                    }
                }
            }

            bool rightWeaponAttack = InputManager.instance.IsBindPressed("Right_Attack", gamepadID);
            bool leftWeaponAttack = InputManager.instance.IsBindPressed("Left_Attack", gamepadID);

            if (playerAttack.IsDuelWielding() && rightWeaponAttack && leftWeaponAttack) // Dual attacking
                m_dualWieldBonus = m_dualWieldSpeed;
            else
                m_dualWieldBonus = 1.0f;

            // Weapon attacks
            if (playerAttack.GetCurrentAttackingHand() == Hand.NONE)
            {
                if (rightWeaponAttack && leftWeaponAttack) // Dual attacking
                {
                    if (m_lastAttackHand == Hand.RIGHT)
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
            else
            {
                m_lastAttackHand = playerAttack.GetCurrentAttackingHand();
            }

            // Ability attacks
            if (InputManager.instance.IsBindDown("Right_Ability", gamepadID))
            {
                playerAbilities.StartUsing(Hand.RIGHT);
            }
            if (InputManager.instance.IsBindDown("Left_Ability", gamepadID))
            {
                playerAbilities.StartUsing(Hand.LEFT);
            }
        }

        if (InputManager.instance.IsBindDown("Toggle_Lockon", gamepadID))
        {
            playerMovement.LockOnTarget();
        }

        if (InputManager.instance.IsBindDown("Consume", gamepadID))
        {
            // Heal from adrenaline
            playerResources.UseAdrenaline();
        }

        if (InputManager.instance.IsBindDown("Switch", gamepadID))
        {
            playerAttack.SwapWeapons();
        }

#if UNITY_EDITOR
        // Debug controls
        if (InputManager.instance.IsKeyDown(KeyType.NUM_ONE))
        {
            DamagePlayer(20.0f, FindObjectOfType<Actor>().gameObject, false);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_TWO))
        {
            playerResources.ChangeAdrenaline(-1);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_THREE))
        {
            playerResources.ChangeAdrenaline(1);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_FOUR))
        {
            StunPlayer(0.2f, transform.up * 80.0f);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_FIVE))
        {
            AdrenalineDrop.CreateAdrenalineDropGroup(5, new Vector3(0, transform.position.y + 0.5f, 0));
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_ZERO))
        {
            //playerResources.ChangeBarrier(10.0f);
            LevelLoader.instance.ReloadLevel();
        }

        // Item debug
        if (InputManager.instance.IsKeyDown(KeyType.NUM_SIX))
        {
            //playerStats.AddEffect(ItemEffect.MAX_HEALTH_INCREASE);
            StorePlayerInfo();
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_SEVEN))
        {
            playerStats.AddEffect(ItemEffect.ABILITY_CD);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_EIGHT))
        {
            playerStats.AddEffect(ItemEffect.ATTACK_SPEED);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_NINE))
        {
            playerStats.AddEffect(ItemEffect.MOVE_SPEED);
        }
#endif
    }
    /*******************
     * StunPlayer : Calls playerMovement StunPlayer function.
     * @author : William de Beer
     * @param : (float) Stun duration, (Vector3) Knockback velocity
     */
    public void StunPlayer(float _stunDuration, Vector3 _knockbackVelocity)
    {
        playerMovement.StunPlayer(_stunDuration, _knockbackVelocity);
    }
    private Vector2 GetPlayerMovementVector()
    {
        if (InputManager.instance.isInGamepadMode) // If using gamepad
        {
            int gamepadID = InputManager.instance.GetAnyGamePad();
            return InputManager.instance.GetBindStick("Move", gamepadID);
        }
        else // If using keyboard
        {
            Vector2 movement = Vector2.zero;
            movement.x += (InputManager.instance.IsBindPressed("Move_Right") ? 1.0f : 0.0f);
            movement.x -= (InputManager.instance.IsBindPressed("Move_Left") ? 1.0f : 0.0f);
            movement.y += (InputManager.instance.IsBindPressed("Move_Forward") ? 1.0f : 0.0f);
            movement.y -= (InputManager.instance.IsBindPressed("Move_Backward") ? 1.0f : 0.0f);
            movement.Normalize();
            m_currentVelocity = Vector3.SmoothDamp(m_currentVelocity, movement, ref m_movementVelocity, 0.1f);
            return m_currentVelocity;
        }

    }

    private Vector2 GetPlayerAimVector()
    {
        if (InputManager.instance.isInGamepadMode) // If using gamepad
        {
            int gamepadID = InputManager.instance.GetAnyGamePad();
            return InputManager.instance.GetBindStick("Aim", gamepadID);
        }
        else // If using mouse
        {
            if(InputManager.instance.IsBindDown("Toggle_Aim"))
            {
                m_isAiming = !m_isAiming;
            }

            if (m_isAiming)
            {
                // Raycast to find raycast point
                RaycastHit hit;
                Ray ray = playerCamera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
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
            }
            else
            {
                m_lastAimDirection = new Vector2(0.0f, 0.0f);
            }
            return m_lastAimDirection;
        }
    }
    public List<Actor> GetActorsInfrontOfPlayer(float _angle, float _distance)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _distance, playerAttack.m_attackTargets);
        List<Actor> targets = new List<Actor>();

        foreach (var collider in colliders)
        {
            Actor actor = collider.GetComponentInParent<Actor>();
            if (actor == null)
                continue;

            Vector3 direction = (actor.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(direction, playerMovement.playerModel.transform.forward);
            if (dot >= Mathf.Cos(_angle * Mathf.Deg2Rad))
            {
                targets.Add(actor);
            }
        }
        return targets;
    }
    public List<Collider> GetCollidersInfrontOfPlayer(float _angle, float _distance)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _distance);
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
    public void DamagePlayer(float _damage, GameObject _attacker = null, bool _bypassInvincibility = false)
    {
        if (!_bypassInvincibility && playerMovement.m_isRollInvincible)
            return;

        if (playerAttack.m_isBlocking && _attacker != null)
        {
            if (IsInfrontOfPlayer(playerAttack.m_blockingAngle, _attacker.transform.position)) 
            {
                // PLAY BLOCK SOUND
                Debug.Log("BLOCK");
                animator.SetTrigger("HitPlayer");
                playerAudioAgent.PlayShieldBlock();
                return;
            }
        }

        playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.HIT_RECIEVED, (_attacker != null) ? _attacker.gameObject : null, _damage);
        playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.HIT_RECIEVED, (_attacker != null) ? _attacker.gameObject : null, _damage);

        Debug.Log($"Player is damaged: {_damage} points of health.");
        playerResources.ChangeHealth(-playerResources.ChangeBarrier(-_damage * (1.0f - playerStats.m_damageResistance)));

        animator.SetTrigger("HitPlayer");

        if (animatorCamera)
            animatorCamera.SetTrigger("Shake");
    }

    public void StorePlayerInfo()
    {
        GameManager.StorePlayerInfo(playerAttack.m_leftWeapon, playerAttack.m_rightWeapon, playerStats.m_effects);
    }
}
