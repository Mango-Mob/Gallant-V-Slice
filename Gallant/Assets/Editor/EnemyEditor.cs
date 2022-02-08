using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyEditor : Editor
{
    private EnemyData m_data;

    private bool m_showLegacy = false;
    private bool m_testCase = false;

    private Dictionary<string, bool> m_attackFoldout;
    public void Awake()
    {
        m_data = target as EnemyData;
    }

    public override void OnInspectorGUI()
    {
        m_showLegacy = EditorGUILayout.Toggle("Legacy View:", m_showLegacy);
        
        if (m_showLegacy)
        {
            base.OnInspectorGUI();
            return;
        }

        m_testCase = EditorGUILayout.Toggle("Test View:", m_testCase);

        if (m_testCase)
        {
            return;
        }
        DrawLineOnGUI();
                
        TextFieldWithLabel("Enemy Name:", ref m_data.enemyName);
        DrawLineOnGUI();

        EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);

        GUI.enabled = !m_data.invincible;
        DoubleFloatField("Health: ", ref m_data.health, ref m_data.deltaHealth);
        GUI.enabled = true;

        DoubleFloatField("Damage Modifier: ", ref m_data.m_damageModifier, ref m_data.deltaDamageMod);
        DoubleFloatField("Speed: ", ref m_data.baseSpeed, ref m_data.deltaSpeed);
        DoubleFloatField("Physical Resist: ", ref m_data.phyResist, ref m_data.deltaPhyResist);
        DoubleFloatField("Ability Resist: ", ref m_data.abilResist, ref m_data.deltaAbilResist);
        ToggleField("Invincible:", ref m_data.invincible);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Adrenaline Gain", EditorStyles.boldLabel);
        MinMaxSliderWithLabels(ref m_data.adrenalineGainMin, ref m_data.adrenalineGainMax, 0, 100, EditorStyles.label.CalcSize(new GUIContent("0")).x);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("States Machine", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_states"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attacks", EditorStyles.boldLabel);
        AttackDictonaryField("Attack", ref m_data.m_attacks);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sound Effects", EditorStyles.boldLabel);
    }

    private void TextFieldWithLabel(string label, ref string data)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.TextField(data);
        EditorGUILayout.EndHorizontal();
    }

    private void ToggleField(string label, ref bool data)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.ToggleLeft("", data);
        EditorGUILayout.EndHorizontal();
    }

    private void DoubleFloatField(string label, ref float data, ref float delta)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.FloatField(data);
        delta = EditorGUILayout.FloatField(delta);
        EditorGUILayout.EndHorizontal();
    }

    private void MinMaxSliderWithLabels(ref float min, ref float max, float limitMin, float limitMax, float labelSizePerChar = 25f)
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

    private void DrawLineOnGUI()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    private void AttackDictonaryField(string label, ref Dictionary<string, Attack> data)
    {
        bool status = false;
        status = EditorGUILayout.Foldout(m_attackFoldout != null, label);

        if(status && m_attackFoldout == null)
        {
            m_attackFoldout = new Dictionary<string, bool>();
        }
        else if(!status && m_attackFoldout != null)
        {
            m_attackFoldout = null;
        }

        if(data.Count > 0)
        {
            string[] keys = data.Keys.ToArray();
            for (int i = 0; i < data.Count; i++)
            {
                AttackField(keys[i], ref data);
            }
        }

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Add New", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Add New")).x)))
        {
            //TODO: CHECK IF NAME DOESN"T ALREADY EXIST
            m_attackFoldout?.Add($"attack{data.Count}", false);
            data.Add($"attack{data.Count}", new Attack());
        }

        EditorGUILayout.LabelField("   |   ", EditorStyles.label, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("  |   ")).x));

        if(GUILayout.Button("Clear", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Clear")).x)))
        {
            m_attackFoldout?.Clear();
            data.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void AttackField(string key, ref Dictionary<string, Attack> data)
    {
        if (m_attackFoldout == null)
            return;

        if (!m_attackFoldout.ContainsKey(key))
            m_attackFoldout.Add(key, false);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        m_attackFoldout[key] = EditorGUILayout.Foldout(m_attackFoldout[key], key);

        GUILayout.EndHorizontal();

        if (m_attackFoldout[key])
        {
            //render
            Attack attack = data[key];

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Update", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Update")).x)))
            {
                //remove data at key
                m_attackFoldout.Remove(key);
                data.Remove(key);

                //Add it back
                data.Add(key, attack);
                m_attackFoldout.Add(key, true);
            }

            EditorGUILayout.LabelField("   |   ", EditorStyles.label, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("  |   ")).x));

            if (GUILayout.Button("Remove", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Remove")).x)))
            {
                m_attackFoldout.Remove(key);
                data.Remove(key);
            }
            GUILayout.EndHorizontal();
        }
    }
}
