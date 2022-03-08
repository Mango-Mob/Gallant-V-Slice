using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Skills : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    SkillTreeReader reader;

    private float m_healthMult = 1.0f;
    private float m_healthIncrease = 1.0f;
    private float m_attackSpeedMult = 1.0f;
    private float m_damageMult = 1.0f;
    private float m_moveSpeedMult = 1.0f;
    private float m_defenceMult = 1.0f;
    private float m_attackMoveMult = 1.0f;
    private float m_cooldownMult = 1.0f;
    private int m_extraHealOrbs = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        reader = SkillTreeReader.instance;

        playerController = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EvaluateSkills()
    {
        m_healthMult = 1.0f;
        m_healthIncrease = 1.0f;
        m_attackSpeedMult = 1.0f;
        m_damageMult = 1.0f;
        m_moveSpeedMult = 1.0f;
        m_defenceMult = 1.0f;
        m_attackMoveMult = 1.0f;
        m_cooldownMult = 1.0f;
        m_extraHealOrbs = 0;

        foreach (var skill in SkillTreeReader.instance.GetSkillTree(InkmanClass.GENERAL).skills)
        {
            CalculateSkillEffect(skill);
        }

        if (playerController.m_inkmanClass != null && playerController.m_inkmanClass.inkmanClass != InkmanClass.GENERAL)
        {
            foreach (var skill in SkillTreeReader.instance.GetSkillTree(playerController.m_inkmanClass.inkmanClass).skills)
            {
                CalculateSkillEffect(skill);
            }
        }

        playerController.playerResources.m_startingAdrenaline = playerController.playerResources.m_defaultAdrenaline + m_extraHealOrbs;
        playerController.playerResources.ResetResources();
    }

    private void CalculateSkillEffect(Skill _skill)
    {
        string path = "Data/Skills/" + _skill.id;
        SkillData skillData = Resources.Load<SkillData>(path);

        if (skillData == null)
        {
            Debug.LogWarning($"{_skill} data not found.");
            return;
        }

        char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        string skillID = _skill.id.Substring(0, _skill.id.IndexOfAny(numbers));

        switch (skillID)
        {
            case "hpIncrease":
                m_healthMult += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "defenseIncrease":
                m_defenceMult += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "healChargeIncrease":
                m_extraHealOrbs = (int)skillData.effectStrength * _skill.upgradeLevel;
                break;
            default:
                Debug.LogWarning($"No case for the skill named {_skill}.");
                break;
        }
    }

}
