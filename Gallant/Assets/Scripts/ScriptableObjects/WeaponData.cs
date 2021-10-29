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

    public static WeaponData GenerateWeapon(int _damage, float _speed)
    {
        //Takes in a level parameter, which is used to generate a random weapon.
        //Damage/Speed are randomly assigned (between a range that increases based on the level value).
        //Random ability is assigned.
        //Random weapon type.
        //Return new weapon.

        WeaponData data = CreateInstance<WeaponData>();

        data.m_damage = _damage;
        data.m_speed = _speed;

        return data;
    }
}
