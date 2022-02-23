using ActorSystem.AI;
using ActorSystem.AI.Components;
//using SOHNE.Accessibility.Colorblindness;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugManager : SingletonPersistent<DebugManager>
{
    public LayerMask m_raycastLayers;
    [SerializeField] private GameObject m_contentParent;
    [SerializeField] private Image m_backgroundImage;

    [SerializeField] private GameObject[] m_panelArray;
    [SerializeField] private Button[] m_buttonArray;

    [Header("Actor Content")]
    [SerializeField] private Text m_ActorCountTxt;
    [SerializeField] private Button m_killAllBtn;
    [SerializeField] private Text m_selelectedName;
    [SerializeField] private Button[] m_toggleSelectedButtons;
    [SerializeField] private Button m_killOneBtn;

    [Header("Scene Content")]
    [SerializeField] private Dropdown m_sceneList;
    [SerializeField] private Dropdown m_colorBlindList;
    [SerializeField] private Button[] m_toggleCamButtons;
    [SerializeField] private SimpleCameraController m_freeCamera;
    [SerializeField] private Slider m_timeSlider;
    [SerializeField] private Toggle m_timeCheck;
    [SerializeField] private Toggle m_HudCheck;

    private Player_Controller m_player;
    private Camera m_mainCamera;

    private GameObject m_selected;
    private void Awake()
    {
        m_player = FindObjectOfType<Player_Controller>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_sceneList.ClearOptions();
        List<string> options = new List<string>();
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int start = path.LastIndexOf('/');
            int end = path.LastIndexOf('.');
            options.Add(path.Substring(start + 1, end - start - 1));
        }
        m_sceneList.AddOptions(options);
        m_selelectedName.text = (m_selected != null) ? $"\"{m_selected.name}\"" : "null";

        m_toggleSelectedButtons[0].interactable = false;
        m_toggleSelectedButtons[1].interactable = false;
        m_killOneBtn.interactable = false;
        OnLevelLoad();
    }

    private void OnLevelLoad()
    {
        m_freeCamera.gameObject.SetActive(false);
        m_toggleCamButtons[0].interactable = (m_player != null);
        m_toggleCamButtons[1].interactable = false;

        m_mainCamera = (m_player != null) ? m_player.GetComponentInChildren<Camera>() : Camera.main;
        m_timeSlider.value = 1.0f;
        Time.timeScale = 1.0f;
        m_freeCamera.SetTarget(m_mainCamera.transform);
    }

    private void OnLevelWasLoaded(int level)
    {
        m_player = FindObjectOfType<Player_Controller>();
        OnLevelLoad();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.IsKeyDown(KeyType.TILDE))
        {
            GetComponent<Animator>().SetTrigger("StateUpdate");
        }
        if (m_contentParent.activeInHierarchy)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = m_timeSlider.value;
            m_freeCamera.enabled = false;

            if (InputManager.Instance.IsMouseButtonDown(MouseButton.LEFT))
            {
                SelectedUpdate();
            }
        }
        else if(m_freeCamera.gameObject.activeInHierarchy)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            m_freeCamera.enabled = true;
        }

        if(!m_timeCheck.isOn)
        {
            m_timeSlider.value = 1.0f;
        }
        m_timeSlider.interactable = m_timeCheck.isOn;

        int count = ActorManager.Instance.m_subscribed.Count;
        m_ActorCountTxt.text = count.ToString();
        m_killAllBtn.interactable = count > 0;

        HUDManager.Instance.gameObject.SetActive(!m_HudCheck.isOn);
    }

    private void SelectedUpdate()
    {
        if (!IsMouseOnWindow())
        {
            Ray ray;

            if (m_freeCamera.gameObject.activeInHierarchy)
                ray = m_freeCamera.GetComponent<Camera>().ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
            else
                ray = m_mainCamera.ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1 << m_raycastLayers))
            {
                m_selected = hit.collider.gameObject;
            }

            m_selelectedName.text = (m_selected != null) ? $"\"{m_selected.name}\"" : "null";

            m_toggleSelectedButtons[0].interactable = !m_selected.activeInHierarchy;
            m_toggleSelectedButtons[1].interactable = m_selected.activeInHierarchy;
            m_killOneBtn.interactable = m_selected.GetComponentInParent<Actor>() != null;
        }
    }

    private bool IsMouseOnWindow()
    {
        Vector2 localPoint = m_backgroundImage.rectTransform.InverseTransformPoint(InputManager.Instance.GetMousePositionInScreen());
        if (m_backgroundImage.rectTransform.rect.Contains(localPoint))
        {
            return true;
        }
        return false;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(m_sceneList.value);
    }

    public void ShowPanel(int index)
    {
        if (m_panelArray.Length > 0 && m_buttonArray.Length > 0)
        {
            for (int i = 0; i < m_panelArray.Length; i++)
            {
                if (i == index)
                {
                    m_panelArray[i].SetActive(true);
                    EventSystem.current.SetSelectedGameObject(m_buttonArray[Mathf.Min(i, m_buttonArray.Length - 1)].gameObject);
                    continue;
                }
                else
                {
                    m_panelArray[i].SetActive(false);
                }
            }
        }
    }

    public void ToggleFreeCamera(bool status)
    {
        m_freeCamera.gameObject.SetActive(status);
        m_player.m_isDisabledInput = status;

        m_toggleCamButtons[0].interactable = !status;
        m_toggleCamButtons[1].interactable = status;
    }

    public void ColorBlindChange()
    {
        //GetComponent<Colorblindness>().Change(m_colorBlindList.value);
    }

    public void KillAllEnemies()
    {
        ActorManager.Instance.KillAll();
    }
    public void KillSelected()
    {
        ActorManager.Instance.Kill(m_selected.GetComponentInParent<ActorSystem.AI.Actor>());
        m_selected = null;
        m_toggleSelectedButtons[0].interactable = false;
        m_toggleSelectedButtons[1].interactable = false;
        m_killOneBtn.interactable = false;
    }

    public void ToggleSelected(bool status)
    {
        m_selected.SetActive(status);
        m_toggleSelectedButtons[0].interactable = !status;
        m_toggleSelectedButtons[1].interactable = status;
    }
}
