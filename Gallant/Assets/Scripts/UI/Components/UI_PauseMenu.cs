using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_PauseMenu : MonoBehaviour
{
    public GameObject m_window;
    public static bool isPaused = false;
    public Pause_TomeDisplay m_tomeList;
    public GameObject m_defaultButton;
    public GameObject m_settingsPannel;

    public void SetPause(bool state)
    {
        Time.timeScale = state ? 0.0f : 1.0f;
        m_window.SetActive(state);

        if(state && InputManager.Instance.isInGamepadMode)
            EventSystem.current.SetSelectedGameObject(m_defaultButton);

        m_tomeList.UpdateTomes();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_window.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.Instance.IsKeyDown(KeyType.ESC) || InputManager.Instance.IsGamepadButtonDown(ButtonType.START, 0))
        {
            SetPause(!m_window.activeInHierarchy);
        }
        isPaused = m_window.activeInHierarchy;

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
        //LevelManager.Instance.
    }

    public void Quit()
    {
        isPaused = false;
        SetPause(false);
        ActorManager.Instance.ClearActors();
        SceneManager.LoadScene(0);
    }
}
