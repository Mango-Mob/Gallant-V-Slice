using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemEffect
{
    NONE,
    MOVE_SPEED,
    ABILITY_CD,
    ATTACK_SPEED,
    MAX_HEALTH_INCREASE,
    PHYSICAL_DAMAGE,
    ABILITY_DAMAGE,
    PHYSICAL_DEFENCE,
    ABILITY_DEFENCE,
    DAMAGE_RESISTANCE,
    ARCANE_FOCUS,
    ALL,
}
/****************
 * Player_Stats: Manages the stats of the player including effects
 * @author : William de Beer
 * @file : Player_Stats.cs
 * @year : 2021
 */
public class Player_Stats : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }

    public float m_movementSpeed = 1.0f;
    public float m_attackSpeed = 1.0f;
    public float m_abilityCD = 1.0f;
    public float m_damageResistance = 0.0f;
    public float m_maximumHealth = 1.0f;
    public float m_physicalDamage = 1.0f;
    public float m_abilityDamage = 1.0f;
    public float m_physicalDefence = 1.0f;
    public float m_abilityDefence = 1.0f;
    public float m_arcaneFocus = 0.0f;

    public Dictionary<EffectData, int> m_effects = new Dictionary<EffectData, int>();

    private void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }

    /*******************
     * AddEffect : Adds an effect to the dictionary
     * @author : William de Beer
     * @param : (ItemEffect) Effect to be added
     */
    public void AddEffect(ItemEffect _effect)
    {
        Debug.Log($"Adding {_effect}");

        if (_effect == ItemEffect.NONE)
            return;

        if(_effect == ItemEffect.ALL)
        {
            for (int i = 0; i <= (int)ItemEffect.ABILITY_DEFENCE; i++)
            {
                if(!ReachedRuneCap((ItemEffect)i))
                    AddEffect((ItemEffect)i);
            }
            return;
        }

        bool foundEffect = false;
        foreach (var effect in m_effects) // Check if effect is already in dictionary
        {
            if (effect.Key.effect == _effect) // Increment if matches
            {
                foundEffect = true;
                m_effects[effect.Key] += 1;
                break;
            }
        }
        if (!foundEffect)
        {
            EffectData data = null;

            data = EffectData.GetEffectData(_effect);

            if (data == null)
                return;

            m_effects.Add(data, 1);
        }

        EvaluateEffects();
    }
    /*******************
     * RemoveEffect : Removes an effect to the dictionary
     * @author : William de Beer
     * @param : (ItemEffect) Effect to be removed
     */
    public void RemoveEffect(ItemEffect _effect)
    {
        if (_effect == ItemEffect.NONE)
            return;

        if (_effect == ItemEffect.ALL)
        {
            for (int i = 0; i <= (int)ItemEffect.ABILITY_DEFENCE; i++)
            {
                if (GetEffectQuantity((ItemEffect)i) > 0)
                    RemoveEffect((ItemEffect)i);
            }
            return;
        }

        bool foundEffect = false;
        List<EffectData> removeList = new List<EffectData>();
        foreach (var effect in m_effects) // Check if effect is already in dictionary
        {
            if (effect.Key.effect == _effect)//  Decrement if matches
            {
                foundEffect = true;
                m_effects[effect.Key] -= 1;
                if (m_effects[effect.Key] <= 0)
                    removeList.Add(effect.Key);
                break;
            }
        }
        if (!foundEffect)
        {
            Debug.LogWarning("Tried to remove effect that was not on player.");
        }

        // If effect quantity is <= 0 then remove it
        foreach (var effect in removeList)
        {
            m_effects.Remove(effect);
        }

        EvaluateEffects();
    }

    /*******************
     * EvaluateEffects : Calculates item effect power
     * @author : William de Beer
     */
    public void EvaluateEffects()
    {
        foreach (var effect in m_effects)
        {
            switch (effect.Key.effect)
            {
                case ItemEffect.NONE:
                    break;
                case ItemEffect.MOVE_SPEED:
                    m_movementSpeed = effect.Key.GetEffectValue(effect.Value);
                    break;
                case ItemEffect.ABILITY_CD:
                    m_abilityCD = effect.Key.GetEffectValue(effect.Value);
                    break;
                case ItemEffect.ATTACK_SPEED:
                    m_attackSpeed = effect.Key.GetEffectValue(effect.Value);
                    break;
                case ItemEffect.DAMAGE_RESISTANCE:
                    m_damageResistance = effect.Key.GetEffectValue(effect.Value);
                    break;
                case ItemEffect.MAX_HEALTH_INCREASE:
                    float currentHealth = m_maximumHealth;
                    m_maximumHealth = effect.Key.GetEffectValue(effect.Value);

                    float healAmount = (m_maximumHealth - currentHealth) * playerController.playerResources.m_maxHealth;

                    if (healAmount > 0.0f)
                        playerController.playerResources.ChangeHealth(healAmount);
                    break;
                case ItemEffect.PHYSICAL_DAMAGE:
                    m_physicalDamage = effect.Key.GetEffectValue(effect.Value);
                    break;
                case ItemEffect.ABILITY_DAMAGE:
                    m_abilityDamage = effect.Key.GetEffectValue(effect.Value);
                    break;
                case ItemEffect.PHYSICAL_DEFENCE:
                    m_physicalDefence = effect.Key.GetEffectValue(effect.Value);
                    playerController.Defence = m_physicalDefence;
                    break;
                case ItemEffect.ABILITY_DEFENCE:
                    m_abilityDefence = effect.Key.GetEffectValue(effect.Value);
                    playerController.Ward = m_abilityDefence;
                    break;
                case ItemEffect.ARCANE_FOCUS:
                    m_arcaneFocus = effect.Key.GetEffectValue(effect.Value);
                    break;
                default:
                    Debug.Log("Added one " + effect.Key + " buff. Total: " + effect.Value);
                    break;
            }
        }

        if (playerController.m_statsMenu == null)
            playerController.m_statsMenu = HUDManager.Instance.GetElement<UI_StatsMenu>("StatsMenu");
        playerController.m_statsMenu.UpdateList();

        //foreach (var effect in m_effects)
        //{
        //    switch (effect.Key)
        //    {
        //        case ItemEffect.NONE:
        //            // Do nothing
        //            break;
        //        case ItemEffect.MOVE_SPEED: 
        //            m_movementSpeed = 1.0f + m_movementSpeedItemStrength * effect.Value;
        //            break;
        //        case ItemEffect.ABILITY_CD: // Ranges from 100% to 30%
        //            m_abilityCD = 1.0f * ((1.0f - m_minimumCDPercentage) * Mathf.Pow(m_abilityCDItemStrength, -effect.Value) + m_minimumCDPercentage);
        //            break;
        //        case ItemEffect.ATTACK_SPEED: 
        //            m_attackSpeed = 1.0f + m_attackSpeedItemStrength * effect.Value;
        //            break;
        //        case ItemEffect.DAMAGE_RESISTANCE: // Each stack is half as effective as the last (Effect should be exclusive to weapons to prevent being overpowered)
        //            m_damageResistance = 0.0f;
        //            for (int i = 1; i <= effect.Value; i++)
        //            {
        //                m_damageResistance += m_damageResistanceStrength * Mathf.Pow(0.5f, i - 1);
        //            }
        //            break;
        //        case ItemEffect.MAX_HEALTH_INCREASE: 
        //            m_maximumHealth = 1.0f + m_maximumHealthStrength * effect.Value;
        //            break;
        //        default:
        //            Debug.LogWarning(effect.Key + " has no effect on stats.");
        //            break;
        //    }
        //    Debug.Log("Added one " + effect.Key + " buff. Total: " + effect.Value);
        //}
    }

    public int GetEffectQuantity(ItemEffect _effect)
    {
        foreach (var effect in m_effects) // Check if effect is in dictionary
        {
            if (effect.Key.effect == _effect)//  Check if effect matches
            {
                return effect.Value; // Return value.
            }
        }

        return 0;
    }

    public bool ReachedRuneCap(ItemEffect _effect)
    {
        return GetEffectQuantity(_effect) >= 10.0f; 
    }
}
