﻿using System.Collections;
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

    [Header("Base Weapon Stats")]
    public int m_damage = 10; 
    public int m_speed = 5;
    public int m_effectiveness = 30; // For shield block %
}