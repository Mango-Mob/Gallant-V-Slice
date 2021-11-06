using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemEffect
{
    NONE,
    MOVE_SPEED,
    ABILITY_CD,
    ATTACK_SPEED,
    DAMAGE_RESISTANCE,
    MAX_HEALTH_INCREASE,
}
/****************
 * Player_Stats: Manages the stats of the player including effects
 * @author : William de Beer
 * @file : Player_Stats.cs
 * @year : 2021
 */
public class Player_Stats : MonoBehaviour
{
    [Header("Movement Speed")]
    public float m_movementSpeed = 1.0f;
    public float m_movementSpeedItemStrength = 0.2f;

    [Header("Attack Speed")]
    public float m_attackSpeed = 1.0f;
    public float m_attackSpeedItemStrength = 0.1f;

    [Header("Ability Cooldown")]
    public float m_abilityCD = 1.0f;
    public float m_minimumCDPercentage = 0.3f;
    public float m_abilityCDItemStrength = 1.5f;

    [Header("Damage Resistance")]
    public float m_damageResistance = 0.0f;
    public float m_damageResistanceStrength = 0.3f;

    [Header("Maximum Health Multiplier")]
    public float m_maximumHealth = 1.0f;
    public float m_maximumHealthStrength = 0.1f;

    public Dictionary<ItemEffect, int> m_effects = new Dictionary<ItemEffect, int>();

    /*******************
     * AddEffect : Adds an effect to the dictionary
     * @author : William de Beer
     * @param : (ItemEffect) Effect to be added
     */
    public void AddEffect(ItemEffect _effect)
    {
        if (_effect == ItemEffect.NONE)
            return;

        bool foundEffect = false;
        foreach (var effect in m_effects) // Check if effect is already in dictionary
        {
            if (effect.Key == _effect) // Increment if matches
            {
                foundEffect = true;
                m_effects[_effect] += 1;
                break;
            }
        }
        if (!foundEffect)
        {
            m_effects.Add(_effect, 1);
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

        bool foundEffect = false;
        List<ItemEffect> removeList = new List<ItemEffect>();
        foreach (var effect in m_effects) // Check if effect is already in dictionary
        {
            if (effect.Key == _effect)//  Decrement if matches
            {
                foundEffect = true;
                m_effects[_effect] -= 1;
                if (m_effects[_effect] <= 0)
                    removeList.Add(_effect);
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
    private void EvaluateEffects()
    {
        foreach (var effect in m_effects)
        {
            switch (effect.Key)
            {
                case ItemEffect.NONE:
                    // Do nothing
                    break;
                case ItemEffect.MOVE_SPEED: 
                    m_movementSpeed = 1.0f + m_movementSpeedItemStrength * effect.Value;
                    break;
                case ItemEffect.ABILITY_CD: // Ranges from 100% to 30%
                    m_abilityCD = 1.0f * ((1.0f - m_minimumCDPercentage) * Mathf.Pow(m_abilityCDItemStrength, -effect.Value) + m_minimumCDPercentage);
                    break;
                case ItemEffect.ATTACK_SPEED: 
                    m_attackSpeed = 1.0f + m_attackSpeedItemStrength * effect.Value;
                    break;
                case ItemEffect.DAMAGE_RESISTANCE: // Each stack is half as effective as the last (Effect should be exclusive to weapons to prevent being overpowered)
                    m_damageResistance = 0.0f;
                    for (int i = 1; i <= effect.Value; i++)
                    {
                        m_damageResistance += m_damageResistanceStrength * Mathf.Pow(0.5f, i - 1);
                    }
                    break;
                case ItemEffect.MAX_HEALTH_INCREASE: 
                    m_maximumHealth = 1.0f + m_maximumHealthStrength * effect.Value;
                    break;
                default:
                    Debug.LogWarning(effect.Key + " has no effect on stats.");
                    break;
            }
            Debug.Log("Added one " + effect.Key + " buff. Total: " + effect.Value);
        }
    }
}
