using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "skillData", menuName = "Game Data/Skill Data", order = 1)]
public class SkillData : ScriptableObject
{
    [Header("Skill Information")]
    public string skillName;
    [TextArea(10, 15)]
    public string description;
    public Sprite skillIcon;

    [Header("Unlock Information")]
    public int upgradeCost = 10;
    public float upgradeMultiplier = 5.0f;
    public int upgradeMaximum = 1;

    [Header("Effect Information")]
    public float percentageStrength = 0.05f;
    public float effectDuration = 0.0f;

    public static string EvaluateDescription(SkillData data)
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
            switch (description[i + 1])
            {
                case 'p':
                    string percentage = (data.percentageStrength * 100.0f).ToString();

                    indexOfDecimal = percentage.IndexOf('.');
                    insert = (indexOfDecimal != -1 ? percentage.Substring(0, indexOfDecimal) : percentage);
                    break;
                case 't':
                    string time = data.effectDuration.ToString();
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
