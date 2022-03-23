using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Ability
{
    FIREWAVE,
    SAND_MISSILE,
    LIGHTNING_BOLT,
    ICE_ROLL,
    HP_BUFF,
    THORNS,
    ARCANE_BOLT,
    FLAME_ROLL,
    ROLL_BASH,

    NONE,
}
/****************
 * Player_Abilities: Description
 * @author : William de Beer
 * @file : Player_Abilities.cs
 * @year : 2021
 */
public class Player_Abilities : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    [SerializeField] private AbilityPassiveVFX m_passiveVFX;

    [Header("Ability Information")]
    public AbilityBase m_leftAbility;
    public AbilityBase m_rightAbility;

    private UI_AbilityIcon m_leftAbilityIcon;
    private UI_AbilityIcon m_rightAbilityIcon;

    [SerializeField] private float m_globalCooldown = 1.0f;
    private float m_leftHandGlobalTimer = 0.0f;
    private float m_rightHandGlobalTimer = 0.0f;

    private void Awake()
    {
        m_leftAbilityIcon = HUDManager.Instance.GetElement<UI_AbilityIcon>("AbilityL");
        m_rightAbilityIcon = HUDManager.Instance.GetElement<UI_AbilityIcon>("AbilityR");
    }
    private void Start()
    {
        playerController = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_passiveVFX != null)
            m_passiveVFX.transform.position = playerController.GetFloorPosition();

        if (m_leftAbilityIcon != null && m_leftAbility != null)
            m_leftAbilityIcon.SetCooldown(m_leftAbility.GetCooldownTime(), m_leftAbility.m_data.isPassive ? 1.0f : m_leftAbility.m_data.cooldownTime * (playerController.playerStats.m_abilityCD));

        if (m_rightAbilityIcon != null && m_rightAbility != null)
            m_rightAbilityIcon.SetCooldown(m_rightAbility.GetCooldownTime(), m_rightAbility.m_data.isPassive ? 1.0f : m_rightAbility.m_data.cooldownTime * (playerController.playerStats.m_abilityCD));

        if (m_leftHandGlobalTimer > 0.0f)
            m_leftHandGlobalTimer -= Time.deltaTime;

        if (m_rightHandGlobalTimer > 0.0f)
            m_rightHandGlobalTimer -= Time.deltaTime;
    }

    /*******************
     * StartUsing : Begin the use of held weapon via animation.
     * @author : William de Beer
     * @param : (Hand) The hand of the weapon to be used
     */
    public void StartUsing(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_leftHandGlobalTimer <= 0.0f && m_leftAbility != null && m_leftAbility.m_canUse && !m_leftAbility.m_isPassive)
                {
                    playerController.animator.SetBool("LeftCast", true);
                    m_leftHandGlobalTimer = m_globalCooldown;
                }
                break;
            case Hand.RIGHT:
                if (m_rightHandGlobalTimer <= 0.0f && m_rightAbility != null && m_rightAbility.m_canUse && !m_rightAbility.m_isPassive)
                {
                    playerController.animator.SetBool("RightCast", true);
                    m_rightHandGlobalTimer = m_globalCooldown;
                }
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
        }
    }

    /*******************
     * UseAbility: Use a ability's functionality from an animation event. This is why it uses a bool instead of a enum.
     * @author : William de Beer
     * @param : (bool) Is left hand, otherwise use right
     */
    public void UseAbility(bool _left)
    {
        if (_left)
        {
            if (m_leftAbility != null)
            {
                m_leftAbility.TriggerAbility();
                //playerController.playerAudioAgent.PlayCast(); // Audio
                m_leftAbility.StartCooldown();
            }
        }
        else
        {
            if (m_rightAbility != null)
            {
                m_rightAbility.TriggerAbility();
                //playerController.playerAudioAgent.PlayCast(); // Audio
                m_rightAbility.StartCooldown();
            }
        }
    }
    /*******************
     * SetAbility : Set ability in specified hand
     * @author : William de Beer
     * @param : (AbilityData), (Hand) 
     */
    public void SetAbility(AbilityData _ability, Hand _hand)
    {
        AbilityBase abilityScript = null;
        if (_ability)
        switch (_ability.abilityPower)
        {
            case Ability.FIREWAVE:
                abilityScript = gameObject.AddComponent<Ability_Firewave>();
                break;
            case Ability.SAND_MISSILE:
                abilityScript = gameObject.AddComponent<Ability_SandMissile>();
                break;
            case Ability.LIGHTNING_BOLT:
                abilityScript = gameObject.AddComponent<Ability_Lightning>();
                break;
            case Ability.ICE_ROLL:
                abilityScript = gameObject.AddComponent<Ability_FrostEvade>();
                break;
            case Ability.HP_BUFF:
                abilityScript = gameObject.AddComponent<Ability_Barrier>();
                break;
            case Ability.THORNS:
                abilityScript = gameObject.AddComponent<Ability_Thorns>();
                break;
            case Ability.ARCANE_BOLT:
                abilityScript = gameObject.AddComponent<Ability_ArcaneBolt>();
                break;
            case Ability.FLAME_ROLL:
                abilityScript = gameObject.AddComponent<Ability_FlameEvade>();
                break;
            case Ability.ROLL_BASH:
                abilityScript = gameObject.AddComponent<Ability_RollBash>();
                break;
            default:
                Debug.Log("No ability script set for " + _ability);
                break;
        }

        if (abilityScript != null)
        {
            abilityScript.m_data = _ability;
            abilityScript.m_attachedHand = _hand;
        }

        switch (_hand)
        {
            case Hand.LEFT:
                Destroy(m_leftAbility);

                if (m_rightAbility != null)
                    m_rightAbility.m_synergyData = null;

                m_leftAbility = abilityScript;
                m_leftAbilityIcon.SetIconSprite(_ability != null ? _ability.abilityIcon : null);
                m_leftAbilityIcon.SetPowerLevel(_ability != null ? _ability.starPowerLevel : 0);
                m_leftAbilityIcon.SetFrame(abilityScript != null ? (abilityScript.m_isPassive ? FrameType.PASSIVE : FrameType.ACTIVE) : FrameType.NONE);

                if (m_leftAbility == null)
                    m_leftAbilityIcon.SetCooldown(0.0f, 1.0f);
                break;
            case Hand.RIGHT:
                Destroy(m_rightAbility);

                if (m_rightAbility != null)
                    m_rightAbility.m_synergyData = null;

                m_rightAbility = abilityScript;
                m_rightAbilityIcon.SetIconSprite(_ability != null ? _ability.abilityIcon : null);
                m_rightAbilityIcon.SetPowerLevel(_ability != null ? _ability.starPowerLevel : 0);
                m_rightAbilityIcon.SetFrame(abilityScript != null ? (abilityScript.m_isPassive ? FrameType.PASSIVE : FrameType.ACTIVE) : FrameType.NONE);

                if (m_rightAbility == null)
                    m_rightAbilityIcon.SetCooldown(0.0f, 1.0f);
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
        }

        if (m_leftAbility != null && m_rightAbility != null && 
            m_leftAbility.m_isSynergyAvailable && m_rightAbility.m_isSynergyAvailable && 
            m_leftAbility.GetType() == m_rightAbility.GetType())
        {
            m_rightAbility.m_synergyData = AbilityData.CreateWeaponDataSynergy(m_leftAbility.m_data, m_rightAbility.m_data);
        }

        if (_ability != null)
            m_passiveVFX.SetAbility(_hand, _ability.abilityPower);
        else
            m_passiveVFX.SetAbility(_hand, Ability.NONE);
    }

    public void PassiveProcess(Hand _hand, PassiveType _type, GameObject _object = null, float _damage = 0.0f)
    {
        AbilityBase abilityScript = null;
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_rightAbility != null)
                    if (m_leftAbility == null || (m_leftAbility.m_isSynergyAvailable && m_leftAbility.GetType() == m_rightAbility.GetType()))
                    {
                        return;
                    }
                abilityScript = m_leftAbility;
                break;
            case Hand.RIGHT:
                abilityScript = m_rightAbility;
                break;
        }

        if (abilityScript == null)
            return;

        switch (_type)
        {
            case PassiveType.PASSIVE:
                abilityScript.AbilityPassive();
                break;
            case PassiveType.HIT_RECIEVED:
                //if (_object == null)
                //    Debug.LogError($"Passive type {_type} requires _object and _float parameters to be used");
                abilityScript.AbilityOnHitRecieved(_object, _damage);
                break;
            case PassiveType.HIT_DEALT:
                //if (_object == null)
                //    Debug.LogError($"Passive type {_type} requires _object and _float parameters to be used");
                abilityScript.AbilityOnHitDealt(_object, _damage);
                break;
            case PassiveType.BEGIN_ROLL:
                abilityScript.AbilityOnBeginRoll();
                break;
            case PassiveType.WHILE_ROLLING:
                abilityScript.AbilityWhileRolling();
                break;
            case PassiveType.END_ROLL:
                abilityScript.AbilityOnEndRoll();
                break;
        }
    }
}
