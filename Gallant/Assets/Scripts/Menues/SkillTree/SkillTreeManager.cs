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
    [SerializeField] private InkmanClass m_treeClass;
    public SkillButton[] m_buttons;

    private void Awake()
    {
        m_buttons = GetComponentsInChildren<SkillButton>();
    }
}
