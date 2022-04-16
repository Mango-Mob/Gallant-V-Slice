using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillTreeDisplayControl : MonoBehaviour
{
    public Canvas m_lineCanvas;
    public Camera m_skillTreeCamera;
    public static SkillTreeDisplayControl _instance { get; private set; }

    [SerializeField] private InkmanClass m_selectedTree;
    private SkillTreeManager m_selectedTreeManager;
    private List<GameObject> m_treeList = new List<GameObject>();
    private List<SkillTreeTabButton> m_tabButtonList = new List<SkillTreeTabButton>();
    private SkillButton m_currentlyDisplayedButton;

    [Header("Upgrade Info Display")]
    [SerializeField] private GameObject m_upgradeInfoObject;
    [SerializeField] private TextMeshProUGUI m_currencyText;

    [SerializeField] private TextMeshProUGUI m_skillName;
    [SerializeField] private TextMeshProUGUI m_skillDescription;
    [SerializeField] private TextMeshProUGUI m_skillUpgradeAmount;
    [SerializeField] private TextMeshProUGUI m_skillCost;
    [SerializeField] private Image m_skillIcon;
    [SerializeField] private Image m_upgradeButtonSprite;

    [Header("General")]
    [SerializeField] private SkillTreeTabButton m_generalTab;
    [SerializeField] private GameObject m_generalTree;

    [Header("Knight")]
    [SerializeField] private SkillTreeTabButton m_knightTab;
    [SerializeField] private GameObject m_knightTree;

    [Header("Mage")]
    [SerializeField] private SkillTreeTabButton m_mageTab;
    [SerializeField] private GameObject m_mageTree;

    [Header("Hunter")]
    [SerializeField] private SkillTreeTabButton m_hunterTab;
    [SerializeField] private GameObject m_hunterTree;

    private EventSystem eventSystem;
    private Vector3 m_canvasStartPos;

    [Header("Navigation Values")]
    public float m_maxZoom = 100.0f;
    public float m_minZoom = 30.0f;
    public float m_boundsDistance = 200.0f;
    public float m_allowedLockonDistance = 5.0f;
    public float m_controllerMoveSpeed = 100.0f;
    public float m_controllerZoomSpeed = 1.0f;
    public float m_mouseDragSpeed = 0.1f;
    public float m_mouseZoomSpeed = 1.0f;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (_instance == null)
        {
            _instance = this;
        }
        m_canvasStartPos = transform.position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_tabButtonList.Add(m_generalTab);
        m_tabButtonList.Add(m_knightTab);
        m_tabButtonList.Add(m_mageTab);
        m_tabButtonList.Add(m_hunterTab);

        m_treeList.Add(m_generalTree);
        m_treeList.Add(m_knightTree);
        m_treeList.Add(m_mageTree);
        m_treeList.Add(m_hunterTree);

        SelectTab(m_selectedTree);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug Thing

#if UNITY_EDITOR
        if (InputManager.Instance.IsKeyDown(KeyType.NUM_ONE))
        {
            PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") + 1000);
        }
#endif

        int gamepadID = InputManager.Instance.GetAnyGamePad();

        int tabChange = (InputManager.Instance.IsGamepadButtonDown(ButtonType.RB, gamepadID) ? 1 : 0) - (InputManager.Instance.IsGamepadButtonDown(ButtonType.LB, gamepadID) ? 1 : 0);

        // REMOVE AFTER ALPHA
        tabChange = 0;

        if (tabChange != 0)
        {
            int m_selectedIndex = (int)m_selectedTree + tabChange;
            m_selectedIndex = Mathf.Clamp(m_selectedIndex, 0, 3);
            m_selectedTree = (InkmanClass)m_selectedIndex;

            SelectTab(m_selectedTree);
        }

        if (InputManager.Instance.isInGamepadMode) // Using Gamepad
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                SelectSkillButton(eventSystem.currentSelectedGameObject.GetComponent<SkillButton>());
            }
            else
            {
                SelectSkillButton(null);
            }
            if (m_currentlyDisplayedButton == null && eventSystem.currentSelectedGameObject?.name != "BackButton")
            {
                SelectSkillButton(m_selectedTreeManager.m_rootSkill);
            }
            //else
            //{
            //    SelectSkillButton(m_currentlyDisplayedButton);
            //}
        }

#if UNITY_EDITOR
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.NORTH, gamepadID))
        {
            m_selectedTreeManager.RefundTree();
            SelectSkillButton(m_selectedTreeManager.m_rootSkill);
        }
#endif

        m_currencyText.text = $"{PlayerPrefs.GetInt("Player Balance")}";

        Navigation(gamepadID);
    }

    public void Navigation(int _gamepadID)
    {
        if (InputManager.Instance.isInGamepadMode)
        {
            float zoom = InputManager.Instance.GetGamepadStick(StickType.RIGHT, _gamepadID).y;
            ScaleAround(transform, m_currentlyDisplayedButton.transform, transform.localScale + Vector3.one * zoom * m_controllerZoomSpeed * Time.deltaTime);
            //transform.localScale += Vector3.one * zoom * m_controllerZoomSpeed * Time.deltaTime;

            if (m_currentlyDisplayedButton != null)
            {
                Vector3 direction = (m_canvasStartPos - m_currentlyDisplayedButton.transform.position);
                float distance = direction.magnitude;

                if (distance > m_allowedLockonDistance)
                {
                    direction.Normalize();
                    transform.position += new Vector3(direction.x, direction.y, 0) * Time.deltaTime * m_controllerMoveSpeed;
                }
            }
        }
        else
        {
            ScaleAround(transform, m_currentlyDisplayedButton.transform, transform.localScale + Vector3.one * InputManager.Instance.GetMouseScrollDelta() * m_mouseZoomSpeed);

            //transform.localScale += Vector3.one * InputManager.Instance.GetMouseScrollDelta() * m_mouseZoomSpeed;
            if (InputManager.Instance.GetMousePress(MouseButton.LEFT))
            {
                Vector2 mouseDelta = InputManager.Instance.GetMouseDelta() * m_mouseDragSpeed;
                transform.position += new Vector3(mouseDelta.x, mouseDelta.y, 0);

            }
        }
        AfterNavigationUpdate();
    }
    private void ScaleAround(Transform target, Transform pivot, Vector3 scale)
    {
        Transform pivotParent = pivot.parent;
        Vector3 pivotPos = pivot.position;
        pivot.SetParent(target);
        target.localScale = scale;
        target.localScale = Vector3.one * Mathf.Clamp(target.localScale.x, m_minZoom, m_maxZoom);
        target.position += pivotPos - pivot.position;
        pivot.SetParent(pivotParent);
    }

    private void AfterNavigationUpdate()
    {
        float boundDist = m_boundsDistance * transform.localScale.x;

        // x bounds
        if (transform.position.x > m_canvasStartPos.x + boundDist)
            transform.position = new Vector3(m_canvasStartPos.x + boundDist, transform.position.y, transform.position.z);
        if (transform.position.x < m_canvasStartPos.x - boundDist)
            transform.position = new Vector3(m_canvasStartPos.x - boundDist, transform.position.y, transform.position.z);

        // y bounds
        if (transform.position.y > m_canvasStartPos.y + boundDist)
            transform.position = new Vector3(transform.position.x, m_canvasStartPos.y + boundDist, transform.position.z);
        if (transform.position.y < m_canvasStartPos.y - boundDist)
            transform.position = new Vector3(transform.position.x, m_canvasStartPos.y - boundDist, transform.position.z);

        foreach (var button in m_selectedTreeManager.m_buttons)
        {
            button.UpdateLinkPosition();
        }
    }
    public void SelectSkillButton(SkillButton _button)
    {
        if (m_currentlyDisplayedButton)
            m_currentlyDisplayedButton.ToggleTooltip(false);

        if (_button == null)
        {
            m_currentlyDisplayedButton = null;
            m_upgradeInfoObject.SetActive(false);
            return;
        }

        if (!_button.IsAvailable())
            return;

        m_currentlyDisplayedButton = _button;
        m_currentlyDisplayedButton.ToggleTooltip(true);

        //Debug.Log($"{_button.m_skillData.skillName} button");

        //m_skillCost.enabled = !(_button.m_skillData.upgradeMaximum < _button.m_upgradeAmount + 1);
        //m_upgradeButtonSprite.enabled = !(_button.m_skillData.upgradeMaximum < _button.m_upgradeAmount + 1);

        //m_upgradeInfoObject.SetActive(true);

        //m_currentlyDisplayedButton = _button;

        //m_skillName.text = _button.m_skillData.skillName;
        //m_skillDescription.text = SkillData.EvaluateDescription(_button.m_skillData);
        //m_skillUpgradeAmount.text = $"Upgrade {_button.m_upgradeAmount}/{_button.m_skillData.upgradeMaximum}";
        //m_skillCost.text = $"Upgrade Cost: {_button.m_skillData.upgradeCost}";
        //m_skillIcon.sprite = _button.m_icon.sprite;

        //if (_button.m_skillData.upgradeMaximum < _button.m_upgradeAmount + 1)
        //{
        //    m_skillCost.enabled = false;
        //}


        eventSystem.SetSelectedGameObject(_button.gameObject);
    }

    public void SelectTab(InkmanClass _class)
    {
        SkillTreeTabButton thisTab = null;
        GameObject thisTree = null;

        foreach (var tree in m_treeList)
        {
            tree.SetActive(false);
        }
        foreach (var tabButton in m_tabButtonList)
        {
            tabButton.ToggleSelected(false);
        }

        switch (_class)
        {
            case InkmanClass.GENERAL:
                thisTab = m_generalTab;
                thisTree = m_generalTree;
                break;
            case InkmanClass.KNIGHT:
                thisTab = m_knightTab;
                thisTree = m_knightTree;
                break;
            case InkmanClass.MAGE:
                thisTab = m_mageTab;
                thisTree = m_mageTree;
                break;
            case InkmanClass.HUNTER:
                thisTab = m_hunterTab;
                thisTree = m_hunterTree;
                break;
            default:
                return;
        }

        thisTree.SetActive(true);
        thisTab.ToggleSelected(true);

        m_selectedTreeManager = thisTree.GetComponent<SkillTreeManager>();
        if (m_selectedTreeManager.m_rootSkill != null)
        {
            eventSystem.SetSelectedGameObject(thisTree.GetComponent<SkillTreeManager>().m_rootSkill.gameObject);
            SelectSkillButton(thisTree.GetComponent<SkillTreeManager>().m_rootSkill);
        }
    }
}
