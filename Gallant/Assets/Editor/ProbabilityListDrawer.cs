using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ProbabilityList<>))]
public class ProbabilityListDrawer : PropertyDrawer
{
    private bool m_foldoutStatus = false;
    private int m_count = 0;

    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        //fieldInfo.GetValue(property.serializedObject.targetObject) as ProbabilityList;
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        m_foldoutStatus = EditorGUI.BeginFoldoutHeaderGroup(position, m_foldoutStatus, label);
        if (m_foldoutStatus)
        {

        }
        EditorGUI.EndFoldoutHeaderGroup();
        m_count = EditorGUI.IntField(position, label, m_count);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
