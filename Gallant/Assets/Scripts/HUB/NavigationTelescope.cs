using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationTelescope : MonoBehaviour
{
    [System.Serializable]
    public struct Destination
    {
        public string sceneName;
        public float locationAngle;
        public Material m_portalMat;
    }

    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_useButton;
    [SerializeField] private GameObject m_navCanvas;
    [SerializeField] private GameObject m_selectCanvas;
    [SerializeField] private Image m_crosshair;
    [SerializeField] private LevelPortal m_portal;
    [SerializeField] private CanvasGroup m_transitionGroup;

    [Header("Settings")]
    public Destination[] m_destinations;
    [SerializeField] private float m_turnSpeed = 45.0f;
    [SerializeField] private float m_targetThreshold = 10.0f;
    [SerializeField] private float m_selectSpeed = 0.5f;

    private Player_Controller playerController;
    private bool m_isActive = false;
    private float m_currentVelocity = 0.0f;

    private int m_targetIndex = -1;
    private float m_selectProgress = 0.0f;

    private bool m_selectFlag = false;
    private Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
        m_camera.enabled = false;
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_useButton.SetActive(!m_isActive);
        m_selectCanvas.SetActive(m_isActive && m_targetIndex != -1);

        m_navCanvas.SetActive(m_isActive);

        if (!m_isActive)
            return;

        if (InputManager.Instance.IsBindDown("Skill_Back"))
        {
            TriggerTransition(true);
        }

        Vector2 movementVector = GetMovementVector();

        if (Mathf.Abs(movementVector.x) > 0.0f)
        {
            m_camera.transform.Rotate(transform.up, movementVector.x * m_turnSpeed * Time.deltaTime);
            m_targetIndex = -1;
        }
        else
        {
            int closestDestinationIndex = -1;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < m_destinations.Length; i++)
            {
                if (i == 0)
                {
                    closestDistance = Mathf.DeltaAngle(m_camera.transform.eulerAngles.y, m_destinations[i].locationAngle);
                    closestDestinationIndex = 0;
                    continue;
                }

                float distance = Mathf.DeltaAngle(m_camera.transform.eulerAngles.y, m_destinations[i].locationAngle);
                if (Mathf.Abs(distance) < Mathf.Abs(closestDistance))
                {
                    closestDistance = distance;
                    closestDestinationIndex = i;
                }
            }


            if (closestDestinationIndex != -1 && Mathf.Abs(closestDistance) < m_targetThreshold)
            {
                float newRot = Mathf.SmoothDampAngle(m_camera.transform.eulerAngles.y, m_destinations[closestDestinationIndex].locationAngle, ref m_currentVelocity, 0.1f);
                m_camera.transform.eulerAngles = new Vector3(0, newRot, 0);
                m_targetIndex = closestDestinationIndex;
            }
            else
            {
                m_targetIndex = -1;
            }
        }

        if (m_targetIndex != -1 && InputManager.Instance.IsBindPressed("Interact"))
        {
            if (!m_selectFlag)
            {
                m_selectProgress += Time.deltaTime * m_selectSpeed;
                m_selectProgress = Mathf.Clamp01(m_selectProgress);

                if (m_selectProgress >= 1.0f)
                {
                    SelectDestination(m_destinations[m_targetIndex]);
                    m_selectProgress = 0.0f;
                    m_selectFlag = true;
                }
            }
        }
        else
        {
            m_selectFlag = false;
            m_selectProgress = 0.0f;
        }
        m_crosshair.fillAmount = m_selectProgress;
    }
    private void SelectDestination(Destination _destination)
    {
        // Change portal destination
        m_portal.m_portalDestination = _destination.sceneName;
        m_portal.gate.GetComponent<MeshRenderer>().material = _destination.m_portalMat;

        // Trigger animation

        // Play audio

        // etc etc
    }
    private Vector2 GetMovementVector()
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

            return movement;
        }
    }
    public void TriggerTransition(bool _closing)
    {
        if (!_closing)
        {
            playerController.ForceZoom(true);
            m_animator.SetTrigger("Fade");
        }
        else
        {
            m_animator.SetTrigger("Fade");
        }
    }

    public void ToggleNavigation()
    {
        if (m_isActive)
        {
            CloseNavigation();
        }
        else
        {
            OpenNavigation();
        }
    }

    public void OpenNavigation()
    {
        m_isActive = true;

        playerController.playerCamera.enabled = false;
        playerController.m_isDisabledInput = true;
        m_camera.enabled = true;

        if (HUDManager.Instance != null)
            HUDManager.Instance.gameObject.SetActive(false);
    }

    public void CloseNavigation()
    {
        m_isActive = false;
        playerController.ForceZoom(false);

        playerController.playerCamera.enabled = true;
        StartCoroutine(DelayControl());
        m_camera.enabled = false;

        if (HUDManager.Instance != null)
            HUDManager.Instance.gameObject.SetActive(true);
    }
    IEnumerator DelayControl()
    {
        yield return new WaitForEndOfFrame();
        playerController.m_isDisabledInput = false;
    }
}
