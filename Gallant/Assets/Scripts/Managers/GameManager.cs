using ActorSystem.AI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static float currentLevel = 0;
    public static float deltaLevel = 1.25f;
    public List<LevelData> m_comapare;
    public static Vector2 m_sensitivity = new Vector2(-400.0f, -250.0f);

    public GameObject m_player;
    public Camera m_activeCamera;
    public bool enableTimer = false;
    public bool m_sceneHasTutorial = false;
    public bool IsInCombat { get { return ActorManager.Instance.m_activeSpawnners.Count > 0; } }
    public int clearedArenas = 0;

    // Run Info 
    public static bool m_runActive = false;
    public static float m_runTime = 0.0f;
    public static int m_killCount = 0;
    public static float m_damageDealt = 0.0f;
    public static float m_healingDealt = 0.0f;

    public AtmosphereScript music { get; private set; }
    public float m_deathDelay = 1.0f;

    public static bool m_joystickCursorEnabled = false;

    protected override void Awake()
    {
        base.Awake();
        if (m_saveInfo == null) { m_saveInfo = new SaveInfo(); }
        Debug.Log("Init bruv");
    }
    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
        music = GetComponentInChildren<AtmosphereScript>();

        for (int i = 0; i < 31; i++)
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Water"), i);

        m_sceneHasTutorial = FindObjectOfType<TutorialManager>() != null;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance == null)
            return;

        int gamepadID = InputManager.Instance.GetAnyGamePad();
        
        if(!m_sceneHasTutorial && m_player.GetComponent<Player_Controller>().playerResources.m_dead)
        {
            m_deathDelay -= Time.deltaTime;
            if(m_deathDelay <= 0)
            {
                LevelManager.Instance.LoadNewLevel("EndScreen", LevelManager.Transition.YOUDIED);
            }
        }

        Cursor.visible = !InputManager.Instance.isInGamepadMode && !m_joystickCursorEnabled;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = Camera.main;
    }

    public void FinishLevel()
    {
        LevelManager.Instance.LoadHubWorld(false);
    }

    public static void Advance()
    {
        GameManager.currentLevel += GameManager.deltaLevel;
        GameManager.Instance.clearedArenas++;
        PlayerPrefs.SetFloat("Level", GameManager.currentLevel);
    }

    public static void ResetRunInfo()
    {
        m_runTime = 0.0f;
        m_killCount = 0;
        m_damageDealt = 0.0f;
        m_healingDealt = 0.0f;
    }
    public static string CalculateTimerString(float _time)
    {
        int seconds = Mathf.FloorToInt(_time % 60);
        int minutes = Mathf.FloorToInt((_time / 60) % 60);
        int hours = Mathf.FloorToInt(_time / 3600);

        return $"{hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";
    }
    public static string CalculateTimeUnitString(float _time)
    {
        float minutes;
        float hours;

        if (_time > 60)
        {
            if (_time > 60 * 60)
            {
                hours = _time / (60 * 60);
                return $"{hours.ToString().Substring(0, 4)} hours";
            }
            minutes = _time / 60;
            return $"{minutes.ToString().Substring(0, 4)} minutes";
        }
        return $"{_time.ToString().Substring(0, 4)} seconds";
    }
    public static string CalculateNumberUnit(float _value)
    {
        int multiples = 0;
        float tempVal = _value;

        while (tempVal > 1000f)
        {
            tempVal /= 1000f;
            multiples++;
        }

        switch (multiples)
        {
            default:
            case 0:
                return $"{_value.ToString()}";
            case 1:
                return $"{_value.ToString()}k";
            case 2:
                return $"{_value.ToString()}M";
            case 3:
                return $"{_value.ToString()}B";
        }
    }


    public static int m_saveSlotInUse = 1;
    #region Player Info Storage
    [Serializable]
    private struct PlayerInfo
    {
        public bool m_validSave;

        public SerializedWeapon m_leftWeapon;
        public AbilityData m_leftAbility;

        public SerializedWeapon m_rightWeapon;
        public AbilityData m_rightAbility;

        public ClassData m_classData;

        public ItemEffect m_leftWeaponEffect;
        public ItemEffect m_rightWeaponEffect;

        public Color m_leftOutlineColor;
        public Color m_rightOutlineColor;

        public List<EffectsInfo> m_effects;

        public float m_health;
        public int m_orbs;
    }

    [Serializable]
    private struct EffectsInfo
    {
        public EffectData effect;
        public int amount;
    }


    static public bool m_containsPlayerInfo = false;
    static private PlayerInfo m_playerInfo;

    public static void ClearPlayerInfoFromFile()
    {
        m_playerInfo = new PlayerInfo();
        m_containsPlayerInfo = false;
        m_playerInfo.m_validSave = false;
        currentLevel = 0;
        string json = JsonUtility.ToJson(m_playerInfo, true);
        if (File.Exists(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/playerInfo.json"))
            File.WriteAllText(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/playerInfo.json", json);
    }
    public static void SavePlayerInfoToFile()
    {
        if (Instance?.m_player == null)
        {
            Debug.LogError("Save player info should not be called here. Only if there is a player in the scene. Contact William de Beer for more info.");
            return;
        }

        Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();
        m_playerInfo.m_validSave = true;

        string json = JsonUtility.ToJson(m_playerInfo, true);

        if (!Directory.Exists(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/"))
            Directory.CreateDirectory(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/");

        File.WriteAllText(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/playerInfo.json", json);

        SaveSaveInfoToFile();
    }
    public static void LoadPlayerInfoFromFile()
    {
        if (File.Exists(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/playerInfo.json"))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/playerInfo.json");

            // Pass the json to JsonUtility, and tell it to create a PlayerInfo object from it
            m_playerInfo = JsonUtility.FromJson<PlayerInfo>(dataAsJson);

            m_containsPlayerInfo = true;
        }

        if (Instance?.m_player != null)
            Instance.m_player.GetComponent<Player_Controller>().LoadPlayerInfo();
    }
    public static void StorePlayerInfo(WeaponData _leftWeapon, WeaponData _rightWeapon, Dictionary<EffectData, int> _effects, ClassData _class, ItemEffect _leftWeaponEffect, ItemEffect _rightWeaponEffect, float _health, int _orbs)
    {
        if (_leftWeapon != null)
        {
            m_playerInfo.m_leftWeapon = SerializedWeapon.SerializeWeapon(_leftWeapon);
            m_playerInfo.m_leftAbility = _leftWeapon.abilityData;
            if (m_playerInfo.m_leftAbility)
                m_playerInfo.m_leftOutlineColor = m_playerInfo.m_leftAbility.droppedEnergyColor;
        }
        else
        {
            m_playerInfo.m_leftWeapon = SerializedWeapon.SerializeWeapon(_leftWeapon); ;
            m_playerInfo.m_leftAbility = null;
        }

        if (_rightWeapon != null)
        {
            m_playerInfo.m_rightWeapon = SerializedWeapon.SerializeWeapon(_rightWeapon);
            m_playerInfo.m_rightAbility = _rightWeapon.abilityData;
            if (m_playerInfo.m_rightAbility)
                m_playerInfo.m_rightOutlineColor = m_playerInfo.m_rightAbility.droppedEnergyColor;
        }
        else
        {
            m_playerInfo.m_rightWeapon = SerializedWeapon.SerializeWeapon(_rightWeapon); ;
            m_playerInfo.m_rightAbility = null;
        }

        m_playerInfo.m_leftWeaponEffect = _leftWeaponEffect;
        m_playerInfo.m_rightWeaponEffect = _rightWeaponEffect;

        //if (_rightWeapon != null)
        //{
        //    m_playerInfo.m_rightWeapon = WeaponData.CreateInstance<WeaponData>();
        //    m_playerInfo.m_rightWeapon.Clone(_rightWeapon);
        //    m_playerInfo.m_rightAbility = _rightWeapon.abilityData;
        //    WeaponData.ApplyAbilityData(m_playerInfo.m_rightWeapon, Ability.NONE, 0);
        //}
        //else
        //{
        //    m_playerInfo.m_rightWeapon = null;
        //    m_playerInfo.m_rightAbility = null;
        //}

        //m_playerInfo.m_rightWeapon.Clone(_rightWeapon);
        //m_playerInfo.m_rightAbility = _rightWeapon.abilityData;
        //WeaponData.ApplyAbilityData(m_playerInfo.m_rightWeapon, Ability.NONE, 0);

        if (m_playerInfo.m_effects != null)
            m_playerInfo.m_effects.Clear();
        else
            m_playerInfo.m_effects = new List<EffectsInfo>();

        foreach (var effect in _effects)
        {
            EffectsInfo effectsInfo;
            effectsInfo.effect = effect.Key;
            effectsInfo.amount = effect.Value;
            m_playerInfo.m_effects.Add(effectsInfo);
        }

        //m_playerInfo.m_effects = _effects;

        m_playerInfo.m_classData = _class;

        m_playerInfo.m_health = _health;
        m_playerInfo.m_orbs = _orbs;

        m_containsPlayerInfo = true;
    }

    public static bool RetrieveValidSaveState()
    {
        return m_playerInfo.m_validSave;
    }
    public static WeaponData RetrieveWeaponData(Hand _hand)
    {
        WeaponData data = null;
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_playerInfo.m_leftWeapon != null)
                    data = SerializedWeapon.DeserializeWeapon(m_playerInfo.m_leftWeapon);
                else
                    return null;
                break;
            case Hand.RIGHT:
                if (m_playerInfo.m_rightWeapon != null)
                    data = SerializedWeapon.DeserializeWeapon(m_playerInfo.m_rightWeapon);
                else
                    return null;
                break;
            default:
                return null;
        }
        return data;
    }
    public static AbilityData RetrieveAbilityData(Hand _hand)
    {
        //switch (_hand)
        //{
        //    case Hand.LEFT:
        //        return m_playerInfo.m_leftAbility;
        //    case Hand.RIGHT:
        //        return m_playerInfo.m_rightAbility;
        //    default:
        //        return null;
        //}
        AbilityData data = AbilityData.CreateInstance<AbilityData>();
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_playerInfo.m_leftAbility)
                    data.Clone(m_playerInfo.m_leftAbility);
                else
                    return null;
                break;
            case Hand.RIGHT:
                if (m_playerInfo.m_rightAbility)
                    data.Clone(m_playerInfo.m_rightAbility);
                else
                    return null;
                break;
            default:
                return null;
        }
        return data;
    }
    
    public static ItemEffect RetrieveWeaponEffect(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                return m_playerInfo.m_leftWeaponEffect;
            case Hand.RIGHT:
                return m_playerInfo.m_rightWeaponEffect;
            default:
                return ItemEffect.NONE;
        }
    }
    public static Color RetrieveOutlineColor(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                return m_playerInfo.m_leftOutlineColor;
            case Hand.RIGHT:
                return m_playerInfo.m_rightOutlineColor;
            default:
                return Color.clear;
        }
    }

    public static Dictionary<EffectData, int> RetrieveEffectsDictionary()
    {
        if (m_playerInfo.m_effects == null)
        {
            m_playerInfo.m_effects = new List<EffectsInfo>();
            return new Dictionary<EffectData, int>();
        }
        else
        {
            Dictionary<EffectData, int> effects = new Dictionary<EffectData, int>();

            foreach (var effect in m_playerInfo.m_effects)
            {
                effects.Add(effect.effect, effect.amount);
            }

            return effects;
        }
    }

    public static ClassData RetrieveClassData()
    {
        return m_playerInfo.m_classData;
    }

    public static float RetrieveHealth()
    {
        return m_playerInfo.m_health;
    }
    public static int RetrieveOrbCount()
    {
        return m_playerInfo.m_orbs;
    }
    public static void ResetPlayerInfo()
    {
        m_playerInfo.m_leftWeapon = null;
        m_playerInfo.m_rightWeapon = null;

        m_playerInfo.m_effects = null;

        m_playerInfo.m_classData = null;

        m_containsPlayerInfo = false;

        PlayerPrefs.SetFloat("Level", 0);
    }

    #endregion

    #region Run Info Storage
    [Serializable]
    public class SaveInfo
    {
        public bool m_validSave;
        // ******
        // Put desired stored variables here!
        // V V V V V V

        public int m_testValue;

        //LEVELS
        public int m_completedTutorial = 0;
        public int m_completedSwamp = 0;
        public int m_completedCastle = 0;

        //NPCs
        public int m_rowanVisits = 0;
        public int m_perceptionVisits = 0;

        // Ʌ Ʌ Ʌ Ʌ Ʌ Ʌ
    }

    static public bool m_containsRunInfo = false;
    static public SaveInfo m_saveInfo;

    public static void ClearSaveInfoFromFile()
    {
        m_saveInfo = new SaveInfo();
        m_containsRunInfo = false;
        m_saveInfo.m_validSave = false;
        string json = JsonUtility.ToJson(m_saveInfo, true);
        if (File.Exists(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/playerInfo.json"))
            File.WriteAllText(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/saveInfo.json", json);
    }
    public static void SaveSaveInfoToFile()
    {
        m_saveInfo.m_validSave = true;

        if (!Directory.Exists(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/"))
            Directory.CreateDirectory(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/");

        string json = JsonUtility.ToJson(m_saveInfo, true);
        File.WriteAllText(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/saveInfo.json", json);
    }
    public static void LoadSaveInfoFromFile()
    {
        Debug.Log("Load");

        m_saveInfo.m_validSave = false;
        if (File.Exists(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/saveInfo.json"))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(Application.persistentDataPath + $"/saveSlot{m_saveSlotInUse}/saveInfo.json");

            // Pass the json to JsonUtility, and tell it to create a RunInfo object from it
            m_saveInfo = JsonUtility.FromJson<SaveInfo>(dataAsJson);

            m_containsPlayerInfo = true;
        }
    }
    #endregion
}
