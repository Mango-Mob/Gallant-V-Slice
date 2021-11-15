using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Data", menuName = "Game Data/Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    public float health;
    public string name;
    public float baseSpeed;
    public float adrenalineGainMin;
    public float adrenalineGainMax;

    [Tooltip("Used to calculate how effective knockbacks are.")]
    public float mass;
    public bool invincible = false;

    [Tooltip("A exponential scale that approaches 100% Damage reduction, where (400r = 80%, 100r = 50%, 50r = 33%, 25r = 20%)")]
    [Range(0, 400)]
    public float resistance;

    public float m_damageModifier = 1.0f;

    public List<State.Type> m_states = new List<State.Type>();

    public static float CalculateDamage(float inTakeDamage, float resistance, float penetration = 0)
    {
        return inTakeDamage * (100f / (100f + resistance));
    }
}
