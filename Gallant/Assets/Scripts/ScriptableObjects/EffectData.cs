using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "effectData", menuName = "Game Data/Effect Data", order = 1)]
[System.Serializable]
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
                    value += m_stackStrength * Mathf.Pow(m_diminishingValue, i - 1);
                }
                break;
            case StackCalculation.CURVE_TO_EXTREME:
                if (_stackCount != 0)
                    value = m_default * ((1.0f - m_extremeValue) * Mathf.Pow(1.0f / m_stackStrength, -_stackCount) + m_extremeValue);
                else
                    value = m_default;
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
    public AnimationCurve m_curve = AnimationCurve.Linear(0, 0, 10, 1); // DO NOT TOUCH - WILLY
    private int m_graphWidth = 40;
    private int m_graphHeight = 5;
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


        EditorGUILayout.Space(20.0f);
        EditorGUILayout.LabelField("Graph", EditorStyles.boldLabel);
        m_graphWidth = EditorGUILayout.IntField("Graph Size", Mathf.Clamp(m_graphWidth, 0, 50));
        m_curve = AnimationCurve.Linear(0, script.m_default, m_graphWidth, script.GetEffectValue(m_graphWidth));

        m_curve = EditorGUILayout.CurveField("Value Graph", m_curve);
        for (int i = 0; i <= m_graphWidth; i++)
        {
            Keyframe keyframe = new Keyframe(i, script.GetEffectValue(i));
            Debug.Log($"{keyframe.time}, {keyframe.value}");
            m_curve.AddKey(keyframe);
        }
    }
}
#endif

