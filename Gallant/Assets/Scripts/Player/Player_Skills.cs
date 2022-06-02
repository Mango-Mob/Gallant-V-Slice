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

    [Header("Skill Tree Ability")]
    private SkillAbility m_attackSpeedStatus;
    public float m_attackSpeedStatusBonus = 1.0f;

    private SkillAbility m_movementSpeedStatus;
    public float m_movementSpeedStatusBonus = 1.0f;

    private SkillAbility m_flamePatchSkill;
    private GameObject m_flamePatchPrefab;

    private SkillAbility m_slowPatchSplashSkill;
    private GameObject m_slowPatchPrefab;

    private SkillAbility m_healthRegenStatus;

    private SkillAbility m_knockbackSplashSkill;
    private GameObject m_knockbackSplashVFX;

    private SkillAbility m_impactSplashSkill;
    private GameObject m_impactSplashVFX;

    private SkillAbility m_fireDamageSplashSkill;
    private GameObject m_fireDamageSplashVFX;
    private class SkillAbility
    {
        public float m_strength { private set; get; } = 0.0f;
        public float m_duration { private set; get; } = 0.0f;
        public SkillAbility(float _strength, float _duration)
        {
            m_strength = _strength;
            m_duration = _duration;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        reader = SkillTreeReader.instance;

        playerController = GetComponent<Player_Controller>();

        EvaluateSkills();

        m_flamePatchPrefab = Resources.Load<GameObject>("Skills/FlamePatch");
        m_slowPatchPrefab = Resources.Load<GameObject>("Skills/FrostPatch");
        m_impactSplashVFX = Resources.Load<GameObject>("Skills/VFX/earthOnHit");
        m_knockbackSplashVFX = Resources.Load<GameObject>("Skills/VFX/earthOnKill");
        m_fireDamageSplashVFX = Resources.Load<GameObject>("Skills/VFX/fireOnHit");
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

        m_attackSpeedStatus = null;
        m_movementSpeedStatus = null;
        m_healthRegenStatus = null;
        m_flamePatchSkill = null;
        m_knockbackSplashSkill = null;
        m_impactSplashSkill = null;
        m_fireDamageSplashSkill = null;
        m_slowPatchSplashSkill = null;

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
            path = "Data/Skills/Elemental/" + _skill.id;
            skillData = Resources.Load<SkillData>(path);
        }

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

                // STATUS EFFECT SKILLS
            case "attackSpeedStatus": // Added
                m_attackSpeedStatus = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "movementSpeedStatus": // Added
                m_movementSpeedStatus = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "healthRegenStatus": // Added
                m_healthRegenStatus = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "flamePatch": // Added
                m_flamePatchSkill = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "knockbackSplash": // Added
                m_knockbackSplashSkill = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "impactSplash": // Added
                m_impactSplashSkill = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "fireDamageSplash": // Added
                m_fireDamageSplashSkill = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
                break;
            case "slowPatch": // Added
                m_slowPatchSplashSkill = new SkillAbility(skillData.effectStrength, skillData.effectDuration);
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
            if (_killedTarget)
                OnKillEffectSkills(tag, _actor, _potency);
        }
    }
    private void OnHitEffectSkills(AbilityTag _tag, Actor _actor, float _potency)
    {
        switch (_tag)
        {
            case AbilityTag.Fire:
                if (m_fireDamageSplashSkill == null)
                    return;

                // Splash damage to nearby enemies

                // Get actors within radius
                Collider[] fireColliders = Physics.OverlapSphere(_actor.transform.position, 5.0f, playerController.playerAttack.m_attackTargets);

                List<Actor> hitList = new List<Actor>();
                // Apply damage to those in radius that is not hit actor (multiply damage by potency)
                foreach (var collider in fireColliders)
                {
                    Actor actor = collider.GetComponentInParent<Actor>();
                    if (actor == null || _actor == actor || hitList.Contains(actor))
                        continue;

                    hitList.Add(actor);
                    float damageToDeal = playerController.playerStats.m_abilityDamage * m_fireDamageSplashSkill.m_strength * _potency;
                    Debug.Log(damageToDeal);
                    actor.DealDamageSilent(damageToDeal, CombatSystem.DamageType.Ability);
                    HUDManager.Instance.GetDamageDisplay().DisplayDamage(actor.transform, CombatSystem.DamageType.Ability, damageToDeal);
                }

                // Spawn VFX
                GameObject fireVFX = Instantiate(m_fireDamageSplashVFX, _actor.m_selfTargetTransform.position, transform.rotation);

                break;
            case AbilityTag.Air:
                if (m_attackSpeedStatus == null)
                    return;
                // Increase attack speed on hit

                // Use status system
                playerController.statusEffectContainer.AddStatusEffect(new AttackSpeedStatus(m_attackSpeedStatus.m_strength, m_attackSpeedStatus.m_duration));

                // SFX
                break;
            case AbilityTag.Earth:
                Debug.Log("Tags Exist!");
                if (m_impactSplashSkill == null)
                    return;

                // Splash impact damage to nearby enemies
                // Get actors within radius
                Collider[] earthColliders = Physics.OverlapSphere(_actor.transform.position, 5.0f, playerController.playerAttack.m_attackTargets);

                // Apply impact to those in radius that is not hit actor (multiply impact by potency)
                foreach (var collider in earthColliders)
                {
                    Actor actor = GetComponentInParent<Actor>();
                    if (actor && _actor != actor)
                    {
                        actor.DealImpactDamage(m_impactSplashSkill.m_strength, 0.0f, (actor.transform.position - _actor.transform.position).normalized, CombatSystem.DamageType.Ability);
                    }
                }

                // Spawn VFX
                GameObject earthVFX = Instantiate(m_impactSplashVFX, _actor.m_selfTargetTransform.position, transform.rotation);
                break;
            case AbilityTag.Water:
                if (m_healthRegenStatus == null)
                    return;
                // Heal regen on hit - doesn't stack (about 1% a second)

                // Use status system
                playerController.statusEffectContainer.AddStatusEffect(new HealthRegenStatus(m_healthRegenStatus.m_strength, m_healthRegenStatus.m_duration));

                // SFX & VFX
                break;
        }
    }
    private void OnKillEffectSkills(AbilityTag _tag, Actor _actor, float _potency)
    {
        switch (_tag)
        {
            case AbilityTag.Fire:
                if (m_flamePatchSkill == null)
                    return;
                // Patch of fire where enemy dies
                GameObject flamePatch = Instantiate(m_flamePatchPrefab, _actor.transform.position, transform.rotation);
                flamePatch.GetComponent<BaseSkillObject>().m_strength = m_flamePatchSkill.m_strength * _potency;
                flamePatch.GetComponent<BaseSkillObject>().m_lifetime = m_flamePatchSkill.m_duration;
                break;
            case AbilityTag.Air:
                if (m_movementSpeedStatus == null)
                    return;
                // Bonus movement speed on kills

                // Use status system
                playerController.statusEffectContainer.AddStatusEffect(new MoveSpeedStatus(m_movementSpeedStatus.m_strength, m_movementSpeedStatus.m_duration));

                // SFX
                break;
            case AbilityTag.Earth:
                if (m_knockbackSplashSkill == null)
                    return;
                // Apply knockback to nearby enemies on kill

                // Get actors within radius
                Collider[] earthColliders = Physics.OverlapSphere(_actor.transform.position, 5.0f, playerController.playerAttack.m_attackTargets);


                // Apply knockback to those in radius that is not hit actor
                foreach (var collider in earthColliders)
                {
                    Actor actor = collider.GetComponentInParent<Actor>();
                    if (actor && _actor != actor)
                    {
                        actor.KnockbackActor((actor.transform.position - _actor.transform.position).normalized * m_knockbackSplashSkill.m_strength);
                    }
                }

                // Spawn VFX
                GameObject earthVFX = Instantiate(m_knockbackSplashVFX, _actor.m_selfTargetTransform.position, transform.rotation);

                break;
            case AbilityTag.Water:
                if (m_slowPatchSplashSkill == null)
                    return;
                // Slow patch on kill
                GameObject frostPatch = Instantiate(m_slowPatchPrefab, _actor.transform.position, transform.rotation);
                frostPatch.GetComponent<BaseSkillObject>().m_strength = m_slowPatchSplashSkill.m_strength * _potency;
                frostPatch.GetComponent<BaseSkillObject>().m_lifetime = m_slowPatchSplashSkill.m_duration;
                break;
        }
    }
}
