using ActorSystem.AI;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor_Data", menuName = "Game Data/Actor Data", order = 1)]
public class ActorData : ScriptableObject
{
    [Header("Editor constants")]

    [Header("Base Stats")]
    public float health;
    public float cost;

    public float agility;
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

    [Header("Changes per room level")]
    public float deltaHealth = 0;
    public float deltaSpeed = 0;
    public float deltaAdrenaline = 0;
    public float deltaPhyResist = 0;
    public float deltaAbilResist = 0;
    public float deltaDamageMod = 0;

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
