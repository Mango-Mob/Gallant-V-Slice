﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Player components
    public Player_Movement playerMovement { private set; get; }
    public Player_Attack playerAttack { private set; get; }
    public Player_Abilities playerAbilities { private set; get; }
    public Player_Resources playerResources { private set; get; }
    public Player_Pickup playerPickup { private set; get; }
    public Player_Stats playerStats { private set; get; }
    public Player_AudioAgent playerAudioAgent { private set; get; }


    [Header("Keyboard Movement")]
    private Vector3 m_currentVelocity = Vector3.zero;
    private Vector3 m_movementVelocity = Vector3.zero;
    private Vector2 m_lastAimDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;

        playerMovement = GetComponent<Player_Movement>();
        playerAbilities = GetComponent<Player_Abilities>();
        playerAttack = GetComponent<Player_Attack>();
        playerResources = GetComponent<Player_Resources>();
        playerPickup = GetComponentInChildren<Player_Pickup>();
        playerStats = GetComponentInChildren<Player_Stats>();
        playerAudioAgent = GetComponent<Player_AudioAgent>();

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
        animator.SetFloat("LeftAttackSpeed", playerStats.m_attackSpeed * (playerAttack.m_leftWeapon == null ? 1.0f : playerAttack.m_leftWeapon.m_speed));
        animator.SetFloat("RightAttackSpeed", playerStats.m_attackSpeed * (playerAttack.m_rightWeapon == null ? 1.0f : playerAttack.m_rightWeapon.m_speed));

        bool rightAttackHeld = InputManager.instance.IsGamepadButtonPressed(ButtonType.RB, gamepadID) || InputManager.instance.GetMouseButtonPressed(MouseButton.RIGHT);
        bool leftAttackHeld = InputManager.instance.IsGamepadButtonPressed(ButtonType.LB, gamepadID) || InputManager.instance.GetMouseButtonPressed(MouseButton.LEFT);

        animator.SetBool("RightAttackHeld", rightAttackHeld);
        animator.SetBool("LeftAttackHeld", leftAttackHeld);

        if (!rightAttackHeld || playerMovement.m_isStunned || playerMovement.m_isRolling)
            playerAttack.StopBlock(Hand.RIGHT);
        if (!leftAttackHeld || playerMovement.m_isStunned || playerMovement.m_isRolling)
            playerAttack.StopBlock(Hand.LEFT);

        float armWeight = animator.GetLayerWeight(animator.GetLayerIndex("Arm"));
        float standArmWeight = animator.GetLayerWeight(animator.GetLayerIndex("StandArm"));
        // Set avatar mask to be used
        if (animator.GetFloat("Horizontal") != 0.0f || animator.GetFloat("Vertical") != 0.0f)
        {
            armWeight += Time.deltaTime * m_standMoveWeightLerpSpeed;
            standArmWeight -= Time.deltaTime * m_standMoveWeightLerpSpeed;
        }
        else
        {
            armWeight -= Time.deltaTime * m_standMoveWeightLerpSpeed;
            standArmWeight += Time.deltaTime * m_standMoveWeightLerpSpeed;
        }

        animator.SetLayerWeight(animator.GetLayerIndex("Arm"), Mathf.Clamp(armWeight, 0.0f, 1.0f));
        animator.SetLayerWeight(animator.GetLayerIndex("StandArm"), Mathf.Clamp(standArmWeight, 0.0f, 0.9f));

        // Move player
        playerMovement.Move(GetPlayerMovementVector(), GetPlayerAimVector(), InputManager.instance.IsGamepadButtonDown(ButtonType.EAST, gamepadID) || InputManager.instance.IsKeyDown(KeyType.SPACE), Time.deltaTime);

        if (!playerMovement.m_isStunned && !playerMovement.m_isRolling) // Make sure player is not stunned
        {
            // Left hand pickup
            if (InputManager.instance.IsGamepadButtonPressed(ButtonType.LEFT, gamepadID) || InputManager.instance.IsKeyPressed(KeyType.R))
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
            if (InputManager.instance.IsGamepadButtonPressed(ButtonType.RIGHT, gamepadID) || InputManager.instance.IsKeyPressed(KeyType.F))
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

            // Weapon attacks
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.RB, gamepadID) || InputManager.instance.GetMouseDown(MouseButton.RIGHT))
            {
                playerAttack.StartUsing(Hand.RIGHT);
                //playerAttack.UseWeapon(false);
            }
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.LB, gamepadID) || InputManager.instance.GetMouseDown(MouseButton.LEFT))
            {
                playerAttack.StartUsing(Hand.LEFT);
                //playerAttack.UseWeapon(true);
            }

            // Ability attacks
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.RT, gamepadID) || InputManager.instance.IsKeyDown(KeyType.E))
            {
                playerAbilities.StartUsing(Hand.RIGHT);
            }
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.LT, gamepadID) || InputManager.instance.IsKeyDown(KeyType.Q))
            {
                playerAbilities.StartUsing(Hand.LEFT);
            }
        }

        if (InputManager.instance.IsGamepadButtonDown(ButtonType.RS, gamepadID) || InputManager.instance.IsKeyDown(KeyType.L_ALT))
        {
            playerMovement.LockOnTarget();
        }

        if (InputManager.instance.IsGamepadButtonDown(ButtonType.NORTH, gamepadID) || InputManager.instance.IsKeyDown(KeyType.V))
        {
            // Heal from adrenaline
            playerResources.UseAdrenaline();
        }

        if (InputManager.instance.IsGamepadButtonDown(ButtonType.UP, gamepadID) || InputManager.instance.IsKeyDown(KeyType.Y))
        {
            playerAttack.SwapWeapons();
        }

#if UNITY_EDITOR
        // Debug controls
        if (InputManager.instance.IsKeyDown(KeyType.NUM_ONE))
        {
            //playerResources.ChangeHealth(-10.0f, FindObjectOfType<Actor>().gameObject);
            DamagePlayer(1.0f, FindObjectOfType<Actor>().gameObject, false);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_TWO))
        {
            playerResources.ChangeAdrenaline(-0.2f);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_THREE))
        {
            playerResources.ChangeAdrenaline(0.2f);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_FOUR))
        {
            StunPlayer(0.2f, transform.up * 80.0f);
        }
        if (InputManager.instance.IsKeyDown(KeyType.NUM_FIVE))
        {
            AdrenalineDrop.CreateAdrenalineDropGroup(5, new Vector3(0, transform.position.y + 0.5f, 0));
        }

        // Item debug
        if (InputManager.instance.IsKeyDown(KeyType.NUM_SIX))
        {
            playerStats.AddEffect(ItemEffect.MAX_HEALTH_INCREASE);
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
            return InputManager.instance.GetGamepadStick(StickType.LEFT, gamepadID);
        }
        else // If using keyboard
        {
            Vector2 movement = Vector2.zero;
            movement.x += (InputManager.instance.IsKeyPressed(KeyType.D) ? 1.0f : 0.0f);
            movement.x -= (InputManager.instance.IsKeyPressed(KeyType.A) ? 1.0f : 0.0f);
            movement.y += (InputManager.instance.IsKeyPressed(KeyType.W) ? 1.0f : 0.0f);
            movement.y -= (InputManager.instance.IsKeyPressed(KeyType.S) ? 1.0f : 0.0f);
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
            return InputManager.instance.GetGamepadStick(StickType.RIGHT, gamepadID);
        }
        else // If using mouse
        {
            if (InputManager.instance.IsKeyPressed(KeyType.L_CTRL) || InputManager.instance.IsKeyPressed(KeyType.L_SHIFT))
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
            Actor actor = collider.GetComponent<Actor>();
            if (actor == null)
                continue;

            Vector3 direction = (actor.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(direction, playerMovement.playerModel.transform.forward);
            float TEMPCOS = Mathf.Cos(_angle);
            if (dot >= Mathf.Cos(_angle * Mathf.Deg2Rad))
            {
                targets.Add(actor);
            }

        }
        return targets;
    }
    public void DamagePlayer(float _damage, GameObject _attacker = null, bool _bypassInvincibility = false)
    {
        if (!_bypassInvincibility && playerMovement.m_isRollInvincible)
            return;

        playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.HIT_RECIEVED, _attacker.gameObject, _damage);
        playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.HIT_RECIEVED, _attacker.gameObject, _damage);

        Debug.Log($"Player is damaged: {_damage} points of health.");
        playerResources.ChangeHealth(-_damage * (1.0f - playerStats.m_damageResistance));

        animator.SetTrigger("HitPlayer");
    }
}
