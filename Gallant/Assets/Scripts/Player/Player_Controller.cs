using System.Collections;
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

    // Player components
    public Player_Movement playerMovement { private set; get; }
    public Player_Attack playerAttack { private set; get; }
    public Player_Abilities playerAbilities { private set; get; }
    public Player_Resources playerResources { private set; get; }
    public Player_Pickup playerPickup { private set; get; }
    public Player_Stats playerStats { private set; get; }

    [Header("Keyboard Movement")]
    private Vector3 m_currentVelocity = Vector3.zero;
    private Vector3 m_movementVelocity = Vector3.zero;
    private Vector2 m_lastAimDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;

        playerMovement = GetComponent<Player_Movement>();
        playerAttack = GetComponent<Player_Attack>();
        playerAbilities = GetComponent<Player_Abilities>();
        playerResources = GetComponent<Player_Resources>();
        playerPickup = GetComponentInChildren<Player_Pickup>();
        playerStats = GetComponentInChildren<Player_Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set animation speeds based on stats
        animator.SetFloat("MovementSpeed", playerStats.m_movementSpeed / 100.0f);
        animator.SetFloat("AttackSpeed", playerStats.m_attackSpeed / 100.0f);

        // Set gamepad being used
        int gamepadID = InputManager.instance.GetAnyGamePad();

        // Move player
        playerMovement.Move(GetPlayerMovementVector(), GetPlayerAimVector(), InputManager.instance.IsGamepadButtonDown(ButtonType.EAST, gamepadID) || InputManager.instance.IsKeyDown(KeyType.SPACE), Time.deltaTime);

        if (!playerMovement.m_isStunned) // Make sure player is not stunned
        {
            // Left hand pickup
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.LEFT, gamepadID) || InputManager.instance.IsKeyDown(KeyType.R))
            {
                DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
                if (droppedWeapon != null)
                    playerAttack.PickUpWeapon(droppedWeapon, Hand.LEFT);
            }

            // Right hand pickup
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.RIGHT, gamepadID) || InputManager.instance.IsKeyDown(KeyType.F))
            {
                DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
                if (droppedWeapon != null)
                    playerAttack.PickUpWeapon(droppedWeapon, Hand.RIGHT);
            }

            // Weapon attacks
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.RB, gamepadID) || InputManager.instance.GetMouseDown(MouseButton.RIGHT))
            {
                playerAttack.UseWeapon(Hand.RIGHT);
            }
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.LB, gamepadID) || InputManager.instance.GetMouseDown(MouseButton.LEFT))
            {
                playerAttack.UseWeapon(Hand.LEFT);
            }

            // Ability attacks
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.RT, gamepadID) || InputManager.instance.IsKeyDown(KeyType.E))
            {
                playerAbilities.UseAbility(Hand.RIGHT);
            }
            if (InputManager.instance.IsGamepadButtonDown(ButtonType.LT, gamepadID) || InputManager.instance.IsKeyDown(KeyType.Q))
            {
                playerAbilities.UseAbility(Hand.LEFT);
            }
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

        // Debug controls
        if (InputManager.instance.IsKeyDown(KeyType.NUM_ONE))
        {
            playerResources.ChangeHealth(-10.0f);
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
            StunPlayer(0.2f, (transform.position - Vector3.zero).normalized * 12.0f);
        }

        // Item debug
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
        if (GameManager.instance.useGamepad) // If using gamepad
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
            m_currentVelocity = Vector3.SmoothDamp(m_currentVelocity, movement, ref m_movementVelocity, 0.1f);
            return m_currentVelocity;
        }

    }

    private Vector2 GetPlayerAimVector()
    {
        if (GameManager.instance.useGamepad) // If using gamepad
        {
            int gamepadID = InputManager.instance.GetAnyGamePad();
            return InputManager.instance.GetGamepadStick(StickType.RIGHT, gamepadID);
        }
        else // If using mouse
        {
            // Raycast to find raycast point
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
            if (Physics.Raycast(ray, out hit, 1000))
            {
                // Return direction from player to hit point
                Vector3 aim = hit.point - transform.position;

                Vector3 normalizedAim = Vector3.zero;
                normalizedAim += aim.z * -transform.right;
                normalizedAim += aim.x * transform.forward;
                normalizedAim *= -1;
                m_lastAimDirection = new Vector2(normalizedAim.x, normalizedAim.z);
            }
            return m_lastAimDirection;
        }
    }

    public void DamagePlayer(float _damage)
    {
        Debug.Log($"Player is damaged: {_damage} points of health.");
        playerResources.ChangeHealth(-_damage * (100.0f - playerStats.m_damageResistance));
    }
}
