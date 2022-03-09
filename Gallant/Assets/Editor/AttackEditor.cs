using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackData))]
public class AttackEditor : Editor
{
    private AttackData m_data;

    public void Awake()
    {
        m_data = target as AttackData;        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
