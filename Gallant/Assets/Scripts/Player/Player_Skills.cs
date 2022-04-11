using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Skills : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    SkillTreeReader reader;

    public float m_healthIncrease = 0.0f;
    public float m_moveSpeedIncrease = 0.0f;
    public float m_healMoveSpeedIncrease = 0.0f;
    public float m_physicalDefenceIncrease = 0.0f;
    public float m_magicalDefenceIncrease = 0.0f;
    public float m_outOfCombatSpeedIncrease = 0.0f;
    public float m_healPowerIncrease = 0.0f;
    public float m_rollDistanceIncrease = 0.0f;
    public float m_staminaRegenRate = 0.0f;
    public float m_stunDecrease = 0.0f;
    public int m_experienceBonus = 0;
    public int m_extraHealOrbs = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        reader = SkillTreeReader.instance;

        playerController = GetComponent<Player_Controller>();

        EvaluateSkills();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EvaluateSkills()
    {
        m_healthIncrease = 0.0f;
        m_moveSpeedIncrease = 1.0f;
        m_healMoveSpeedIncrease = 1.0f;
        m_physicalDefenceIncrease = 0.0f;
        m_magicalDefenceIncrease = 0.0f;
        m_outOfCombatSpeedIncrease = 1.0f;
        m_healPowerIncrease = 0.0f;
        m_rollDistanceIncrease = 1.0f;
        m_staminaRegenRate = 1.0f;
        m_stunDecrease = 0.0f;
        m_experienceBonus = 0;
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
        string skillID;
        if (_skill.id.IndexOfAny(numbers) != -1)
            skillID = _skill.id.Substring(0, _skill.id.IndexOfAny(numbers));
        else
            skillID = _skill.id;

        switch (skillID)
        {
            case "abilDefence": // Added 
                m_magicalDefenceIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "defenceIncrease": // Added
                m_physicalDefenceIncrease += skillData.effectStrength * _skill.upgradeLevel;
                m_magicalDefenceIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "experienceBonus": // Added
                m_experienceBonus += (int)skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "healChargeIncrease": // Added
                m_extraHealOrbs = (int)skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "healPowerIncrease": // Added
                m_healPowerIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "hpIncrease": // Added
                m_healthIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "moveSpeedIncrease": // Added
                m_moveSpeedIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "outOfCombatSpeedIncrease": // Added
                m_outOfCombatSpeedIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "phyDefence": // Added
                m_physicalDefenceIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "rollDistanceIncrease": // Added
                m_rollDistanceIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "staminaRegenRate": // Added
                m_staminaRegenRate += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "stunDecrease": // Added
                m_stunDecrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            case "healMoveSpeed": // Added
                m_healMoveSpeedIncrease += skillData.effectStrength * _skill.upgradeLevel;
                break;
            default:
                Debug.LogWarning($"No case for the skill named {skillID}.");
                break;
        }
    }

    public void ActivateSkills(List<AbilityTag> _abilityTags, Actor _actor, float _potency, bool _killedTarget = false)
    {
        if (_abilityTags == null)
            return;

        foreach (var tag in _abilityTags)
        {
            OnHitEffectSkills(tag, _actor, _potency);
            if (!_killedTarget)
                OnKillEffectSkills(tag, _actor, _potency);
        }
    }
    private void OnHitEffectSkills(AbilityTag _tag, Actor _actor, float _potency)
    {
        switch (_tag)
        {
            case AbilityTag.Fire:
                // Splash damage to nearby enemies
                break;
            case AbilityTag.Air:
                // Increase attack speed on hit
                break;
            case AbilityTag.Earth:
                // Splash impact damage to nearby enemies
                break;
            case AbilityTag.Water:
                // Heal regen on hit - doesn't stack (about 1% a second)
                break;
        }
    }
    private void OnKillEffectSkills(AbilityTag _tag, Actor _actor, float _potency)
    {
        switch (_tag)
        {
            case AbilityTag.Fire:
                // Patch of fire where enemy dies
                break;
            case AbilityTag.Air:
                // Bonus movement speed on kills
                break;
            case AbilityTag.Earth:
                // Knockback nearby enemies on kill
                break;
            case AbilityTag.Water:
                // Slow patch on kill
                break;
        }
    }
}
