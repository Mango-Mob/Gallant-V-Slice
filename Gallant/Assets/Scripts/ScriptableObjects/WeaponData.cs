using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * WeaponData by William de Beer
 * File: WeaponData.cs
 * Description:
 *		ScriptableObject which contain weapon stats.
 */
[CreateAssetMenu(fileName = "weaponData", menuName = "Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    public Weapon weaponType; // The type of weapon the object is.
    public GameObject weaponModelPrefab; // Prefab of weapon model to be used when player is holding it

    public Sprite weaponIcon;
    public float hitCenterOffset = 0.75f;
    public float hitSize = 1.0f;
    public AbilityData abilityData;
    public ItemEffect itemEffect; // Only for weapons with passives.

    [Header("Base Weapon Stats")]
    public int m_damage = 10; 
    public float m_speed = 1;
}
