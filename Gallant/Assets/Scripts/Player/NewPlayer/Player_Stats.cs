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
}
public class Player_Stats : MonoBehaviour
{
    public float m_movementSpeed = 100.0f;
    public float m_attackSpeed = 100.0f;
    public float m_abilityCD = 100.0f;
    public float m_damageResistance = 0.0f;

    public Dictionary<ItemEffect, int> m_effects = new Dictionary<ItemEffect, int>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEffect(ItemEffect _effect)
    {
        bool foundEffect = false;
        foreach (var effect in m_effects)
        {
            if (effect.Key == _effect)
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
    public void RemoveEffect(ItemEffect _effect)
    {
        bool foundEffect = false;
        List<ItemEffect> removeList = new List<ItemEffect>();
        foreach (var effect in m_effects)
        {
            if (effect.Key == _effect)
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
    private void EvaluateEffects()
    {
        foreach (var effect in m_effects)
        {
            switch (effect.Key)
            {
                case ItemEffect.NONE:
                    // Do nothing
                    break;
                case ItemEffect.MOVE_SPEED: // Each effect has diminishing effectiveness.
                    m_movementSpeed = 100.0f * Mathf.Pow(effect.Value, 0.3f);
                    break;
                case ItemEffect.ABILITY_CD: // Ranges from 100% to 30%
                    m_abilityCD = 100.0f * (0.7f * Mathf.Pow(1.5f, -effect.Value) + 0.3f);
                    break;
                case ItemEffect.ATTACK_SPEED: // Each effect has diminishing effectiveness.
                    m_attackSpeed = 100.0f * Mathf.Pow(effect.Value, 0.3f);
                    break;
                case ItemEffect.DAMAGE_RESISTANCE: // Each stack is half as effective as the last (Effect should be exclusive to weapons to prevent being overpowered)
                    m_damageResistance = 0.0f;
                    for (int i = 1; i <= effect.Value; i++)
                    {
                        m_damageResistance += 30.0f * Mathf.Pow(0.5f, i - 1);
                    }
                    break;
                default:
                    Debug.LogWarning(effect.Key + " has no effect on stats.");
                    break;
            }
        }
    }
}
