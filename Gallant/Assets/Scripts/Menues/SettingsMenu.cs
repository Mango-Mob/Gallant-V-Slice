using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Display Components")]
    public Button m_displayBtn;
    public GameObject m_displayMenu;

    [Header("Audio Components")]
    public Button m_audioBtn;
    public GameObject m_audioMenu;
    public List<Slider> m_sliders = new List<Slider>();

    [Header("Control Components")]
    public Button m_controlsBtn;
    public GameObject m_controlsMenu;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_sliders.Count; i++)
        {
            m_sliders[i].value = AudioManager.instance.volumes[i];
        }
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
}
