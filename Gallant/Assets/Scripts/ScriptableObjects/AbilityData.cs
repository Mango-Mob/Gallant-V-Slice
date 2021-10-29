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
    public string weaponTitle; //_ <weapon> of <title>
    public Ability abilityPower;
    public Sprite abilityIcon;

    [Space(10)]
    [Range(1, 3)]
    public int starPowerLevel = 1;
    [Header("Ability Stats")]
    [Tooltip("The time between uses of the ability.")]
    public float cooldownTime = 1.0f; // The time between uses of the ability.
    [Tooltip("The impact damage of the ability (does not apply to all abilities).")]
    public float damage = 0.0f; // The impact damage of the ability (does not apply to all abilities)
    [Tooltip("The lifetime of the instance spawned by the cast (does not apply to all abilities).")]
    public float lifetime = 0.0f; // The lifetime of the instance spawned by the cast (does not apply to all abilities)
    [Tooltip("The effectiveness of any buff or status effect applied to the target. (does not apply to all abilities).")]
    public float effectiveness = 0.0f; // The effectiveness of any buff or status effect applied to the target. (does not apply to all abilities)
    [Tooltip("The duration of any buff or status effect applied to the target. (does not apply to all abilities).")]
    public float duration = 0.0f; // The duration of any buff or status effect applied to the target. (does not apply to all abilities)
}
