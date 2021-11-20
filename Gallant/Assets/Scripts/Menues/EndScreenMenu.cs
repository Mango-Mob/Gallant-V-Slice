using System.Collections;
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

    public static float m_time = 0.0f;
    public static int m_levelReached = 1;
    public static int m_roomsCleared = 0;
    public static int m_damageDealt = 0;

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
        m_timeText.text = m_time.ToString();
        m_levelReachedText.text = m_levelReached.ToString();
        m_roomsClearedText.text = m_roomsCleared.ToString();
        m_damageDealtText.text = m_damageDealt.ToString();

        if (m_victory)
        {
            m_defeatText.SetActive(false);
        }
        else
        {
            m_victoryText.SetActive(false);
        }

        m_time = 0.0f;
        m_levelReached = 1;
        m_roomsCleared = 0;
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
}
