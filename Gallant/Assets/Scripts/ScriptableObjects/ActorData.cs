using ActorSystem.AI;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor_Data", menuName = "Game Data/Actor Data", order = 1)]
public class ActorData : ScriptableObject
{
    [Header("Editor constants")]

    [Header("Base Stats")]
    public float health;
    public float stamina;
    public float staminaReg;
    public float radius;
    public int tier = 1;

    public string ActorName;
    public float baseSpeed;

    public float adrenalineGainMin;
    
    public float adrenalineGainMax;

    [Tooltip("Used to calculate how effective knockbacks are.")]
    public float mass;
    public bool invincible = false;

    [Range(0, 400)]
    public float phyResist;
    [Range(0, 400)]
    public float abilResist;

    public float m_damageModifier = 1.0f;

    [Header("Sound Effects")]
    public AudioClip[] deathSounds;
    public AudioClip[] hurtSounds;

    public List<State.Type> m_states;
    public State.Type m_initialState;

    public static float CalculateDamage(float inTakeDamage, float resistance, float penetration = 0)
    {
        return inTakeDamage * (100f / (100f + resistance));
    }
}
