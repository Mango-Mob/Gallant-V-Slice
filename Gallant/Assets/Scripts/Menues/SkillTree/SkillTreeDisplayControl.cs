using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillTreeDisplayControl : MonoBehaviour
{
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

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (_instance == null)
        {
            _instance = this;
        }
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
        if (InputManager.instance.IsKeyDown(KeyType.NUM_ONE))
        {
            PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") + 1);
        }
#endif

        int gamepadID = InputManager.instance.GetAnyGamePad();

        int tabChange = (InputManager.instance.IsGamepadButtonDown(ButtonType.RB, gamepadID) ? 1 : 0) - (InputManager.instance.IsGamepadButtonDown(ButtonType.LB, gamepadID) ? 1 : 0);

        if (tabChange != 0)
        {
            int m_selectedIndex = (int)m_selectedTree + tabChange;
            m_selectedIndex = Mathf.Clamp(m_selectedIndex, 0, 3);
            m_selectedTree = (InkmanClass)m_selectedIndex;

            SelectTab(m_selectedTree);
        }

        if (eventSystem.currentSelectedGameObject != null)
        {
            SelectSkillButton(eventSystem.currentSelectedGameObject.GetComponent<SkillButton>());
        }
        else
        {
            SelectSkillButton(null);
        }

        if (InputManager.instance.IsGamepadButtonDown(ButtonType.NORTH, gamepadID))
        {
            m_selectedTreeManager.RefundTree();
        }

        m_currencyText.text = $"${PlayerPrefs.GetInt("Player Balance")}";
    }
    public void SelectSkillButton(SkillButton _button)
    {
        if (_button == null)
        {
            m_upgradeInfoObject.SetActive(false);
            return;
        }

        m_skillCost.enabled = !(_button.m_upgradeMaximum < _button.m_upgradeAmount + 1);
        m_upgradeButtonSprite.enabled = !(_button.m_upgradeMaximum < _button.m_upgradeAmount + 1);

        m_upgradeInfoObject.SetActive(true);

        m_currentlyDisplayedButton = _button;

        m_skillName.text = _button.m_skillName;
        m_skillDescription.text = _button.m_skillDescription;
        m_skillUpgradeAmount.text = $"Upgrade {_button.m_upgradeAmount}/{_button.m_upgradeMaximum}";
        m_skillCost.text = $"Upgrade Cost ${_button.m_unlockCost}";
        m_skillIcon.sprite = _button.m_icon.sprite;

        if (_button.m_upgradeMaximum < _button.m_upgradeAmount + 1)
        {
            m_skillCost.enabled = false;
        }
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
        }
    }
}
