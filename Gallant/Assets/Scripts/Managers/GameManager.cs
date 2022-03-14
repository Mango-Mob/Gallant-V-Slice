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
    public static float deltaLevel = 1/2f;
    
    public static Vector2 m_sensitivity = new Vector2(-400.0f, -250.0f);
    
    public GameObject m_player;
    public Camera m_activeCamera;
    public bool enableTimer = false;

    public AtmosphereScript music { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_activeCamera = m_player.GetComponentInChildren<Camera>();
        music = GetComponentInChildren<AtmosphereScript>();

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
        public SerializedWeapon m_leftWeapon;
        public AbilityData m_leftAbility;

        public SerializedWeapon m_rightWeapon;
        public AbilityData m_rightAbility;

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
        if (_leftWeapon != null)
        {
            m_playerInfo.m_leftWeapon = SerializedWeapon.SerializeWeapon(_leftWeapon);
            m_playerInfo.m_leftAbility = _leftWeapon.abilityData;
        }
        else
        {
            m_playerInfo.m_leftWeapon = null;
            m_playerInfo.m_leftAbility = null;
        }

        if (_rightWeapon != null)
        {
            m_playerInfo.m_rightWeapon = SerializedWeapon.SerializeWeapon(_rightWeapon);
            m_playerInfo.m_rightAbility = _rightWeapon.abilityData;
        }
        else
        {
            m_playerInfo.m_rightWeapon = null;
            m_playerInfo.m_rightAbility = null;
        }

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

        m_containsPlayerInfo = true;
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
