using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * WeaponData: Scriptable object containing data for weapon
 * @author : William de Beer
 * @file : WeaponData.cs
 * @year : 2021
 */
[CreateAssetMenu(fileName = "weaponData", menuName = "Weapon Data", order = 1)]
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

    public static WeaponData GenerateWeapon(int _level)
    {
        WeaponData data = null;

        // Random weapon type.
        Weapon newWeaponType = (Weapon)Random.Range(0, 3);
        switch (newWeaponType)
        {
            case Weapon.SWORD:
                data = Resources.Load<WeaponData>("Data/BaseWeapons/swordData");
                break;
            case Weapon.SHIELD:
                data = Resources.Load<WeaponData>("Data/BaseWeapons/shieldData");
                break;
            case Weapon.BOOMERANG:
                data = Resources.Load<WeaponData>("Data/BaseWeapons/boomerangData");
                break;
            default:
                Debug.LogWarning("Could not create weapon due to inavlid weapon type randomised.");
                return null;
        }

        // Damage / Speed are randomly assigned (between a range that increases based on the level value).
        data.m_damage += (int)(data.m_damage * Random.Range(0.05f, 0.1f) * (_level - 1.0f));
        data.m_speed += (int)(data.m_speed * Random.Range(0.05f, 0.1f) * (_level - 1.0f));

        data.m_level = _level;

        // Random ability and power level is assigned.
        Ability newAbilityType = (Ability)Random.Range(0, 6);
        int curve = Random.Range(0, 20) + Random.Range(0, 20) - 19;
        int result = Mathf.Max(_level + curve, 0);

        // Power level
        int powerLevel = 0;
        if (result < 20)
            powerLevel = 1;
        else if (result < 40)
            powerLevel = 2;
        else
            powerLevel = 3;

        // Ability
        switch (newAbilityType)
        {
            case Ability.FIREWAVE:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/firewave" + powerLevel.ToString());
                break;
            case Ability.ACID_POOL:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/acidpool" + powerLevel.ToString());
                break;
            case Ability.LIGHTNING_BOLT:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/lightning" + powerLevel.ToString());
                break;
            case Ability.ICE_ROLL:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/frostevade" + powerLevel.ToString());
                break;
            case Ability.HP_BUFF:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/hpbuff" + powerLevel.ToString());
                break;
            case Ability.THORNS:
                data.abilityData = Resources.Load<AbilityData>("Data/Abilities/thorns" + powerLevel.ToString());
                break;
            default:
                Debug.LogWarning("Could not add ability due to inavlid ability type randomised.");
                break;
        }

        // Create weapon name
        data.weaponName = data.abilityData.weaponTitle + " " + data.weaponName;

        //Return new weapon.
        return data;
    }
}
