using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomData))]
public class RoomEditor : Editor
{
    private RoomData m_data;

    public void Awake()
    {
        m_data = target as RoomData;        
    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label($"Spawn Cost : {m_data.CalculateCost()}");
        base.OnInspectorGUI();
    }
}
