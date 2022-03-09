using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillButton : MonoBehaviour, IPointerEnterHandler
{
    [Header("Skill Information")]
    public SkillData m_skillData;
    public Image m_icon;

    private SkillTreeManager m_manager;

    [Header("Unlock Information")]
    [SerializeField] private List<SkillButton> m_unlockDependencies = new List<SkillButton>();
    private List<SkillButtonLink> m_dependencyLink = new List<SkillButtonLink>();

    public int m_upgradeAmount {get; private set; } = 0;
    [SerializeField] private TextMeshProUGUI m_upgradeNumberText;
    [SerializeField] private Image[] m_availabilityImages;

    [Header("Line Anchor Positions")]
    [SerializeField] private Transform m_lineEnterance;
    [SerializeField] private Transform m_lineExit;

    [Header("Components")]
    private Button m_button;
    private CanvasGroup m_canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        m_manager = GetComponentInParent<SkillTreeManager>();
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_button = GetComponent<Button>();

        m_icon.sprite = m_skillData.skillIcon;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsUnlockable())
        {
            foreach (var image in m_availabilityImages)
            {
                image.color = new Color(1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            foreach (var image in m_availabilityImages)
            {
                image.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }

        m_canvasGroup.alpha = IsAvailable() ? 1.0f : 0.0f;
        m_button.enabled = IsAvailable();
        foreach (var link in m_dependencyLink)
        {
            link.ToggleAvailability(IsAvailable());
        }
        

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InputManager.Instance.isInGamepadMode)
        {
            SelectSkill();
        }
    }

    public void SelectSkill()
    {
        SkillTreeDisplayControl._instance.SelectSkillButton(this);
    }
    public void PurchaseSkill()
    {
        if (IsUnlockable()) 
        {
            m_upgradeAmount++;
            m_upgradeNumberText.text = m_upgradeAmount.ToString();
            PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") - m_skillData.upgradeCost);
            
            SkillTreeReader.instance.UnlockSkill(m_manager.m_treeClass, m_skillData.name);
        }
    }
    public void RefundSkill()
    {
        PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") + m_upgradeAmount * m_skillData.upgradeCost);
        m_upgradeAmount = 0;
        m_upgradeNumberText.text = m_upgradeAmount.ToString();
    }

    public bool IsUnlockable()
    {
        if (PlayerPrefs.GetInt("Player Balance") < m_skillData.upgradeCost || m_skillData.upgradeMaximum < m_upgradeAmount + 1)
        {
            return false;
        }

        if (m_unlockDependencies.Count == 0)
            return true;

        bool unlockable = false;
        foreach (var item in m_unlockDependencies)
        {
            m_dependencyLink[m_unlockDependencies.IndexOf(item)].ToggleActive(item.m_upgradeAmount > 0);
            if (item.m_upgradeAmount > 0)
            {
                unlockable = true;
            }
        }

        return unlockable;
    }
    public bool IsAvailable()
    {
        if (m_unlockDependencies.Count == 0)
            return true;

        bool unlockable = false;
        foreach (var item in m_unlockDependencies)
        {
            m_dependencyLink[m_unlockDependencies.IndexOf(item)].ToggleActive(item.m_upgradeAmount > 0);
            if (item.m_upgradeAmount > 0)
            {
                unlockable = true;
            }
        }

        return unlockable;
    }
    public void CreateDepencencyLinks()
    {
        foreach (var dependency in m_unlockDependencies)
        {
            GameObject newObject = Instantiate(SkillTreeManager.m_linePrefab, transform.position, Quaternion.identity, SkillTreeDisplayControl._instance.m_lineCanvas.transform);
            //newObject.transform.SetParent(SkillTreeDisplayControl._instance.m_lineCanvas.transform);
            //newObject.transform.SetAsFirstSibling();
            //newObject.transform.localScale = new Vector3(1, 1.0f * (dependency.m_lineExit.position - m_lineEnterance.position).magnitude, 1);
            //newObject.GetComponent<RectTransform>().sizeDelta = new Vector2(6, (dependency.m_lineExit.position - m_lineEnterance.position).magnitude * 2.0f);

            newObject.transform.position = (m_lineEnterance.position + dependency.m_lineExit.position) / 2 ;

            Vector3 difference = m_lineEnterance.position - dependency.m_lineExit.position;
            newObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan(difference.y / difference.x) + 90.0f);

            m_dependencyLink.Add(newObject.GetComponent<SkillButtonLink>());
            
           newObject.GetComponent<LineRenderer>().SetPosition(0, m_lineEnterance.position + Vector3.forward * 5.0f);
           newObject.GetComponent<LineRenderer>().SetPosition(1, dependency.m_lineExit.position + Vector3.forward * 5.0f);
        }
    }
    public void SetUpgradeLevel(int _level)
    {
        if (_level == -1)
        {
            Debug.Log("Skill ID not found in tree");
            return;
        }

        m_upgradeAmount = _level;
        m_upgradeNumberText.text = m_upgradeAmount.ToString();
    }
}
