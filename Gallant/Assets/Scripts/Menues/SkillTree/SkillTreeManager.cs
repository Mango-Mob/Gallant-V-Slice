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

    [SerializeField] private InkmanClass m_treeClass;
    public SkillButton m_rootSkill;
    private SkillButton[] m_buttons;

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
    }
}
