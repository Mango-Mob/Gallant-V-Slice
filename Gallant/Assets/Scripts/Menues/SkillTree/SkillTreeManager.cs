using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InkmanClass
{
    GENERAL,
    KNIGHT,
    MAGE,
    HUNTER,
}

public class SkillTreeManager : MonoBehaviour
{
    public static GameObject m_linePrefab { get; private set; }

    public InkmanClass m_treeClass { get; private set; }
    public SkillButton m_rootSkill;
    public SkillButton[] m_buttons { get; private set; }

    private void Awake()
    {
        if (m_linePrefab == null)
            m_linePrefab = Resources.Load<GameObject>("UI/SkillTree/SkillButtonLink");

        m_buttons = GetComponentsInChildren<SkillButton>();
    }
    private void Start()
    {
        foreach (var button in m_buttons)
        {
            button.CreateDepencencyLinks();
        }
        
        foreach (var button in m_buttons)
        {
            if (button.m_skillData)
                button.SetUpgradeLevel(SkillTreeReader.instance.GetUpgradeLevel(m_treeClass, button.m_skillData.name));
            else
                Debug.Log($"{button} does not have skill data attached.");
        }
    }
    public void RefundTree()
    {
        foreach (var button in m_buttons)
        {
            button.RefundSkill();
        }

        SkillTreeReader.instance.EmptySkillTree(m_treeClass);
    }
}
