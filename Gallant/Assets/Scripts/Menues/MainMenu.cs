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
    public Button m_loadButton;
    public Image m_loadStrikeOut;

    [Header("Settings")]
    public SettingsMenu m_settingsMenu;

    [Header("Collection")]
    public GameObject m_collectionMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainDisplay();

        GameManager.LoadPlayerInfoFromFile();
        m_loadButton.interactable = GameManager.RetrieveValidSaveState();
        m_loadStrikeOut.enabled = !GameManager.RetrieveValidSaveState();
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
        if (isNew)
        {
            GameManager.ResetPlayerInfo();
            GameManager.ClearPlayerInfoFromFile();
            SkillTreeReader.instance.EmptyAllTrees();
            
            PlayerPrefs.SetInt("SwampLevel", 0);
            PlayerPrefs.SetInt("CastleLevel", 0);
            PlayerPrefs.SetInt("MagmaLevel", 0);
            EndScreenMenu.Restart();
            LevelManager.Instance.LoadNewLevel("Tutorial");
            return;
        }
        else
        {
            GameManager.LoadPlayerInfoFromFile();
        }

        GameManager.currentLevel = PlayerPrefs.GetFloat("Level", 0f);
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
