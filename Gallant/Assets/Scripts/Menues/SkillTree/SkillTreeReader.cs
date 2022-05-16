using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Skill
{
    public string id;
    public int upgradeLevel;
}

[System.Serializable]
public class SkillTree
{
    public List<Skill> skills;
}

public class SkillTreeReader : MonoBehaviour
{
    private static SkillTreeReader _instance;

    public static SkillTreeReader instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject loader = GameObject.Instantiate(Resources.Load<GameObject>("SkillTreeReader"));
                _instance = loader.GetComponent<SkillTreeReader>();
                loader.name = "SkillTreeReader";
                return _instance;
            }
            return _instance;
        }
    }

    // Array with all the skills in our skilltree
    private SkillTree m_skillTree;

    // Dictionary with the skills in our skilltree
    public List<SkillTree> m_skills { get; private set; } = new List<SkillTree>();
    //public Dictionary<string, Skill> m_basicSkills { get; private set; }
    //public Dictionary<string, Skill> m_knightSkills { get; private set; }
    //public Dictionary<string, Skill> m_mageSkills { get; private set; }
    //public Dictionary<string, Skill> m_hunterSkills { get; private set; }
    //public InkmanClass m_treeClass { get; private set; }


    // Variable for caching the currently being inspected skill
    private Skill m_currentSkill;

    //public int availablePoints = 100;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            SetUpSkillTrees();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization of the skill tree
    void SetUpSkillTrees()
    {
        for (int i = 0; i < 4; i++)
        {
            m_skills.Add(new SkillTree());
        }

        LoadSkillTree(InkmanClass.GENERAL);
        LoadSkillTree(InkmanClass.KNIGHT);
        LoadSkillTree(InkmanClass.MAGE);
        LoadSkillTree(InkmanClass.HUNTER);
    }
    private string GetSkillTreePath(InkmanClass _class)
    {
        string path = Application.persistentDataPath;
        switch (_class)
        {
            case InkmanClass.GENERAL:
                path += $"/saveSlot{GameManager.m_saveSlotInUse}/generalTree.json";
                break;
            case InkmanClass.KNIGHT:
                path += $"/saveSlot{GameManager.m_saveSlotInUse}/knightTree.json";
                break;
            case InkmanClass.MAGE:
                path += $"/saveSlot{GameManager.m_saveSlotInUse}/mageTree.json";
                break;
            case InkmanClass.HUNTER:
                path += $"/saveSlot{GameManager.m_saveSlotInUse}/hunterTree.json";
                break;
        }

        return path;
    }

    public void LoadSkillTree(InkmanClass _class)
    {
        if (m_skills[(int)_class].skills != null)
            m_skills[(int)_class].skills.Clear();

        string path = GetSkillTreePath(_class);

        string dataAsJson;
        if (File.Exists(path))
        {
            // Read the json from the file into a string
            
            dataAsJson = File.ReadAllText(path);

            // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
            SkillTree loadedData = JsonUtility.FromJson<SkillTree>(dataAsJson);

            m_skills[(int)_class] = loadedData;

            if (m_skills[(int)_class].skills == null)
            {
                m_skills[(int)_class].skills = new List<Skill>();
            }
        }
        else
        {
            if (!Directory.Exists(Application.persistentDataPath + $"/saveSlot{ GameManager.m_saveSlotInUse}/"))
                Directory.CreateDirectory(Application.persistentDataPath + $"/saveSlot{ GameManager.m_saveSlotInUse}/");

            File.WriteAllText(GetSkillTreePath(_class), JsonUtility.ToJson(m_skills[(int)_class], true));

            Debug.Log("Created new skill tree file!");

            if (m_skills[(int)_class].skills == null)
            {
                m_skills[(int)_class].skills = new List<Skill>();
            }
        }
    }

    public void SaveSkillTree(InkmanClass _class)
    {
        string json = JsonUtility.ToJson(m_skills[(int)_class], true);
        File.WriteAllText(GetSkillTreePath(_class), json);
    }


    public int GetUpgradeLevel(InkmanClass _class, string id_skill)
    {
        foreach (var skill in m_skills[(int)_class].skills)
        {
            if (skill.id == id_skill)
            {
                return skill.upgradeLevel;
            }
        }

        return -1;
    }

    public SkillTree GetSkillTree(InkmanClass _class)
    {
        return m_skills[(int)_class];
    }

    public void UnlockSkill(InkmanClass _class, string id_skill)
    {
        foreach (var skill in m_skills[(int)_class].skills)
        {
            if (skill.id == id_skill)
            {
                skill.upgradeLevel++;
                SaveSkillTree(_class);

                return;
            }
        }

        Skill newSkill = new Skill();
        newSkill.id = id_skill;
        newSkill.upgradeLevel = 1;

        m_skills[(int)_class].skills.Add(newSkill);

        SaveSkillTree(_class);
    }

    public void EmptySkillTree(InkmanClass _class)
    {
        if (m_skills[(int)_class] != null && m_skills[(int)_class].skills != null)
            m_skills[(int)_class].skills.Clear();

        SaveSkillTree(_class);
    }
    public void EmptyAllTrees()
    {
        foreach (var playerClass in System.Enum.GetValues(typeof(InkmanClass)))
        {
            EmptySkillTree((InkmanClass)playerClass);
        }
    }

    //public bool CanSkillBeUnlocked(int id_skill)
    //{
    //    bool canUnlock = true;
    //    if (m_skills.TryGetValue(id_skill, out m_currentSkill)) // The skill exists
    //    {
    //        if (m_currentSkill.cost <= availablePoints) // Enough points available
    //        {
    //            int[] dependencies = m_currentSkill.dependencies;
    //            for (int i = 0; i < dependencies.Length; ++i)
    //            {
    //                if (m_skills.TryGetValue(dependencies[i], out m_currentSkill))
    //                {
    //                    if (!m_currentSkill.unlocked)
    //                    {
    //                        canUnlock = false;
    //                        break;
    //                    }
    //                }
    //                else // If one of the dependencies doesn't exist, the skill can't be unlocked.
    //                {
    //                    return false;
    //                }
    //            }
    //        }
    //        else // If the player doesn't have enough skill points, can't unlock the new skill
    //        {
    //            return false;
    //        }

    //    }
    //    else // If the skill id doesn't exist, the skill can't be unlocked
    //    {
    //        return false;
    //    }
    //    return canUnlock;
    //}
}