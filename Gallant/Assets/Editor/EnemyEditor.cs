using Actor.AI;
using System;
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

    private bool m_showAudio = false;
    private Dictionary<string, bool> m_attackFoldout;

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
        FloatFieldWithLabel("Delta Adrenaline:", ref m_data.deltaAdrenaline);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("States Machine", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_states"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Attacks", EditorStyles.boldLabel);

        AttackListField("Attack", ref m_data.m_attacks);

        EditorGUILayout.Space();

        m_showAudio = EditorGUILayout.Foldout(m_showAudio, "Sound Effects");
        
        if(m_showAudio)
        {
            TextFieldWithLabel("Hurt: ", ref m_data.hurtSoundName, 10);
            TextFieldWithLabel("Death: ", ref m_data.deathSoundName, 10);
        }
    }

    private void OnTestView()
    {
        testLevel = EditorGUILayout.IntSlider("Level: ", testLevel, 0, 20);

        EditorGUILayout.Space();
        DrawLineOnGUI();

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
        DrawLineOnGUI();

        EditorGUILayout.LabelField("Player Input: ", EditorStyles.boldLabel);
        DoubleFloatField("Physical Damage | Speed", ref testPlayerPhyDamage, ref testPlayerAttSped);
        DoubleFloatField("Ability Damage(per Second) | Seconds", ref testPlayerAbilDamage, ref testPlayerAbilPerSecond);

        EditorGUILayout.Space();
        DrawLineOnGUI();

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

    private void TextFieldWithLabel(string label, ref string data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.TextField(data);
        EditorGUILayout.EndHorizontal();
    }

    private void FloatFieldWithLabel(string label, ref float data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.FloatField(data);
        EditorGUILayout.EndHorizontal();
    }

    private void Vector3FieldWithLabel(string label, ref Vector3 data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.Vector3Field("", data);
        EditorGUILayout.EndHorizontal();
    }
    private void IntegerFieldWithLabel(string label, ref int data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.IntField(data);
        EditorGUILayout.EndHorizontal();
    }

    private void UnsignedIntegerFieldWithLabel(string label, ref uint data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        int newData = EditorGUILayout.IntField((int)data);

        if (newData >= 0)
            data = (uint)newData;

        EditorGUILayout.EndHorizontal();
    }

    private void GameObjectFieldWithLabel(string label, ref GameObject data, float offset = 0)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(offset);
        EditorGUILayout.LabelField(label, EditorStyles.label);
        data = EditorGUILayout.ObjectField(data, typeof(GameObject), false) as GameObject;
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

    private void AttackListField(string label, ref List<AttackData> data)
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

                data.Add(new AttackData($"attack{ DateTime.Now.ToString()}"));
            }

            EditorGUILayout.LabelField("   |   ", EditorStyles.label, GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("  |   ")).x));

            if (GUILayout.Button("Clear", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Clear")).x)))
            {
                m_attackFoldout?.Clear();
                data?.Clear();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private bool AttackField(int dataID, ref List<AttackData> data, float offset = 10)
    {
        if (m_attackFoldout == null)
            return true;

        if (!m_attackFoldout.ContainsKey(data[dataID].name))
            m_attackFoldout.Add(data[dataID].name, false);

        GUILayout.BeginHorizontal();
        GUILayout.Space(offset);

        m_attackFoldout[data[dataID].name] = EditorGUILayout.Foldout(m_attackFoldout[data[dataID].name], data[dataID].name);
        string key = data[dataID].name;
        GUILayout.EndHorizontal();

        if (m_attackFoldout[data[dataID].name])
        {

            TextFieldWithLabel("Name: ", ref data[dataID].name, offset);
            if (key != data[dataID].name) //has name changed?
            {
                //remove data at key
                m_attackFoldout.Remove(data[dataID].name);
                m_attackFoldout.Add(data[dataID].name, true);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(offset);
            EditorGUILayout.LabelField("Type: ");
            data[dataID].attackType = (AttackData.AttackType)EditorGUILayout.EnumPopup(data[dataID].attackType);
            GUILayout.EndHorizontal();

            UnsignedIntegerFieldWithLabel("Animation ID: ", ref data[dataID].animID, offset);
            FloatFieldWithLabel("Base Damage: ", ref data[dataID].baseDamage, offset);
            UnsignedIntegerFieldWithLabel("Instances: ", ref data[dataID].instancesPerAttack, offset);
            FloatFieldWithLabel("Attack Range: ", ref data[dataID].attackRange, offset);
            FloatFieldWithLabel("Cooldown: ", ref data[dataID].cooldown, offset);
            UnsignedIntegerFieldWithLabel("Priority: ", ref data[dataID].priority, offset);


            EditorGUILayout.Space();
            switch (data[dataID].attackType)
            {
                default:
                case AttackData.AttackType.Melee:
                    Vector3FieldWithLabel("AttackOriginOffset: ", ref data[dataID].attackOriginOffset, offset);
                    break;
                case AttackData.AttackType.Ranged:
                    GameObjectFieldWithLabel("Projectile prefab: ", ref data[dataID].projectile, offset);
                    FloatFieldWithLabel("Projectile Speed: ", ref data[dataID].projSpeed, offset);
                    FloatFieldWithLabel("Projectile LifeTime: ", ref data[dataID].projLifeTime, offset);
                    break;
                case AttackData.AttackType.Instant:
                    GameObjectFieldWithLabel("Projectile prefab: ", ref data[dataID].projectile, offset);
                    Vector3FieldWithLabel("AttackOriginOffset: ", ref data[dataID].attackOriginOffset, offset);
                    FloatFieldWithLabel("Projectile LifeTime: ", ref data[dataID].projLifeTime, offset);
                    break;
            }
            EditorGUILayout.Space();
            GameObjectFieldWithLabel("VFX prefab: ", ref data[dataID].vfxSpawn, offset);

            GUILayout.BeginHorizontal();
            GUILayout.Space(offset);

            if (GUILayout.Button("Remove", EditorStyles.linkLabel, GUILayout.Width(EditorStyles.linkLabel.CalcSize(new GUIContent("Remove")).x)))
            {
                m_attackFoldout.Remove(data[dataID].name);
                m_data.m_attacks.Remove(data[dataID]);
                GUILayout.EndHorizontal();
                return false;
            }
            GUILayout.EndHorizontal();
        }
        return true;
    }
}
