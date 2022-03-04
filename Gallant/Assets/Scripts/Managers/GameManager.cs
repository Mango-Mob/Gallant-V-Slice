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
    private struct PlayerInfo
    {
        public WeaponData m_leftWeapon;
        public WeaponData m_rightWeapon;
        public ClassData m_classData;
         
        public Dictionary<ItemEffect, int> m_effects;
    }

    static public bool m_containsPlayerInfo = false;
    static private PlayerInfo m_playerInfo;

    public static void StorePlayerInfo(WeaponData _leftWeapon, WeaponData _rightWeapon, Dictionary<ItemEffect, int> _effects, ClassData _class)
    {
        m_playerInfo.m_leftWeapon = _leftWeapon;
        m_playerInfo.m_rightWeapon = _rightWeapon;

        m_playerInfo.m_effects = _effects;

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

    public static Dictionary<ItemEffect, int> RetrieveEffectsDictionary()
    {
        return m_playerInfo.m_effects;
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
