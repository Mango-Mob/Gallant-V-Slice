using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/****************
 * AbilityData: Scriptable object containing data for ability
 * @author : William de Beer
 * @file : AbilityData.cs
 * @year : 2021
 */
[CreateAssetMenu(fileName = "abilityData", menuName = "Ability Data", order = 1)]
public class AbilityData : ScriptableObject
{
    [Header("Ability Information")]
    public string abilityName;
    public Ability abilityPower;
    public Sprite abilityIcon;


    public float cooldownTime = 1.0f;
    [Range(1, 3)]
    public int starPowerLevel = 1;
}
