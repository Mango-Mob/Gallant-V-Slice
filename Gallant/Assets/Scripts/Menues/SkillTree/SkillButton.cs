using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.Experimental.SceneManagement;

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

    [SerializeField] private bool m_permaLocked = false;

    [Header("Tooltip")]
    [SerializeField] private GameObject m_tooltipObject;
    [SerializeField] private TextMeshProUGUI m_tooltipName;
    [SerializeField] private TextMeshProUGUI m_tooltipDesc;
    [SerializeField] private TextMeshProUGUI m_tooltipCost;
    [SerializeField] private TextMeshProUGUI m_tooltipCurrentLevel;
    private bool m_tooltipsActive = false;
    private float m_tooltipLerp = 0.0f;
    [SerializeField] private float m_tooltipLerpSpeed = 4.0f;

    private bool m_hiddenMode = false;

    // Start is called before the first frame update
    void Start()
    {
        m_manager = GetComponentInParent<SkillTreeManager>();
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_button = GetComponent<Button>();

        m_icon.sprite = m_skillData.skillIcon;
        m_tooltipObject.SetActive(true);
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

        m_canvasGroup.alpha = (IsAvailable()) ? 1.0f : 0.0f;
        m_button.enabled = IsAvailable();
        foreach (var link in m_dependencyLink)
        {
            link.ToggleAvailability((IsAvailable()) && link.m_dependency.IsAvailable());
        }

        if (m_hiddenMode)
        {
            m_upgradeNumberText.text = "?";
            m_icon.color = Color.black;

            m_tooltipName.text = "???";
            m_tooltipDesc.text = "???";

            m_tooltipCost.gameObject.SetActive(true);
            m_tooltipCost.text = "???";
        }
        else
        {
            m_upgradeNumberText.text = m_upgradeAmount.ToString();
            m_icon.color = Color.white;

            // Tooltip update
            m_tooltipName.text = $"{m_skillData.skillName} {m_upgradeAmount}/{m_skillData.upgradeMaximum}";
            m_tooltipDesc.text = SkillData.EvaluateDescription(m_skillData);

            if (m_upgradeAmount < m_skillData.upgradeMaximum)
            {
                m_tooltipCost.text = $"{GetCurrentCost()}";
                m_tooltipCost.gameObject.SetActive(true);
            }
            else
            {
                m_tooltipCost.gameObject.SetActive(false);
            }
        }
        m_tooltipLerp = Mathf.Clamp01(m_tooltipLerp + (m_tooltipsActive ? 1.0f : -1.0f) * Time.deltaTime * m_tooltipLerpSpeed);
        m_tooltipObject.transform.localScale = new Vector3(m_tooltipLerp, m_tooltipObject.transform.localScale.y, m_tooltipObject.transform.localScale.z);
    }
    private int GetCurrentCost()
    {
        return (int)(m_skillData.upgradeCost * (m_upgradeAmount > 0 ? m_upgradeAmount * m_skillData.upgradeMultiplier : 1.0f));
    }
    public void ToggleTooltip(bool _active)
    {
        m_tooltipsActive = _active;
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
            PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") - GetCurrentCost());
            m_upgradeAmount++;
            m_upgradeNumberText.text = m_upgradeAmount.ToString();

            SkillTreeReader.instance.UnlockSkill(m_manager.m_treeClass, m_skillData.name);
        }
        SelectSkill();
    }
    public void RefundSkill()
    {
        while (m_upgradeAmount > 0)
        {
            m_upgradeAmount--;
            PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") + GetCurrentCost());
        }
        m_upgradeAmount = 0;
        m_upgradeNumberText.text = m_upgradeAmount.ToString();
    }

    public bool IsUnlockable()
    {
        if (PlayerPrefs.GetInt("Player Balance") < GetCurrentCost() || m_skillData.upgradeMaximum < m_upgradeAmount + 1)
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
        if (m_permaLocked)
            return false;

        // Hidden check
        m_hiddenMode = false;
        foreach (var dependency in m_unlockDependencies)
        {
            if (dependency.m_unlockDependencies.Count == 0)
                m_hiddenMode = true;

            foreach (var item in dependency.m_unlockDependencies)
            {
                if (item.m_upgradeAmount > 0)
                {
                    m_hiddenMode = true;
                    break;
                }
            }
            if (m_hiddenMode)
                break;
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

        if (!unlockable && m_hiddenMode)
            return true;
        else
            m_hiddenMode = false;

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

            SkillButtonLink linkScript = newObject.GetComponent<SkillButtonLink>();
            m_dependencyLink.Add(linkScript);
            linkScript.SetPoints(m_lineEnterance, dependency.m_lineExit);
            linkScript.m_dependency = dependency;
           //newObject.GetComponent<LineRenderer>().SetPosition(0, m_lineEnterance.position + Vector3.forward * 20.0f);
           //newObject.GetComponent<LineRenderer>().SetPosition(1, dependency.m_lineExit.position + Vector3.forward * 20.0f);
        }
        UpdateLinkPosition();
    }
    public void UpdateLinkPosition()
    {
        foreach (var dependency in m_dependencyLink)
        {
            dependency.UpdatePositions();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var other in m_unlockDependencies)
        {
            Gizmos.DrawLine(transform.position, other.transform.position);
        }

#if UNITY_EDITOR
        Handles.color = Color.white;
        Handles.Label(transform.position, m_skillData ? m_skillData.skillName : "EMPTY");

        if (PrefabStageUtility.GetCurrentPrefabStage() == null || PrefabStageUtility.GetCurrentPrefabStage().scene.name != "SkillButton")
        {
            gameObject.name = m_skillData ? m_skillData.name : "emptyButton";
        }
#endif
    }
}
