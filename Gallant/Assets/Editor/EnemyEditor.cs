using Actor.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyData))]
public class EnemyEditor : Editor
{
    private EnemyData m_data;

    private bool m_showLegacy = false;
    private bool m_testCase = false;

    private bool m_showAudio = false;
    private List<bool> m_attackFoldout;

    private int testLevel = 0;

    private float testPlayerPhyDamage;
    private float testPlayerAttSped = 1.0f;
    private float testPlayerAbilDamage;
    private float testPlayerAbilPerSecond;

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
            OnTestView();
            return;
        }
        EditorExtentions.DrawLineOnGUI();

        EditorExtentions.TextField("Enemy Name:", ref m_data.enemyName);
        EditorExtentions.DrawLineOnGUI();

        EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);

        GUI.enabled = !m_data.invincible;
        EditorExtentions.DoubleFloatField("Health: ", ref m_data.health, ref m_data.deltaHealth);
        GUI.enabled = true;

        EditorExtentions.DoubleFloatField("Damage Modifier: ", ref m_data.m_damageModifier, ref m_data.deltaDamageMod);
        EditorExtentions.DoubleFloatField("Speed: ", ref m_data.baseSpeed, ref m_data.deltaSpeed);
        EditorExtentions.DoubleFloatField("Physical Resist: ", ref m_data.phyResist, ref m_data.deltaPhyResist);
        EditorExtentions.DoubleFloatField("Ability Resist: ", ref m_data.abilResist, ref m_data.deltaAbilResist);
        ToggleField("Invincible:", ref m_data.invincible);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Adrenaline Gain", EditorStyles.boldLabel);
        EditorExtentions.MinMaxSlider(ref m_data.adrenalineGainMin, ref m_data.adrenalineGainMax, 0, 100, EditorStyles.label.CalcSize(new GUIContent("0")).x);
        EditorExtentions.FloatField("Delta Adrenaline:", ref m_data.deltaAdrenaline);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("States Machine", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_states"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attacks", EditorStyles.boldLabel);

        AttackListField($"Attack ({m_data.m_attacks.Count})", ref m_data.m_attacks);

        EditorGUILayout.Space();

        m_showAudio = EditorGUILayout.Foldout(m_showAudio, "Sound Effects");
        
        if(m_showAudio)
        {
            EditorExtentions.TextField("Hurt: ", ref m_data.hurtSoundName, 10);
            EditorExtentions.TextField("Death: ", ref m_data.deathSoundName, 10);
        }
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

        float PhyResistPercent = CombatSystem.CalculateDamageNegated(CombatSystem.DamageType.Physical, m_data.phyResist + m_data.deltaPhyResist * testLevel);
        EditorGUILayout.LabelField($"Physical Resist: {m_data.phyResist + m_data.deltaPhyResist * testLevel} ({Mathf.FloorToInt((PhyResistPercent * 100)).ToString()}%)", EditorStyles.label);

        float AbilResistPercent = CombatSystem.CalculateDamageNegated(CombatSystem.DamageType.Ability, m_data.abilResist + m_data.deltaAbilResist * testLevel);
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

    private void UpdateList()
    {
        if (m_attackFoldout != null)
        {
            while (m_attackFoldout.Count < m_data.m_attacks.Count)
                m_attackFoldout.Add(false);

            while (m_attackFoldout.Count > m_data.m_attacks.Count)
                m_attackFoldout.RemoveAt(m_attackFoldout.Count - 1);
        }
    }

    private void ToggleField(string label, ref bool data)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.ToggleLeft("", data);
        EditorGUILayout.EndHorizontal();
    }

    private void AttackListField(string label, ref List<AttackData> data)
    {
        bool status = false;
        status = EditorGUILayout.Foldout(m_attackFoldout != null, label);

        if(status && m_attackFoldout == null)
        {
            m_attackFoldout = new List<bool>();
            UpdateList();
        }
        else if(!status && m_attackFoldout != null)
        {
            m_attackFoldout = null;
        }

        if (status)
        {
            if (data != null && data.Count > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (!AttackField(i, ref m_data.m_attacks))
                    {
                        i--;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Add New")).x)))
            {
                if (data == null)
                    data = new List<AttackData>();

                string path = AssetDatabase.GetAssetPath(target);
                string name = $"newAttack_{target.name}";
                path = path.Substring(0, path.LastIndexOf("/")+1) + "Attacks";

                if (!AssetDatabase.IsValidFolder(path))
                    Directory.CreateDirectory(path);

                AttackData createdData = ScriptableObject.CreateInstance<AttackData>();
                createdData.name = name;
                AssetDatabase.CreateAsset(createdData, path+$"/{name}.asset");
                data.Add(createdData);
                UpdateList();
            }

            EditorGUILayout.LabelField("   |   ", EditorStyles.label, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("  |   ")).x));

            if (GUILayout.Button("Add Exist", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Add New")).x)))
            {
                if (data == null)
                    data = new List<AttackData>();

                data.Add(null);
                UpdateList();
            }

            EditorGUILayout.LabelField("   |   ", EditorStyles.label, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("  |   ")).x));

            if (GUILayout.Button("Clear", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Clear")).x)))
            {
                m_attackFoldout?.Clear();
                data?.Clear();
                UpdateList();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private bool AttackField(int dataID, ref List<AttackData> data, float offset = 10)
    {
        if (m_attackFoldout == null)
            return true;

        string name = "Empty";
        
        GUILayout.BeginHorizontal();
        GUILayout.Space(offset);

        if (data[dataID] != null)
        {
            name = data[dataID].name;
        }

        
        m_attackFoldout[dataID] = EditorGUILayout.Foldout(m_attackFoldout[dataID], name);

        GUILayout.EndHorizontal();

        if (m_attackFoldout[dataID])
        {
            data[dataID] = EditorExtentions.ScriptableObjectField("Data: ", data[dataID], offset);
            
            if (data[dataID] == null)
                return true;

            data[dataID] = EditorExtentions.SaveField<AttackData>("Name: ", data[dataID], offset);

            GUILayout.BeginHorizontal();
            GUILayout.Space(offset);
            EditorGUILayout.LabelField("Type: ");
            data[dataID].attackType = (AttackData.AttackType)EditorGUILayout.EnumPopup(data[dataID].attackType);
            GUILayout.EndHorizontal();

            EditorExtentions.TextField("Animation ID: ", ref data[dataID].animID, offset);
            EditorExtentions.FloatField("Base Damage: ", ref data[dataID].baseDamage, offset);
            EditorExtentions.UnsignedIntegerField("Instances: ", ref data[dataID].instancesPerAttack, offset);
            EditorExtentions.FloatField("Attack Range: ", ref data[dataID].attackRange, offset);
            EditorExtentions.FloatField("Cooldown: ", ref data[dataID].cooldown, offset);
            EditorExtentions.UnsignedIntegerField("Priority: ", ref data[dataID].priority, offset);

            EditorGUILayout.Space();
            switch (data[dataID].attackType)
            {
                default:
                case AttackData.AttackType.Melee:
                    EditorExtentions.Vector3Field("AttackOriginOffset: ", ref data[dataID].attackOriginOffset, offset);
                    break;
                case AttackData.AttackType.Ranged:
                    EditorExtentions.GameObjectField("Projectile prefab: ", ref data[dataID].projectile, offset);
                    EditorExtentions.FloatField("Projectile Speed: ", ref data[dataID].projSpeed, offset);
                    EditorExtentions.FloatField("Projectile LifeTime: ", ref data[dataID].projLifeTime, offset);
                    break;
                case AttackData.AttackType.Instant:
                    EditorExtentions.GameObjectField("Projectile prefab: ", ref data[dataID].projectile, offset);
                    EditorExtentions.Vector3Field("AttackOriginOffset: ", ref data[dataID].attackOriginOffset, offset);
                    EditorExtentions.FloatField("Projectile LifeTime: ", ref data[dataID].projLifeTime, offset);
                    break;
            }
            EditorGUILayout.Space();
            EditorExtentions.GameObjectField("VFX prefab: ", ref data[dataID].vfxSpawn, offset);

            GUILayout.BeginHorizontal();
            GUILayout.Space(offset);

            if (GUILayout.Button("Remove", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Remove")).x)))
            {
                m_data.m_attacks.Remove(data[dataID]);
                GUILayout.EndHorizontal();
                return false;
            }
            GUILayout.EndHorizontal();
        }
        return true;
    }
}
