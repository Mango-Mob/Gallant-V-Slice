using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    public GameObject m_window;
    public static bool isPaused = false;
    public Pause_TomeDisplay m_tomeList;
    public GameObject m_defaultButton;
    public GameObject m_settingsPannel;
    public Button m_returnToHubButton;


    public Button[] m_allButtons;
    public GameObject m_confirmPannel;
    public GameObject m_noButton;

    private NavigationTelescope m_telescopeInstance;
    private CollectableRoom m_collectionInstance;

    public void SetPause(bool state)
    {
        Time.timeScale = state ? 0.0f : 1.0f;
        m_window.SetActive(state);

        if(state && InputManager.Instance.isInGamepadMode)
            EventSystem.current.SetSelectedGameObject(m_defaultButton);

        m_tomeList.UpdateTomes();

        if (!state)
        {
            m_settingsPannel.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_window.SetActive(false);
        m_confirmPannel.SetActive(false);
        m_returnToHubButton.interactable = (GameManager.m_saveInfo.m_startedRun);

        m_telescopeInstance = FindObjectOfType<NavigationTelescope>();
        m_collectionInstance = FindObjectOfType<CollectableRoom>();
    }

    // Update is called once per frame
    void Update()
    {
        bool telescopeActive = m_telescopeInstance != null && m_telescopeInstance.m_isActive;
        bool collectionsActive = m_collectionInstance != null && m_collectionInstance.m_display.activeInHierarchy;
        bool transitionActive = LevelManager.transition;

        if(!telescopeActive && !collectionsActive && !transitionActive && !NavigationManager.Instance.IsVisible && (InputManager.Instance.IsKeyDown(KeyType.ESC) || InputManager.Instance.IsGamepadButtonDown(ButtonType.START, 0)))
        {
            SetPause(!m_window.activeInHierarchy);
        }
        isPaused = m_window.activeInHierarchy;

        foreach (var button in m_allButtons)
        {
            button.enabled = !m_settingsPannel.activeInHierarchy;
        }

        //if (EventSystem.current.currentSelectedGameObject != null)
            //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

        if(!m_settingsPannel.activeInHierarchy && isPaused)
        {
            if (InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(m_defaultButton);
            }
            else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
    
    public void SelectDefault()
    {
        EventSystem.current.SetSelectedGameObject(m_defaultButton);
    }

    public void OnDisable()
    {
        Time.timeScale = 1.0f;
    }

    public void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }

    public void ReturnToHub()
    {
        GameManager.ClearPlayerInfoFromFile();
        GameManager.ResetPlayerInfo();

        m_confirmPannel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(m_noButton);
        foreach (var item in m_allButtons)
        {
            item.interactable = false;
        }
    }

    public void Quit()
    {
        isPaused = false;
        SetPause(false);
        ActorManager.Instance.ClearActors();
        SceneManager.LoadScene(0);
    }

    public void Confirm()
    {
        LevelManager.Instance.LoadHubWorld(true);
    }

    public void Decline()
    {
        m_confirmPannel.SetActive(false);
        foreach (var item in m_allButtons)
        {
            item.interactable = true;
        }

        SelectDefault();
    }
}
