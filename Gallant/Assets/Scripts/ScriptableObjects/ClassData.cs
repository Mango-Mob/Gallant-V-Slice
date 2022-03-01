using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "classData", menuName = "Game Data/Class Data", order = 1)]
public class ClassData : ScriptableObject
{
    public InkmanClass inkmanClass;
    public Sprite m_classIcon;

    [Header("Starting Weapons")]
    public WeaponData leftWeapon;
    public WeaponData rightWeapon;

    [Header("Armour")]
    public Mesh helmetModel;
    public Material helmetMaterial;
}
