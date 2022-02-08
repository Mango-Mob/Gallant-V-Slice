using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy_Data", menuName = "Game Data/Enemy Data", order = 1)]
public class EnemyData : ScriptableObject
{
    [Header("Editor constants")]

    [Header("Base Stats")]
    public float health;
    
    public string enemyName;
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
    public string deathSoundName;
    public string hurtSoundName;

    public List<State.Type> m_states = new List<State.Type>();
    public Dictionary<string, Attack> m_attacks = new Dictionary<string, Attack>();

    public static float CalculateDamage(float inTakeDamage, float resistance, float penetration = 0)
    {
        return inTakeDamage * (100f / (100f + resistance));
    }
}

public class Attack
{
    public float baseDamage;
    public string animationName;
    public float cooldown;
    public uint priority;

    public GameObject projectile;
    public GameObject vfxSpawn;

    public enum AttackType
    {
        Melee,      //Effect after animation
        Ranged,     //Spawn Proj after animation
        Instant,    //Spawn proj at target after animation
    }
}
