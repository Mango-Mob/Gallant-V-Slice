using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavigationTelescope : MonoBehaviour
{
    [System.Serializable]
    public struct Destination
    {
        public string levelTitle;
        public string sceneName;
        public float locationAngle;
        public int dangerLevel;
        public Color m_portalColor;
        public bool levelLocked;
    }

    [SerializeField] private int m_startingLocationIndex;
    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_useButton;
    [SerializeField] private GameObject m_navCanvas;
    [SerializeField] private GameObject m_selectCanvas;
    [SerializeField] private Image m_crosshair;
    [SerializeField] private LevelPortal m_portal;
    [SerializeField] private CanvasGroup m_transitionGroup;
    [SerializeField] private ButtonHeldCheck m_buttonHeldCheck;
    [SerializeField] private GameObject[] m_navButtonObjects;
    [SerializeField] private Image m_locked;
    [SerializeField] private Interactable m_interactable;

    [Header("Settings")]
    public Destination[] m_destinations;
    [SerializeField] private float m_turnSpeed = 45.0f;
    [SerializeField] private float m_targetThreshold = 10.0f;
    [SerializeField] private float m_selectSpeed = 0.5f;
    [SerializeField] private float m_mouseDragSpeed = 20.0f;
    public float m_angleClamp = 60.0f;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI m_levelTitleText;
    [SerializeField] private TextMeshProUGUI m_dangerLevelText;

    private Player_Controller playerController;
    [HideInInspector] public bool m_isActive = false;
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
        m_camera.transform.localRotation = Quaternion.Euler(m_camera.transform.localRotation.eulerAngles.x, 
            m_destinations[m_startingLocationIndex].locationAngle, 
            m_camera.transform.localRotation.eulerAngles.z);
        m_animator = GetComponent<Animator>();
        m_locked.enabled = false;

        GameManager.LoadSaveInfoFromFile();
        m_destinations[1].levelLocked = GameManager.m_saveInfo.m_completedSwamp == 0;
        m_destinations[2].levelLocked = GameManager.m_saveInfo.m_completedCastle == 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_useButton.SetActive(!m_isActive);
        m_selectCanvas.SetActive(m_isActive && m_targetIndex != -1);

        foreach (var navObject in m_navButtonObjects)
        {
            navObject.SetActive(m_isActive && m_targetIndex != -1 && !m_destinations[m_targetIndex].levelLocked);
        }


        if (m_selectCanvas.activeInHierarchy && m_targetIndex != -1)
        {
            m_levelTitleText.text = m_destinations[m_targetIndex].levelTitle;
            m_dangerLevelText.text = $"Danger Level: {m_destinations[m_targetIndex].dangerLevel}";
        }

        m_navCanvas.SetActive(m_isActive);

        if (!m_isActive)
            return;

        if (InputManager.Instance.IsBindDown("Skill_Back"))
        {
            TriggerTransition(true);
        }

        Vector2 movementVector = GetMovementVector();
        if (InputManager.Instance.GetMousePress(MouseButton.LEFT))
        {
            Vector2 mouseDelta = InputManager.Instance.GetMouseDelta() * m_mouseDragSpeed;
            movementVector.x += mouseDelta.x;
        }

        if (Mathf.Abs(movementVector.x) > 0.0f)
        {
            m_camera.transform.Rotate(transform.up, movementVector.x * m_turnSpeed * Time.deltaTime);
            
            m_camera.transform.localRotation = Quaternion.Euler(m_camera.transform.localRotation.eulerAngles.x,
                Mathf.Clamp(m_camera.transform.localRotation.eulerAngles.y, 180.0f + -m_angleClamp, 180.0f + m_angleClamp),
                m_camera.transform.localRotation.eulerAngles.z);

            m_targetIndex = -1;
        }
        else if (!InputManager.Instance.GetMousePress(MouseButton.LEFT))
        {
            int closestDestinationIndex = -1;
            float closestDistance = Mathf.Infinity;

            for (int i = 0; i < m_destinations.Length; i++)
            {
                if (i == 0)
                {
                    closestDistance = Mathf.DeltaAngle(m_camera.transform.localEulerAngles.y, m_destinations[i].locationAngle);
                    closestDestinationIndex = 0;
                    continue;
                }

                float distance = Mathf.DeltaAngle(m_camera.transform.localEulerAngles.y, m_destinations[i].locationAngle);
                if (Mathf.Abs(distance) < Mathf.Abs(closestDistance))
                {
                    closestDistance = distance;
                    closestDestinationIndex = i;
                }
            }


            if (closestDestinationIndex != -1 && Mathf.Abs(closestDistance) < m_targetThreshold)
            {
                float newRot = Mathf.SmoothDampAngle(m_camera.transform.localEulerAngles.y, m_destinations[closestDestinationIndex].locationAngle, ref m_currentVelocity, 0.1f);
                m_camera.transform.localEulerAngles = new Vector3(m_camera.transform.localRotation.eulerAngles.x, newRot, m_camera.transform.localRotation.eulerAngles.z);
                m_targetIndex = closestDestinationIndex;

                m_locked.enabled = m_destinations[m_targetIndex].levelLocked;
            }
            else
            {
                m_targetIndex = -1;
                m_locked.enabled = false;
            }
        }

        if (m_targetIndex != -1 && !m_destinations[m_targetIndex].levelLocked && (InputManager.Instance.IsBindPressed("Interact") || m_buttonHeldCheck.m_isButtonHeld))
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

                    TriggerTransition(true);
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
        //m_portal.m_portalDestination = _destination.sceneName;
        //m_portal.SetColor(_destination.m_portalColor);

        NavigationManager.Instance.Clear(true);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();

        LevelManager.Instance.LoadNewLevel(_destination.sceneName);
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
        m_interactable.m_usable = _closing;
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
