using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Camera camera;

    // Player components
    public Player_Movement playerMovement { private set; get; }
    public Player_Attack playerAttack { private set; get; }
    public Player_Abilities playerAbilities { private set; get; }
    public Player_Resources playerResources { private set; get; }

    [Header("Keyboard Movement")]
    private Vector3 m_currentVelocity = Vector3.zero;
    private Vector3 m_movementVelocity = Vector3.zero;
    private Vector2 m_lastAimDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;

        playerMovement = GetComponent<Player_Movement>();
        playerAttack = GetComponent<Player_Attack>();
        playerAbilities = GetComponent<Player_Abilities>();
        playerResources = GetComponent<Player_Resources>();
    }

    // Update is called once per frame
    void Update()
    {
        int gamepadID = InputManager.instance.GetAnyGamePad();
        playerMovement.Move(GetPlayerMovementVector(), GetPlayerAimVector(), InputManager.instance.IsGamepadButtonDown(ButtonType.EAST, gamepadID) || InputManager.instance.IsKeyDown(KeyType.SPACE), Time.deltaTime);
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
        if (GameManager.instance.useGamepad)
        {
            int gamepadID = InputManager.instance.GetAnyGamePad();
            return InputManager.instance.GetGamepadStick(StickType.RIGHT, gamepadID);
        }
        else
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(InputManager.instance.GetMousePositionInScreen());
            if (Physics.Raycast(ray, out hit, 1000))
            {
                Vector2 aim = new Vector2(hit.point.x - transform.position.x, hit.point.z - transform.position.z);

                m_lastAimDirection = aim;
                return aim;
            }
            return m_lastAimDirection;
        }
    }
}
