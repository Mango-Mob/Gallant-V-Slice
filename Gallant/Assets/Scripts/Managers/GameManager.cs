using ActorSystem.AI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static float currentLevel = 0;
    public static float deltaLevel = 1/3f;
    
    public static Vector2 m_sensitivity = new Vector2(-400.0f, -250.0f);

    public GameObject m_player;
    public Camera m_activeCamera;
    internal bool enableTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = m_player.GetComponentInChildren<Camera>();

        for (int i = 0; i < 31; i++)
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Water"), i);
        
    }

    // Update is called once per frame
    void Update()
    {
        int gamepadID = InputManager.Instance.GetAnyGamePad();
    }

    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    public void FinishLevel()
    {
        //TODO
        Debug.Log("Game over");
    }

    public static void Advance()
    {
        currentLevel += deltaLevel;
    }

    #region Player Info Storage
    [Serializable]
    private struct PlayerInfo
    {
        public WeaponData m_leftWeapon;
        public WeaponData m_rightWeapon;
        public ClassData m_classData;
         
        //public Dictionary<EffectData, int> m_effects;
        public List<EffectsInfo> m_effects;
    }

    [Serializable]
    private struct EffectsInfo
    {
        public EffectData effect;
        public int amount;
    }


    static public bool m_containsPlayerInfo = false;
    static private PlayerInfo m_playerInfo;

    public static void SavePlayerInfoToFile()
    {
        Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();

        string json = JsonUtility.ToJson(m_playerInfo);
        File.WriteAllText(Application.persistentDataPath + "/playerInfo.json", json);
    }
    public static void LoadPlayerInfoFromFile()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.json"))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(Application.persistentDataPath + "/playerInfo.json");

            // Pass the json to JsonUtility, and tell it to create a PlayerInfo object from it
            m_playerInfo = JsonUtility.FromJson<PlayerInfo>(dataAsJson);

            m_containsPlayerInfo = true;
        }

        Instance.m_player.GetComponent<Player_Controller>().LoadPlayerInfo();
    }
    public static void StorePlayerInfo(WeaponData _leftWeapon, WeaponData _rightWeapon, Dictionary<EffectData, int> _effects, ClassData _class)
    {
        m_playerInfo.m_leftWeapon = _leftWeapon;
        m_playerInfo.m_rightWeapon = _rightWeapon;

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

        m_containsPlayerInfo = true;
    }

    public static WeaponData RetrieveWeaponData(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                return m_playerInfo.m_leftWeapon;
            case Hand.RIGHT:
                return m_playerInfo.m_rightWeapon;
            default:
                return null;
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

    public static void ResetPlayerInfo()
    {
        m_playerInfo.m_leftWeapon = null;
        m_playerInfo.m_rightWeapon = null;

        m_playerInfo.m_effects = null;

        m_playerInfo.m_classData = null;

        m_containsPlayerInfo = false;
    }

    #endregion
}
