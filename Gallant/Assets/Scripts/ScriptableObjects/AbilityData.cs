using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/****************
 * AbilityData: Scriptable object containing data for ability
 * @author : William de Beer
 * @file : AbilityData.cs
 * @year : 2021
 */
[CreateAssetMenu(fileName = "abilityData", menuName = "Game Data/Ability Data", order = 1)]
public class AbilityData : ScriptableObject
{
    [Header("Ability Information")]
    public string abilityName;
    public string weaponTitle; //_ <weapon> of <title>
    public Ability abilityPower;
    public Sprite abilityIcon;
    [TextArea(10, 15)]
    public string description;

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

    public static string EvaluateDescription(AbilityData data)
    {
        string description = data.description;
        int nextIndex = description.IndexOf('%');

        if (nextIndex == -1)
            return description;

        //Loop through all instances of %, while extending up the string
        for (int i = nextIndex; i < description.Length && i != -1; i = nextIndex)
        {
            string before = description.Substring(0, i);
            string insert = "";
            int indexOfDecimal = -1;
            string after = description.Substring(i + 2);
            switch (description[i+1])
            {
                case 'd': //%d = damage
                    insert = Mathf.FloorToInt(data.damage).ToString();
                    break;
                case 'l':
                    string life = data.lifetime.ToString();
                    indexOfDecimal = life.IndexOf('.') + 2;
                    insert = (indexOfDecimal != -1 ? life.Substring(0, indexOfDecimal) : life);
                    break;
                case 'e':
                    string effect = data.effectiveness.ToString();
                    indexOfDecimal = effect.IndexOf('.') + 2;
                    insert = (indexOfDecimal != -1 ? effect.Substring(0, indexOfDecimal) : effect);
                    break;
                case 'p':
                    string percentage = (data.effectiveness * 100.0f).ToString();

                    indexOfDecimal = percentage.IndexOf('.');
                    insert = (indexOfDecimal != -1 ? percentage.Substring(0, indexOfDecimal) : percentage);
                    break;
                case 't':
                    string time = data.duration.ToString();
                    indexOfDecimal = time.IndexOf('.') + 2;
                    insert = (indexOfDecimal != -1 ? time.Substring(0, indexOfDecimal) : time);
                    break;
                case '%':
                    insert = "%";
                    break;
                default:
                    Debug.LogError($"Evaluation Description: Char not supported: {description[i + 1]}");
                    break;
            }
            description = string.Concat(before, insert, after);
            nextIndex = description.IndexOf('%', nextIndex + insert.Length);
        }

        return description;
    }
}
