using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [Header("Skill Information")]
    public string m_skillName;
    public string m_skillDescription;
    public string m_skillID;
    public Image m_icon;

    private SkillTreeManager m_manager;

    [Header("Unlock Information")]
    [SerializeField] private SkillButton[] m_unlockDependencies;
    public int m_unlockCost = 1;
    public int m_upgradeMaximum = 1;
    public int m_upgradeAmount {get; private set; } = 0;
    [SerializeField] private TextMeshProUGUI m_upgradeNumberText;
    
    [Header("Line Anchor Positions")]
    [SerializeField] private Transform m_lineEnterance;
    [SerializeField] private Transform m_lineExit;

    // Start is called before the first frame update
    void Start()
    {
        m_manager = GetComponentInParent<SkillTreeManager>();
    }

    // Update is called once per frame
    void Update()
    {
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
            PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") - m_unlockCost);
        }
    }

    public bool IsUnlockable()
    {
        if (PlayerPrefs.GetInt("Player Balance") < m_unlockCost || m_upgradeMaximum < m_upgradeAmount + 1)
        {
            return false;
        }

        if (m_unlockDependencies.Length == 0)
            return true;

        foreach (var item in m_unlockDependencies)
        {
            if (item.m_upgradeAmount > 0)
            {
                return true;
            }
        }

        return false;
    }
}
