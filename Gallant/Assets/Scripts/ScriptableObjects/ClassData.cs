using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "classData", menuName = "Game Data/Class Data", order = 1)]
[System.Serializable]
public class ClassData : ScriptableObject
{
    public InkmanClass inkmanClass;
    public Sprite m_classIcon;

    [Header("Starting Weapons")]
    public WeaponData startWeapon;

    [Header("Armour")]
    public Mesh helmetModel;
    public Material helmetMaterial;

    [Header("Starting Runes")]
    public int movementSpeed = 0;
    public int attackSpeed = 0;
    public int abilityCD = 0;
    public int maximumHealth = 0;
    public int physicalDamage = 0;
    public int abilityDamage = 0;
    public int physicalDefence = 0;
    public int abilityDefence = 0;
}