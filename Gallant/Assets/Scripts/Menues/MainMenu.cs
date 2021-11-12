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
        if(InputManager.instance.IsGamepadButtonDown(ButtonType.NORTH, 0) || InputManager.instance.IsKeyDown(KeyType.T))
        {
            GameManager.instance.enableTimer = true;
            GetComponent<SoloAudioAgent>().Play();
        }

        if(m_mainDisplay.activeInHierarchy)
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
    }

    public void StartGame()
    {
        GameManager.currentLevel = 0;
        LevelLoader.instance.LoadNewLevel("MainLevel_1");
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
        LevelLoader.instance.QuitGame();
    }
}
