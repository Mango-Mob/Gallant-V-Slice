using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "abilityData", menuName = "Ability Data", order = 1)]
public class AbilityData : ScriptableObject
{
    [Header("Ability Information")]
    public string abilityName;
    public Ability abilityPower;
    public Sprite icon;
    public float cooldownTime = 1.0f;
    [Range(1, 3)]
    public int starPowerLevel = 1;
}
