using ActorSystem.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActorData))]
public class ActorEditor : Editor
{
    private ActorData m_data;

    private bool m_showLegacy = false;
    private bool m_testCase = false;

    private bool m_showAudio = false;

    private int testLevel = 0;

    private float testPlayerPhyDamage;
    private float testPlayerAttSped = 1.0f;
    private float testPlayerAbilDamage;
    private float testPlayerAbilPerSecond;

    public void Awake()
    {
        m_data = target as ActorData;
    }

    public override void OnInspectorGUI()
    {
        m_showLegacy = EditorGUILayout.Toggle("Legacy View:", m_showLegacy);
       
        //if (m_showLegacy)
        //{
            base.OnInspectorGUI();
            return;
        //}

        //m_testCase = EditorGUILayout.Toggle("Test View:", m_testCase);

        //if (m_testCase)
        //{
        //    OnTestView();
        //    return;
        //}
        //EditorExtentions.DrawLineOnGUI();
        //
        //EditorExtentions.TextField("Actor Name:", ref m_data.ActorName);
        //EditorExtentions.DrawLineOnGUI();
        //
        //EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);
        //
        //GUI.enabled = !m_data.invincible;
        //EditorExtentions.DoubleFloatField("Health: ", ref m_data.health, ref m_data.deltaHealth);
        //GUI.enabled = true;
        //
        //EditorExtentions.DoubleFloatField("Damage Modifier: ", ref m_data.m_damageModifier, ref m_data.deltaDamageMod);
        //EditorExtentions.DoubleFloatField("Speed: ", ref m_data.baseSpeed, ref m_data.deltaSpeed);
        //EditorExtentions.DoubleFloatField("Physical Resist: ", ref m_data.phyResist, ref m_data.deltaPhyResist);
        //EditorExtentions.DoubleFloatField("Ability Resist: ", ref m_data.abilResist, ref m_data.deltaAbilResist);
        //ToggleField("Invincible:", ref m_data.invincible);
        //
        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Adrenaline Gain", EditorStyles.boldLabel);
        //EditorExtentions.MinMaxSlider(ref m_data.adrenalineGainMin, ref m_data.adrenalineGainMax, 0, 100, EditorStyles.label.CalcSize(new GUIContent("0")).x);
        //EditorExtentions.FloatField("Delta Adrenaline:", ref m_data.deltaAdrenaline);
        //
        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("States Machine", EditorStyles.boldLabel);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("m_states"));
        //
        //EditorGUILayout.Space();
        //
        //m_showAudio = EditorGUILayout.Foldout(m_showAudio, "Sound Effects");
        //
        //if(m_showAudio)
        //{
        //    //EditorExtentions.TextField("Hurt: ", ref m_data.hurtSoundName, 10);
        //    //EditorExtentions.TextField("Death: ", ref m_data.deathSoundName, 10);
        //}
    }

    private void OnTestView()
    {
        testLevel = EditorGUILayout.IntSlider("Level: ", testLevel, 0, 20);

        EditorGUILayout.Space();
        EditorExtentions.DrawLineOnGUI();

        float health = m_data.health + m_data.deltaHealth * testLevel;
        EditorGUILayout.LabelField($"Health: {health}", EditorStyles.label);
        EditorGUILayout.LabelField($"Damage Modifer: {m_data.m_damageModifier + m_data.deltaDamageMod * testLevel}", EditorStyles.label);
        EditorGUILayout.LabelField($"Speed: {m_data.baseSpeed + m_data.deltaSpeed * testLevel}", EditorStyles.label);

        float PhyResistPercent = CombatSystem.CalculateDamageNegated(CombatSystem.DamageType.Physical, m_data.phyResist + m_data.deltaPhyResist * testLevel, 0);
        EditorGUILayout.LabelField($"Physical Resist: {m_data.phyResist + m_data.deltaPhyResist * testLevel} ({Mathf.FloorToInt((PhyResistPercent * 100)).ToString()}%)", EditorStyles.label);

        float AbilResistPercent = CombatSystem.CalculateDamageNegated(CombatSystem.DamageType.Ability, m_data.abilResist + m_data.deltaAbilResist * testLevel, 0);
        EditorGUILayout.LabelField($"Ability Resist: {m_data.abilResist + m_data.deltaAbilResist * testLevel} ({Mathf.FloorToInt((AbilResistPercent * 100)).ToString()}%)", EditorStyles.label);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField($"Min Adrenaline: {m_data.adrenalineGainMin + m_data.deltaAdrenaline * testLevel}", EditorStyles.label);
        EditorGUILayout.LabelField($"Average Adrenaline: {(m_data.adrenalineGainMax + m_data.adrenalineGainMin)/2f + m_data.deltaAdrenaline * testLevel}", EditorStyles.label);
        EditorGUILayout.LabelField($"Max Adrenaline: {m_data.adrenalineGainMax + m_data.deltaAdrenaline * testLevel}", EditorStyles.label);
        EditorGUILayout.Space();
        EditorExtentions.DrawLineOnGUI();

        EditorGUILayout.LabelField("Player Input: ", EditorStyles.boldLabel);
        EditorExtentions.DoubleFloatField("Physical Damage | Speed", ref testPlayerPhyDamage, ref testPlayerAttSped);
        EditorExtentions.DoubleFloatField("Ability Damage(per Second) | Seconds", ref testPlayerAbilDamage, ref testPlayerAbilPerSecond);

        EditorGUILayout.Space();
        EditorExtentions.DrawLineOnGUI();

        EditorGUILayout.LabelField("Physical Result: ", EditorStyles.boldLabel);
        if(testPlayerPhyDamage > 0 && testPlayerAttSped > 0)
        {
            int hitsRequired = Mathf.CeilToInt(health / (testPlayerPhyDamage * (1.0f - PhyResistPercent)));
            EditorGUILayout.LabelField($"   Maximum physical hits to kill: {hitsRequired}", EditorStyles.label);
            EditorGUILayout.LabelField($"   Maximum seconds till death: {(float)(hitsRequired/testPlayerAttSped)} seconds", EditorStyles.label);
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Ability Result: ", EditorStyles.boldLabel);
        if (testPlayerAbilDamage > 0 && testPlayerAbilPerSecond > 0)
        {
            int hitsRequired = Mathf.CeilToInt(health / (testPlayerAbilDamage * (1.0f - AbilResistPercent)));
            EditorGUILayout.LabelField($"   Maximum casts hits to kill: {hitsRequired}", EditorStyles.label);
            EditorGUILayout.LabelField($"   Maximum seconds till death: {(float)(hitsRequired / testPlayerAbilPerSecond)} seconds", EditorStyles.label);
        }
        EditorGUILayout.Space();
    }

    private void ToggleField(string label, ref bool data)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.ToggleLeft("", data);
        EditorGUILayout.EndHorizontal();
    }
}
