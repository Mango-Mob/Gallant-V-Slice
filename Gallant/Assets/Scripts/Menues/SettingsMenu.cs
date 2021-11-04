using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Display Components")]
    public Button m_displayBtn;
    public GameObject m_displayMenu;
    public Dropdown m_resList;
    public Dropdown m_fullscreen;

    [Header("Audio Components")]
    public Button m_audioBtn;
    public GameObject m_audioMenu;
    public List<Slider> m_sliders = new List<Slider>();

    [Header("Control Components")]
    public Button m_controlsBtn;
    public GameObject m_controlsMenu;


    private Resolution[] m_localResolutions;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_sliders.Count; i++)
        {
            m_sliders[i].value = AudioManager.instance.volumes[i];
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
    }

    private void OnEnable()
    {
        ShowDisplay();
    }
    private void OnDestroy()
    {
        AudioManager.instance.SaveData();
    }

    private void OnDisable()
    {
        AudioManager.instance.SaveData();
    }

    private void AudioUpdate()
    {
        for (int i = 0; i < m_sliders.Count; i++)
        {
            AudioManager.instance.volumes[i] = m_sliders[i].value;
        }
    }

    private void Refresh()
    {
        m_displayBtn.interactable = true;
        m_audioBtn.interactable = true;
        m_controlsBtn.interactable = true;

        m_displayMenu.SetActive(false);
        m_audioMenu.SetActive(false);
        m_controlsMenu.SetActive(false);
    }

    public void ShowDisplay()
    {
        Refresh();
        m_displayBtn.interactable = false;
        m_displayMenu.SetActive(true);
    }

    public void ShowAudio()
    {
        Refresh();
        m_audioBtn.interactable = false;
        m_audioMenu.SetActive(true);
    }

    public void ShowControls()
    {
        Refresh();
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
}
