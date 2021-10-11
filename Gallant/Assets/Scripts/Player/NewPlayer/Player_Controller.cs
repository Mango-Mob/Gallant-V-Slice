using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Player_Controller by William de Beer
 * File: Player_Controller.cs
 * Description:
 *		Controls interactions between different player components and handles player input.
 */
public class Player_Controller : MonoBehaviour
{
    private Camera playerCamera;

    // Player components
    public Player_Movement playerMovement { private set; get; }
    public Player_Attack playerAttack { private set; get; }
    public Player_Abilities playerAbilities { private set; get; }
    public Player_Resources playerResources { private set; get; }
    public Player_Pickup playerPickup { private set; get; }

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
    }

    // Update is called once per frame
    void Update()
    {
        // Set gamepad being used
        int gamepadID = InputManager.instance.GetAnyGamePad();

        // Move player
        playerMovement.Move(GetPlayerMovementVector(), GetPlayerAimVector(), InputManager.instance.IsGamepadButtonDown(ButtonType.EAST, gamepadID) || InputManager.instance.IsKeyDown(KeyType.SPACE), Time.deltaTime);

        // Left hand pickup
        if (InputManager.instance.IsGamepadButtonDown(ButtonType.WEST, gamepadID) || InputManager.instance.IsKeyDown(KeyType.R))
        {
            DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
            if (droppedWeapon != null)
                playerAttack.PickUpWeapon(droppedWeapon, Hand.LEFT);
        }

        // Right hand pickup
        if (InputManager.instance.IsGamepadButtonDown(ButtonType.NORTH, gamepadID) || InputManager.instance.IsKeyDown(KeyType.F))
        {
            DroppedWeapon droppedWeapon = playerPickup.GetClosestWeapon();
            if (droppedWeapon != null)
                playerAttack.PickUpWeapon(droppedWeapon, Hand.RIGHT);
        }

        // Weapon attacks
        if (InputManager.instance.IsGamepadButtonPressed(ButtonType.RB, gamepadID) || InputManager.instance.GetMouseDown(MouseButton.RIGHT))
        {
            playerAttack.UseWeapon(Hand.RIGHT);
        }
        if (InputManager.instance.IsGamepadButtonPressed(ButtonType.LB, gamepadID) || InputManager.instance.GetMouseDown(MouseButton.LEFT))
        {
            playerAttack.UseWeapon(Hand.LEFT);
        }
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
}
