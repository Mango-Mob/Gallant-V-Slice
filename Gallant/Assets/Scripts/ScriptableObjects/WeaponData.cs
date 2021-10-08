using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "weaponData", menuName = "Weapon Data", order = 1)]
public class WeaponData : ScriptableObject
{
    public Weapon weaponType;
    public GameObject weaponModelPrefab;
    [Header("Base Weapon Stats")]
    public int m_damage;
    public int m_speed;
}
