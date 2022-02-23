using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class EditorExtentions
{
    private EditorExtentions() { }

    public static T SaveField<T>(string label, T toSave, float offset = 0) where T : Object
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        toSave.name = EditorGUILayout.TextField(toSave.name);

        if(GUILayout.Button("Save"))
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(toSave), toSave.name);
        }

        EditorGUILayout.EndHorizontal();

        return toSave;
    }

    public static void TextField(string label, ref string data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.TextField(data);

        EditorGUILayout.EndHorizontal();
    }
    public static void FloatField(string label, ref float data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.FloatField(data);
        EditorGUILayout.EndHorizontal();
    }

    public static void Vector3Field(string label, ref Vector3 data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.Vector3Field("", data);
        EditorGUILayout.EndHorizontal();
    }

    public static void IntegerField(string label, ref int data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.IntField(data);
        EditorGUILayout.EndHorizontal();
    }

    public static void UnsignedIntegerField(string label, ref uint data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        int newData = EditorGUILayout.IntField((int)data);

        if (newData >= 0)
            data = (uint)newData;

        EditorGUILayout.EndHorizontal();
    }

    public static void DoubleFloatField(string label, ref float dataA, ref float dataB)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        dataA = EditorGUILayout.FloatField(dataA);
        dataB = EditorGUILayout.FloatField(dataB);
        EditorGUILayout.EndHorizontal();
    }

    public static void TripleFloatField(string label, ref float dataA, ref float dataB, ref float dataC)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        dataA = EditorGUILayout.FloatField(dataA);
        dataB = EditorGUILayout.FloatField(dataB);
        dataC = EditorGUILayout.FloatField(dataC);
        EditorGUILayout.EndHorizontal();
    }

    public static void GameObjectField(string label, ref GameObject data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.ObjectField(data, typeof(GameObject), false) as GameObject;
        EditorGUILayout.EndHorizontal();
    }

    public static T ScriptableObjectField<T>(string label, T data, float offset = 0) where T : ScriptableObject
    {
        T temp;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        temp = EditorGUILayout.ObjectField(data, typeof(T), false) as T;
        EditorGUILayout.EndHorizontal();

        return temp;
    }

    public static void DrawLineOnGUI()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    public static void MinMaxSlider(ref float min, ref float max, float limitMin, float limitMax, float labelSizePerChar = 25f)
    {
        string limitMaxAsString = limitMax.ToString();
        float labelSize = EditorStyles.label.CalcSize(new GUIContent(".")).x + labelSizePerChar * limitMaxAsString.Length;

        EditorGUILayout.BeginHorizontal();

        min = EditorGUILayout.FloatField(min, EditorStyles.label, GUILayout.Width(labelSize));
        EditorGUILayout.MinMaxSlider(ref min, ref max, limitMin, limitMax);
        max = EditorGUILayout.FloatField(max, EditorStyles.label, GUILayout.Width(labelSize));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"   Average:", EditorStyles.boldLabel, GUILayout.Width(EditorStyles.boldLabel.CalcSize(new GUIContent("   Average:")).x));
        EditorGUILayout.LabelField(((max + min) / 2f).ToString(), EditorStyles.label);
        EditorGUILayout.EndHorizontal();
    }
}
