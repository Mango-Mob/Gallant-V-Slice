using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * WeaponData: Scriptable object containing data for weapon
 * @author : William de Beer
 * @file : WeaponData.cs
 * @year : 2021
 */
[CreateAssetMenu(fileName = "weaponData", menuName = "Game Data/Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Weapon weaponType; // The type of weapon the object is.
    public GameObject weaponModelPrefab; // Prefab of weapon model to be used when player is holding it

    public Sprite weaponIcon;
    public float hitCenterOffset = 0.75f;
    public float hitSize = 1.0f;
    public AbilityData abilityData;
    public ItemEffect itemEffect; // Only for weapons with passives.

    [Header("Base Weapon Stats")]
    public int m_level = 0;
    public int m_damage = 10; 
    public float m_speed = 1;
    public float m_knockback = 1;

    [Header("Dropped Weapon Data")]
    public float m_dropScaleMultiplier = 1.0f;

    public static WeaponData GenerateWeapon(int _level)
    {
        WeaponData data = CreateInstance<WeaponData>();

        // Random weapon type.
        Weapon newWeaponType = (Weapon)Random.Range(0, 3);
        switch (newWeaponType)
        {
            case Weapon.SWORD:
                data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/swordData"));
                break;
            case Weapon.SHIELD:
                data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/shieldData"));
                break;
            case Weapon.BOOMERANG:
                data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/boomerangData"));
                break;
            default:
                Debug.LogWarning("Could not create weapon due to inavlid weapon type randomised.");
                return null;
        }

        data.abilityData = null;

        // Damage / Speed are randomly assigned (between a range that increases based on the level value).
        data.m_damage += (int)(data.m_damage * Random.Range(0.1f, 0.2f) * (_level - 1.0f));
        data.m_speed += (data.m_speed * Random.Range(0.05f, 0.1f) * (_level - 1.0f));

        data.m_level = _level;

        // Random ability and power level is assigned.
        Ability newAbilityType = (Ability)Random.Range(0, 6);
        int curve = Random.Range(0, 3) + Random.Range(0, 3) - 2;
        int result = Mathf.Max(_level + curve, 0);

        // Power level
        int powerLevel = 0;
        if (result < 3)
            powerLevel = 1;
        else if (result < 6)
            powerLevel = 2;
        else
            powerLevel = 3;

        // Ability
        switch (newAbilityType)
        {
            case Ability.FIREWAVE:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/firewave" + powerLevel.ToString());
                break;
            case Ability.SAND_MISSILE:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/sandmissile" + powerLevel.ToString());
                break;
            case Ability.LIGHTNING_BOLT:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/lightning" + powerLevel.ToString());
                break;
            case Ability.ICE_ROLL:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/frostevade" + powerLevel.ToString());
                break;
            case Ability.HP_BUFF:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/barrier" + powerLevel.ToString());
                break;
            case Ability.THORNS:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/thorns" + powerLevel.ToString());
                break;
            default:
                Debug.LogWarning("Could not add ability due to inavlid ability type randomised.");
                break;
        }

        // Create weapon name
        if(data.abilityData != null)
        { 
            data.weaponName = data.weaponName + " of " + data.abilityData.weaponTitle;
            data.abilityData.lastCooldown = 0.0f;
        }

        //Return new weapon.
        return data;
    }

    public static WeaponData GenerateSpecificWeapon(int _weaponLevel, Weapon _weaponType, Ability _abilityType, int _powerLevel)
    {
        WeaponData data = CreateInstance<WeaponData>();

        // Weapon type.
        switch (_weaponType)
        {
            case Weapon.SWORD:
                data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/swordData"));
                break;
            case Weapon.SHIELD:
                data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/shieldData"));
                break;
            case Weapon.BOOMERANG:
                data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/boomerangData"));
                break;
            default:
                Debug.LogWarning("Could not create weapon due to inavlid weapon type randomised.");
                return null;
        }

        data.abilityData = null;

        // Damage / Speed are randomly assigned (between a range that increases based on the level value).
        data.m_damage += (int)(data.m_damage * Random.Range(0.05f, 0.1f) * (_weaponLevel - 1.0f));
        data.m_speed += (int)(data.m_speed * Random.Range(0.05f, 0.1f) * (_weaponLevel - 1.0f));

        data.m_level = _powerLevel;

        // Ability
        switch (_abilityType)
        {
            case Ability.FIREWAVE:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/firewave" + _powerLevel.ToString());
                break;
            case Ability.SAND_MISSILE:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/sandmissile" + _powerLevel.ToString());
                break;
            case Ability.LIGHTNING_BOLT:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/lightning" + _powerLevel.ToString());
                break;
            case Ability.ICE_ROLL:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/frostevade" + _powerLevel.ToString());
                break;
            case Ability.HP_BUFF:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/barrier" + _powerLevel.ToString());
                break;
            case Ability.THORNS:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/thorns" + _powerLevel.ToString());
                break;
            default:
                Debug.LogWarning("Could not add ability due to inavlid ability type randomised.");
                break;
        }

        // Create weapon name
        if (data.abilityData != null)
        {
            data.weaponName = data.weaponName + " of " + data.abilityData.weaponTitle;
            data.abilityData.lastCooldown = 0.0f;
        }

        //Return new weapon.
        return data;
    }

    public void Clone(WeaponData other)
    {
         this.weaponName = other.weaponName;
         this.weaponType = other.weaponType;
         this.weaponModelPrefab = other.weaponModelPrefab;

         this.weaponIcon = other.weaponIcon;
         this.hitCenterOffset = other.hitCenterOffset;
         this.hitSize = other.hitSize;
         this.abilityData = other.abilityData;
         this.itemEffect = other.itemEffect;

         this.m_level = other.m_level;
         this.m_damage = other.m_damage;
         this.m_speed = other.m_speed;
         this.m_knockback = other.m_knockback;
    }
}
