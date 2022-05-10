using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Display Components")]
    public Button m_displayBtn;
    public GameObject m_displayMenu;
    public GameObject m_displayDefaultSelected;
    public Dropdown m_resList;
    public Dropdown m_fullscreen;

    public GameObject[] m_controllerDisplay;
    public Color m_bumperColor;


    [Header("Audio Components")]
    public Button m_audioBtn;
    public GameObject m_audioDefaultSelected;
    public GameObject m_audioMenu;
    public List<Slider> m_sliders = new List<Slider>();

    [Header("Control Components")]
    public Button m_controlsBtn;
    public GameObject m_controlsMenu;
    public Image m_keyboard;
    public Image m_controller;


    private Resolution[] m_localResolutions;
    private int m_currentMenuID = 0;
    private int m_maxID = 2;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_sliders.Count; i++)
        {
            m_sliders[i].value = AudioManager.Instance.volumes[i];
        }
        ApplicationManager.instance.Wake();
        m_localResolutions = Screen.resolutions;
        Array.Reverse(m_localResolutions);
        m_resList.ClearOptions();
        List<string> resOptions = new List<string>();
        int selected = 0;
        for (int i = 0; i < m_localResolutions.Length; i++)
        {
            resOptions.Add($"{m_localResolutions[i].width}x{m_localResolutions[i].height} @ {m_localResolutions[i].refreshRate}Hz");
            
            if(m_localResolutions[i].width == ApplicationManager.instance.m_width 
                && m_localResolutions[i].height == ApplicationManager.instance.m_height
                && m_localResolutions[i].refreshRate == ApplicationManager.instance.m_rate)
            {
                selected = i;
            }
        }
        
        m_resList.AddOptions(resOptions);
        m_resList.SetValueWithoutNotify(selected);

        m_displayBtn.GetComponent<Image>().color = m_bumperColor;

        m_fullscreen.ClearOptions();
        List<string> options = new List<string>();
        options.Add("Exclusive Fullscreen");
        options.Add("Fullscreen Window");
        options.Add("Maximized Window");
        options.Add("Windowed");
        m_fullscreen.AddOptions(options);
        m_fullscreen.SetValueWithoutNotify((int)ApplicationManager.instance.m_fullscreen);
    }

    // Update is called once per frame
    void Update()
    {
        AudioUpdate();
        foreach (var item in m_controllerDisplay)
        {
            item.SetActive(InputManager.Instance.isInGamepadMode);
        }

        if(InputManager.Instance.IsGamepadButtonDown(ButtonType.LB, 0) &&  m_currentMenuID > 0)
        {
            m_currentMenuID--;
            UpdateMenu();
        }
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RB, 0) && m_currentMenuID < m_maxID)
        {
            m_currentMenuID++;
            UpdateMenu();
        }

        if (InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
        {
            if(m_displayMenu.activeInHierarchy)
                EventSystem.current.SetSelectedGameObject(m_displayDefaultSelected);
            else
                EventSystem.current.SetSelectedGameObject(m_audioDefaultSelected);

        }
        else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        m_keyboard.enabled = !InputManager.Instance.isInGamepadMode;
        m_controller.enabled = InputManager.Instance.isInGamepadMode;
    }

    private void UpdateMenu()
    {
        switch (m_currentMenuID)
        {
            default:
            case 0:
                ShowDisplay();
                break;
            case 1:
                ShowAudio();
                break;
            case 2:
                ShowControls();
                break;
        }
    }

    private void OnEnable()
    {
        ShowDisplay();
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void OnDestroy()
    {
        AudioManager.Instance?.SaveData();
        InputManager.Instance?.SaveBinds();
    }

    private void OnDisable()
    {
        AudioManager.Instance?.SaveData();
        InputManager.Instance?.SaveBinds();
    }

    private void AudioUpdate()
    {
        for (int i = 0; i < m_sliders.Count; i++)
        {
            AudioManager.Instance.volumes[i] = m_sliders[i].value;
        }
    }

    private void Refresh()
    {
        m_displayBtn.interactable = true;
        m_audioBtn.interactable = true;
        m_controlsBtn.interactable = true;

        m_displayBtn.GetComponent<Image>().color = Color.white;
        m_audioBtn.GetComponent<Image>().color = Color.white;
        m_controlsBtn.GetComponent<Image>().color = Color.white;

        m_displayMenu.SetActive(false);
        m_audioMenu.SetActive(false);
        m_controlsMenu.SetActive(false);
    }

    public void ShowDisplay()
    {
        Refresh();
        //m_displayBtn.GetComponent<Image>().color = m_bumperColor;
        m_displayBtn.interactable = false;
        m_displayMenu.SetActive(true);

        if (InputManager.Instance.isInGamepadMode)
        {
            EventSystem.current.SetSelectedGameObject(m_displayDefaultSelected);
        }
        else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ShowAudio()
    {
        Refresh();
        //m_audioBtn.GetComponent<Image>().color = m_bumperColor;
        m_audioBtn.interactable = false;
        m_audioMenu.SetActive(true);

        if (InputManager.Instance.isInGamepadMode)
        {
            EventSystem.current.SetSelectedGameObject(m_audioDefaultSelected);
        }
        else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ShowControls()
    {
        Refresh();
        //m_controlsBtn.GetComponent<Image>().color = m_bumperColor;
        m_controlsBtn.interactable = false;
        m_controlsMenu.SetActive(true);
    }

    public void ResolutionSelection(Dropdown change)
    {
        int select = change.value;

        ApplicationManager.instance.SetResolution(m_localResolutions[select], true);
    }

    public void SetFullScreen(Dropdown change)
    {
        int select = change.value;

        if ((int)ApplicationManager.instance.m_fullscreen == select)
            return;

        ApplicationManager.instance.SetFullScreen(select, true);
    }

    public void ResetControls()
    {
        InputManager.Instance.SetDefaultKeyBinds();
        foreach (var item in GetComponentsInChildren<KeyBindOption>())
        {
            item.ResetDisplay();
        }
    }
}
