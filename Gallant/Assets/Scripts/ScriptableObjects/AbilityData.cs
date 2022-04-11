using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
public enum AbilityTag
{
    Fire,
    Air,
    Earth,
    Water,
}


/****************
 * AbilityData: Scriptable object containing data for ability
 * @author : William de Beer
 * @file : AbilityData.cs
 * @year : 2021
 */
[CreateAssetMenu(fileName = "abilityData", menuName = "Game Data/Ability Data", order = 1)]
[System.Serializable]
public class AbilityData : ScriptableObject
{
    [Header("Ability Information")]
    public string abilityName;
    public string weaponTitle; //_ <weapon> of <title>
    public Ability abilityPower;
    public Sprite abilityIcon;
    public AbilityTag[] m_tags;
    [TextArea(10, 15)]
    public string description;
    [ColorUsage(true, true)] public Color droppedEnergyColor;

    [Space(10)]
    [Range(1, 3)]
    public int starPowerLevel = 1;
    [Header("Ability Stats")]
    [Tooltip("The time between uses of the ability.")]
    public float cooldownTime = 1.0f; // The time between uses of the ability.
    [Tooltip("The impact damage of the ability (does not apply to all abilities).")]
    public float damage = 0.0f; // The impact damage of the ability (does not apply to all abilities)
    [Tooltip("The lifetime of the instance spawned by the cast (does not apply to all abilities).")]
    public float lifetime = 0.0f; // The lifetime of the instance spawned by the cast (does not apply to all abilities)
    [Tooltip("The effectiveness of any buff or status effect applied to the target. (does not apply to all abilities).")]
    public float effectiveness = 0.0f; // The effectiveness of any buff or status effect applied to the target. (does not apply to all abilities)
    [Tooltip("The duration of any buff or status effect applied to the target. (does not apply to all abilities).")]
    public float duration = 0.0f; // The duration of any buff or status effect applied to the target. (does not apply to all abilities)

    [HideInInspector] public float lastCooldown = 0.0f;
    public bool isPassive = false;

    public Sprite overwriteStaffIcon;
    public static string EvaluateDescription(AbilityData data)
    {
        string description = data.description;
        int nextIndex = description.IndexOf('%');

        if (nextIndex == -1)
            return description;

        //Loop through all instances of %, while extending up the string
        for (int i = nextIndex; i < description.Length && i != -1; i = nextIndex)
        {
            string before = description.Substring(0, i);
            string insert = "";
            int indexOfDecimal = -1;
            string after = description.Substring(i + 2);
            switch (description[i+1])
            {
                case 'd': //%d = damage
                    insert = Mathf.FloorToInt(data.damage).ToString();
                    break;
                case 'l':
                    string life = data.lifetime.ToString();
                    indexOfDecimal = life.IndexOf('.') + 2;
                    insert = (indexOfDecimal != -1 ? life.Substring(0, indexOfDecimal) : life);
                    break;
                case 'e':
                    string effect = data.effectiveness.ToString();
                    indexOfDecimal = effect.IndexOf('.') + 2;
                    insert = (indexOfDecimal != -1 ? effect.Substring(0, indexOfDecimal) : effect);
                    break;
                case 'p':
                    string percentage = (data.effectiveness * 100.0f).ToString();

                    indexOfDecimal = percentage.IndexOf('.');
                    insert = (indexOfDecimal != -1 ? percentage.Substring(0, indexOfDecimal) : percentage);
                    break;
                case 't':
                    string time = data.duration.ToString();
                    indexOfDecimal = time.IndexOf('.') + 2;
                    insert = (indexOfDecimal != -1 ? time.Substring(0, indexOfDecimal) : time);
                    break;
                case '%':
                    insert = "%";
                    break;
                default:
                    Debug.LogError($"Evaluation Description: Char not supported: {description[i + 1]}");
                    break;
            }
            description = string.Concat(before, insert, after);
            nextIndex = description.IndexOf('%', nextIndex + insert.Length);
        }

        return description;
    }

    public static AbilityData CreateWeaponDataSynergy(AbilityData _data1, AbilityData _data2)
    {
        AbilityData data = CreateInstance<AbilityData>();

        data.damage = (_data1.damage + _data2.damage) * 0.75f;
        data.lifetime = (_data1.lifetime + _data2.lifetime) * 0.75f;
        data.effectiveness = (_data1.effectiveness + _data2.effectiveness) * 0.75f;
        data.duration = (_data1.duration + _data2.duration) * 0.75f;

        return data;
    }
    public void Clone(AbilityData other)
    {
        this.abilityName = other.abilityName;
        this.weaponTitle = other.weaponTitle;
        this.abilityPower = other.abilityPower;
        this.abilityIcon = other.abilityIcon;
        this.description = other.description;
        
        this.starPowerLevel = other.starPowerLevel;
        this.cooldownTime = other.cooldownTime; 
        this.damage = other.damage; 
        this.lifetime = other.lifetime; 
        this.effectiveness = other.effectiveness;
        this.duration = other.duration;
        
        this.lastCooldown = other.lastCooldown;
        this.isPassive = other.isPassive;

        this.overwriteStaffIcon = other.overwriteStaffIcon;
    }
}

#if UNITY_EDITOR
/****************
 * WeaponDataEditor: Reorderable list for tags
 * @author : William de Beer
 * @file : WeaponData.cs
 * @year : 2022
 */
[CustomEditor(typeof(AbilityData), true)]
public class AbilityDataEditor : Editor
{
    SerializedProperty panelComponent;
    ReorderableList list;

    private void OnEnable()
    {
        panelComponent = serializedObject.FindProperty("m_tag");

        list = new ReorderableList(serializedObject, panelComponent, true, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
    }

    //Draws the items stored in the reorderable list.
    void DrawListItems(Rect _rect, int _index, bool _isActive, bool _isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(_index);

        if (element != null && element.objectReferenceValue != null)
        {
            EditorGUI.LabelField(new Rect(_rect.x, _rect.y, 400, EditorGUIUtility.singleLineHeight), element.objectReferenceValue.name);
        }
        else if (element.objectReferenceValue == null)
        {
            Debug.LogWarning("Could not find object associated with serialized property.");
        }
    }

    //Draws the header of the reorderable list.
    void DrawHeader(Rect _rect)
    {
        string name = "Objects";
        EditorGUI.LabelField(_rect, name);
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Draw list
        serializedObject.Update();
        if (panelComponent != null)
            list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif