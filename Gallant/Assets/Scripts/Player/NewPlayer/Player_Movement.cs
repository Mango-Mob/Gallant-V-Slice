using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Player_Movement by William de Beer
 * File: Player_Movement.cs
 * Description:
 *		Contains logic for player movement such as running and dodging.
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
    private Vector3 m_lastMoveDirection;

    private bool m_grounded = true;
    private float m_yVelocity = 0.0f;

    [Header("Dodge Attributes")]
    public float m_shadowDuration = 1.0f;
    public GameObject m_adrenShadowPrefab;
    private float m_rollTimer = 0.0f;
    public float m_rollDuration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
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
    }

    private void RollUpdate()
    {
        if (m_isRolling) // Check if the player is supposed to be rolling
        {
            // Move player in stored direction while roll is active
            characterController.Move(m_lastMoveDirection.normalized * m_rollSpeed * Time.fixedDeltaTime
                + transform.up * m_yVelocity * Time.fixedDeltaTime);
            RotateToFaceDirection(new Vector3(m_lastMoveDirection.x, 0, m_lastMoveDirection.z));

            m_rollTimer -= Time.fixedDeltaTime;
            if (m_rollTimer <= 0.0f)
                m_isRolling = false;
        }
    }
    public void StopRoll()
    {
        m_isRolling = false;
    }

    public void Move(Vector2 _move, Vector2 _aim, bool _roll, float _deltaTime)
    {
        if (m_isRolling) // If the player is rolling prevent other movement
            return;
        if (_aim.magnitude != 0) // If the player is trying to aim...
        {
            // Make player model face aim direction
            Vector3 normalizedAim = Vector3.zero;
            normalizedAim += _aim.y * transform.forward;
            normalizedAim += _aim.x * transform.right;
            RotateToFaceDirection(new Vector3(normalizedAim.x, 0, normalizedAim.z));
        }

        float speed = m_moveSpeed; // Player movement speed

        Vector3 normalizedMove = Vector3.zero;
        Vector3 movement = Vector3.zero;

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
        }
        if (_roll) // If roll input is triggered
        {
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
        // Move
        characterController.Move(movement + transform.up * m_yVelocity * Time.fixedDeltaTime);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + playerModel.transform.forward * 2.0f);
    }
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
    public void GiveAdrenaline(float _val)
    {
        playerController.playerResources.ChangeAdrenaline(100 * _val);

        GetComponent<Player_AudioAgent>().PlayAdrenalineGain();

        //Slow motion
        GameManager.instance.SlowTime(0.4f, _val);
    }
}
