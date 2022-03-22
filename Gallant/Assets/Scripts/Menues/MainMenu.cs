using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;

//Update please
public class MainMenu : MonoBehaviour
{
    public string m_sceneName = "MainGameScene";
    public GameObject m_mainDisplay;
    //public GameObject m_settingDisplay;

    public Button m_firstSelectedButton;

    [Header("Settings")]
    public SettingsMenu m_settingsMenu;

    [Header("Collection")]
    public GameObject m_collectionMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainDisplay();
    }
    // Update is called once per frame
    void Update()
    {
        if(m_mainDisplay.activeInHierarchy)
        {
            if (InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(m_firstSelectedButton.gameObject);
            }
            else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    public void StartGame(bool isNew = false)
    {
        GameManager.m_firstTime = isNew;
        GameManager.currentLevel = 0;
        GameManager.ResetPlayerInfo();

        if (!isNew)
            GameManager.LoadPlayerInfoFromFile();
        else
            GameManager.SavePlayerInfoToFile();


        EndScreenMenu.Restart();
        LevelManager.Instance.LoadNewLevel("HubWorld");
    }
    public void MainDisplay()
    {
        m_settingsMenu.gameObject.SetActive(false);
        m_collectionMenu.SetActive(false);
        m_mainDisplay.SetActive(true);
    }
    public void Settings()
    {
        m_settingsMenu.gameObject.SetActive(true);
        m_mainDisplay.SetActive(false);
        m_collectionMenu.SetActive(false);
    }
    public void Collection()
    {
        m_collectionMenu.SetActive(true);
        m_mainDisplay.SetActive(false);
        m_settingsMenu.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        LevelManager.Instance.QuitGame();
    }
}
