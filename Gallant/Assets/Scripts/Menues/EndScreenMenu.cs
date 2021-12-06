﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EndScreenMenu : MonoBehaviour
{
    public string m_menuSceneName = "MainMenu";
    public Button m_firstSelectedButton;

    public static bool m_victory = false;

    public static float elapsedTimeInSeconds = 0;
    public static int roomsCleared = 0;
    public static float damageDealt = 0;
    public static int levelReached = 1;

    [SerializeField] private GameObject m_victoryText;
    [SerializeField] private GameObject m_defeatText;

    [Header("Information Fields")]
    
    [SerializeField] private TextMeshProUGUI m_timeText;
    [SerializeField] private TextMeshProUGUI m_levelReachedText;
    [SerializeField] private TextMeshProUGUI m_roomsClearedText;
    [SerializeField] private TextMeshProUGUI m_damageDealtText;

    // Start is called before the first frame update
    void Start()
    {
        m_timeText.text = CaluclateTime();
        m_levelReachedText.text = (levelReached).ToString();
        m_roomsClearedText.text = (roomsCleared).ToString();
        m_damageDealtText.text = CalculateDamage();

        if (m_victory)
        {
            m_defeatText.SetActive(false);
        }
        else
        {
            m_victoryText.SetActive(false);
        }

        Restart();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(m_firstSelectedButton.gameObject);
        }
        else if (!InputManager.instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void BackToMenu()
    {
        LevelLoader.instance.LoadNewLevel(m_menuSceneName);
    }

    private string CalculateDamage()
    {
        int multiples = 0;
        float tempVal = damageDealt;

        while (tempVal > 1000f)
        {
            tempVal /= 1000f;
            multiples++;
        }

        switch (multiples)
        {
            default:
            case 0:
                return $"{damageDealt.ToString()}";
            case 1:
                return $"{damageDealt.ToString()}k";
            case 2:
                return $"{damageDealt.ToString()}M";
            case 3:
                return $"{damageDealt.ToString()}B";
        }
    }

    private string CaluclateTime()
    {
        float minutes;
        float hours;

        if (elapsedTimeInSeconds > 60)
        {
            if (elapsedTimeInSeconds > 60 * 60)
            {
                hours = elapsedTimeInSeconds / (60 * 60);
                return $"{hours.ToString().Substring(0, 4)} hours";
            }
            minutes = elapsedTimeInSeconds / 60;
            return $"{minutes.ToString().Substring(0, 4)} minutes";
        }
        return $"{elapsedTimeInSeconds.ToString().Substring(0, 4)} seconds";
    }

    public static void Restart()
    {
        elapsedTimeInSeconds = 0;
        roomsCleared = 0;
        damageDealt = 0;
        levelReached = 1;
    }
}
