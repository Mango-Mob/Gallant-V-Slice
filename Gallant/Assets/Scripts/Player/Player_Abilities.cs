using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Ability
{
    FIREWAVE,
    ACID_POOL,
    LIGHTNING_BOLT,
    ICE_ROLL,
    HP_BUFF,
    THORNS,
}
/****************
 * Player_Abilities: Description
 * @author : William de Beer
 * @file : Player_Abilities.cs
 * @year : 2021
 */
public class Player_Abilities : MonoBehaviour
{
    [Header("Ability Information")]
    public AbilityBase m_leftAbility;
    public AbilityBase m_rightAbility;

    [Header("Ability Icons")]
    [SerializeField] private UI_AbilityIcon m_leftAbilityIcon;
    [SerializeField] private UI_AbilityIcon m_rightAbilityIcon;

    // Update is called once per frame
    void Update()
    {
        if (m_leftAbilityIcon != null && m_leftAbility != null)
            m_leftAbilityIcon.SetCooldownFill(m_leftAbility.GetCooldownTime());

        if (m_rightAbilityIcon != null && m_rightAbility != null)
            m_rightAbilityIcon.SetCooldownFill(m_rightAbility.GetCooldownTime());
    }
    /*******************
     * UseAbility : Use ability in specified hand
     * @author : William de Beer
     * @param : (Hand) 
     */
    public void UseAbility(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_leftAbility != null)
                    m_leftAbility.TriggerAbility();
                break;
            case Hand.RIGHT:
                if (m_rightAbility != null)
                    m_rightAbility.TriggerAbility();
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
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
            case Ability.ACID_POOL:
                break;
            case Ability.LIGHTNING_BOLT:
                abilityScript = gameObject.AddComponent<Ability_Lightning>();
                break;
            case Ability.ICE_ROLL:
                abilityScript = gameObject.AddComponent<Ability_FrostEvade>();
                break;
            case Ability.HP_BUFF:
                break;
            case Ability.THORNS:
                abilityScript = gameObject.AddComponent<Ability_Thorns>();
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
                m_leftAbility = abilityScript;
                m_leftAbilityIcon.SetIconSprite(_ability != null ? _ability.abilityIcon : null);
                break;
            case Hand.RIGHT:
                Destroy(m_rightAbility);
                m_rightAbility = abilityScript;
                m_rightAbilityIcon.SetIconSprite(_ability != null ? _ability.abilityIcon : null);
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
        }
    }
}
