using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "effectData", menuName = "Game Data/Effect Data", order = 1)]
public class EffectData : ScriptableObject
{   
    public enum StackCalculation
    {
        ADDITIVE,
        DIMINISHING,
        CURVE_TO_EXTREME,
    }

    [Header("Effect Information")]
    public ItemEffect effect;
    public Sprite icon;
    public float m_default = 1.0f;
    [Space(20.0f)]
    public StackCalculation m_stackCalculation;
    public float m_stackStrength = 0.1f;
    [HideInInspector] public float m_diminishingValue = 0.5f;
    [HideInInspector] public float m_extremeValue = 0.3f;

    public float GetEffectValue(int _stackCount)
    {
        float value = 0.0f;

        switch (m_stackCalculation)
        {
            case StackCalculation.ADDITIVE:
                value = m_default + m_stackStrength * _stackCount;
                break;
            case StackCalculation.DIMINISHING:
                for (int i = 1; i <= _stackCount; i++)
                {
                    value += m_stackStrength * Mathf.Pow(0.5f, i - 1);
                }
                break;
            case StackCalculation.CURVE_TO_EXTREME:
                value = 1.0f * ((1.0f - m_extremeValue) * Mathf.Pow(m_stackStrength, -_stackCount) + m_extremeValue);
                break;
            default:
                break;
        }

        return value;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EffectData))]
public class RandomScript_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 

        EffectData script = (EffectData)target;

        if (script.m_stackCalculation == EffectData.StackCalculation.CURVE_TO_EXTREME) 
        {
            script.m_extremeValue = EditorGUILayout.FloatField("Extreme Value", script.m_extremeValue);
        }
        else if (script.m_stackCalculation == EffectData.StackCalculation.DIMINISHING)
        {
            script.m_diminishingValue = EditorGUILayout.FloatField("Diminishing Value", script.m_diminishingValue);
        }
    }
}
#endif

