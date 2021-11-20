using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Application_Singleton
    public static float currentLevel = 0;
    public static float deltaLevel = 0.25f;
    private static GameManager _instance = null;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject loader = new GameObject();
                _instance = loader.AddComponent<GameManager>();
                return loader.GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_instance == this)
        {
            InitialiseFunc();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Second Instance of GameManager was created, this instance was destroyed.");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    #endregion 

    public static Vector2 m_sensitivity = new Vector2(-400.0f, -250.0f);

    public GameObject m_player;

    internal bool enableTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined;
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //float currentTime = Time.timeScale;
        //currentTime += Time.deltaTime;
        //currentTime = Mathf.Clamp(currentTime, 0.0f, 1.0f);
        //Time.timeScale = currentTime;

        int gamepadID = InputManager.instance.GetAnyGamePad();
    }

    private void OnLevelWasLoaded(int level)
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    private void InitialiseFunc()
    {
        gameObject.name = "Game Manager";
    }
    private void TimeUpdate()
    {
        
    }
    public void SlowTime(float _percentage, float _duration)
    {
        StartCoroutine(SlowTimeRoutine(_percentage, _duration));
    }
    private IEnumerator SlowTimeRoutine(float _percentage, float _duration)
    {
        
        Time.timeScale = _percentage;
        AudioManager.instance.m_globalPitch = Time.timeScale;
        Time.fixedDeltaTime = 0.02f * _percentage;
        float rate = _duration / (1f - _percentage);
        while (_duration > 0)
        {
            _duration -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
            AudioManager.instance.m_globalPitch = Time.timeScale;
            Time.timeScale += rate * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = 0.02f * _percentage;
        }
        Time.timeScale = 1.0f;
        AudioManager.instance.m_globalPitch = Time.timeScale;
        Time.fixedDeltaTime = 0.02f;
        yield return null;
    }

    public static void Advance()
    {
        currentLevel += deltaLevel;
    }

    #region Player Info Storage
    private struct PlayerInfo
    {
        public WeaponData m_leftWeapon;
        public WeaponData m_rightWeapon;
         
        public Dictionary<ItemEffect, int> m_effects;
    }

    static public bool m_containsPlayerInfo = false;
    static private PlayerInfo m_playerInfo;

    public static void StorePlayerInfo(WeaponData _leftWeapon, WeaponData _rightWeapon, Dictionary<ItemEffect, int> _effects)
    {
        m_playerInfo.m_leftWeapon = _leftWeapon;
        m_playerInfo.m_rightWeapon = _rightWeapon;

        m_playerInfo.m_effects = _effects;

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

    public static Dictionary<ItemEffect, int> RetrieveEffectsDictionary()
    {
        return m_playerInfo.m_effects;
    }

    public static void ResetPlayerInfo()
    {
        m_playerInfo.m_leftWeapon = null;
        m_playerInfo.m_rightWeapon = null;

        m_playerInfo.m_effects = null;

        m_containsPlayerInfo = false;
    }

    #endregion
}
