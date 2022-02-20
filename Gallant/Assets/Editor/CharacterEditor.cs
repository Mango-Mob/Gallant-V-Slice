using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterData))]
public class CharacterEditor : Editor
{
    private CharacterData m_data;
    private int m_bodyIndex = 0;
    private int m_faceIndex = 0;
    private float m_scale;

    public void Awake()
    {
        m_data = target as CharacterData;        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        m_bodyIndex = EditorGUILayout.IntSlider("Body:", m_bodyIndex, 0, m_data.m_characterBody.Length - 1);
        m_faceIndex = EditorGUILayout.IntSlider("Face:", m_faceIndex, 0, m_data.m_characterFace.Length - 1);
        m_scale = EditorGUILayout.Slider("Preview Scale: ", m_scale, 0.01f, 2.0f);

        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        m_data.DrawToGUI(new Vector2(0, 150), m_bodyIndex, m_faceIndex, m_scale);
    }
}
