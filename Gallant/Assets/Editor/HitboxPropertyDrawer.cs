using ActorSystem.Data;
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Hitbox))]
public class HitboxPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect oneCell = new Rect(position.x, position.y, position.width, position.height/GetDivisionCount((Hitbox.HitType)property.FindPropertyRelative("type").enumValueIndex));

        GUI.Label(oneCell, label);
        EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 1, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("type"));

        switch ((Hitbox.HitType)property.FindPropertyRelative("type").enumValueIndex)
        {
            case Hitbox.HitType.Box:
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 2, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("center"));
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 3, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("size"));
                break;
            default:
            case Hitbox.HitType.Sphere:
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 2, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("center"));
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 3, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("radius"));
                break;
            case Hitbox.HitType.Capsule:
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 2, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("start"));
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 3, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("end"));
                EditorGUI.PropertyField(new Rect(oneCell.x + 15f, oneCell.y + oneCell.height * 4, oneCell.width - 15f, oneCell.height), property.FindPropertyRelative("radius"));
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 1.2f * GetDivisionCount((Hitbox.HitType)property.FindPropertyRelative("type").enumValueIndex);
    }

    private int GetDivisionCount(Hitbox.HitType propertyType)
    {
        switch (propertyType)
        {
            case Hitbox.HitType.Box:
                return 4;
            default:
            case Hitbox.HitType.Sphere:
                return 4;
            case Hitbox.HitType.Capsule:
                return 5;
        }
    }
}
