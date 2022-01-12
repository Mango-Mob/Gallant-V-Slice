using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class Skill
{
    public string id;
    public int upgradeLevel;
    public int cost;
}

[System.Serializable]
public class SkillTree
{
    public Skill[] skilltree;
}

public class SkillTreeReader : MonoBehaviour
{
    private static SkillTreeReader _instance;

    public static SkillTreeReader Instance
    {
        get
        {
            return _instance;
        }
        set
        {
        }
    }

    // Array with all the skills in our skilltree
    private Skill[] m_skillTree;

    // Dictionary with the skills in our skilltree
    public List<Dictionary<string, Skill>> m_skills { get; private set; }
    public Dictionary<string, Skill> m_basicSkills { get; private set; }
    public Dictionary<string, Skill> m_knightSkills { get; private set; }
    public Dictionary<string, Skill> m_mageSkills { get; private set; }
    public Dictionary<string, Skill> m_hunterSkills { get; private set; }
    //public InkmanClass m_treeClass { get; private set; }


    // Variable for caching the currently being inspected skill
    private Skill m_currentSkill;

    public int availablePoints = 100;

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
            m_skills.Add(new Dictionary<string, Skill>());
        }

        LoadSkillTree(InkmanClass.GENERAL);
        LoadSkillTree(InkmanClass.KNIGHT);
        LoadSkillTree(InkmanClass.MAGE);
        LoadSkillTree(InkmanClass.HUNTER);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private string GetSkillTreePath(InkmanClass _class)
    {
        string path = Application.persistentDataPath;
        switch (_class)
        {
            case InkmanClass.GENERAL:
                path += "Data/SkillTrees/generalTree.json";
                break;
            case InkmanClass.KNIGHT:
                path += "Data/SkillTrees/knightTree.json";
                break;
            case InkmanClass.MAGE:
                path += "Data/SkillTrees/mageTree.json";
                break;
            case InkmanClass.HUNTER:
                path += "Data/SkillTrees/hunterTree.json";
                break;
        }

        return path;
    }

    public void LoadSkillTree(InkmanClass _class)
    {
        m_skills[(int)_class].Clear();

        string path = GetSkillTreePath(_class);

        string dataAsJson;
        if (File.Exists(path))
        {
            // Read the json from the file into a string
            
            dataAsJson = File.ReadAllText(path);

            // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
            SkillTree loadedData = JsonUtility.FromJson<SkillTree>(dataAsJson);

            // Store the SkillTree as an array of Skill
            m_skillTree = new Skill[loadedData.skilltree.Length];
            m_skillTree = loadedData.skilltree;

            // Populate a dictionary with the skill id and the skill data itself
            for (int i = 0; i < m_skillTree.Length; ++i)
            {
                m_skills[(int)_class].Add(m_skillTree[i].id, m_skillTree[i]);
            }
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    public void SaveSkillTree(InkmanClass _class)
    {
        File.WriteAllText(GetSkillTreePath(_class), JsonUtility.ToJson(m_skills[(int)_class]));
    }

    public int GetUpgradeLevel(InkmanClass _class, string id_skill)
    {
        if (m_skills[(int)_class].TryGetValue(id_skill, out m_currentSkill))
        {
            return m_currentSkill.upgradeLevel;
        }
        else
        {
            return -1;
        }
    }

    public bool UnlockSkill(InkmanClass _class, string id_Skill)
    {
        if (m_skills[(int)_class].TryGetValue(id_Skill, out m_currentSkill))
        {
            if (m_currentSkill.cost <= availablePoints)
            {
                availablePoints -= m_currentSkill.cost;
                m_currentSkill.upgradeLevel++;

                // We replace the entry on the dictionary with the new one (already unlocked)
                m_skills[(int)_class].Remove(id_Skill);
                m_skills[(int)_class].Add(id_Skill, m_currentSkill);

                return true;
            }
            else
            {
                return false;   // The skill can't be unlocked. Not enough points
            }
        }
        else
        {
            return false;   // The skill doesn't exist
        }
    }

    public void EmptySkillTree(InkmanClass _class)
    {
        m_skills[(int)_class].Clear();

        LoadSkillTree(_class);
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