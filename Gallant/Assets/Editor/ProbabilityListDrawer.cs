using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Extentions.WeightedOption<>))]
public class ProbabilityListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.ObjectField(new Rect(position.x, position.y, position.width - 48, position.height), property.FindPropertyRelative("data"), new GUIContent());
        property.FindPropertyRelative("weight").intValue = EditorGUI.IntField(new Rect(position.x + position.width - 48, position.y, 48, position.height), property.FindPropertyRelative("weight").intValue);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
