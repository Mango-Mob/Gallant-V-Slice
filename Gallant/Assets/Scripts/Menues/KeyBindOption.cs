using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindOption : MonoBehaviour
{
    [Header("Settings")]
    public string m_bindID;
    public Button[] m_slots;
    public Image[] m_imageSlot;
    public Text[] m_textSlot;
    public bool m_stickOnly = false;
    public bool m_keyboardOnly = false;
    public bool m_controllerOnly = false;
    public bool m_onlyOne = false;

    private bool hasQueuedRefresh = false;
    private List<InputManager.Bind> m_myBinds;
    private int m_isListening = -1;

    // Start is called before the first frame update
    void Start()
    {
        if(m_onlyOne)
        {
            m_slots[1].gameObject.SetActive(false);
        }

        InputManager.Bind[] array = InputManager.instance.GetBinds(m_bindID);
        m_myBinds = (array != null) ? new List<InputManager.Bind>(array) : new List<InputManager.Bind>();
        for (int i = 0; i < m_slots.Length; i++)
        {
            CleanListener(i);
        }

        if(m_myBinds.Count == 0)
        {
            hasQueuedRefresh = false;
            return;
        }

        for (int i = 0; i < m_myBinds.Count && i < m_slots.Length; i++)
        {
            if(m_myBinds[i] != null)
            {
                switch (InputManager.Bind.GetTypeID(m_myBinds[i].enumType))
                {
                    case 0:
                        UpdateKeyboardDisplay((KeyType)m_myBinds[i].value, i);
                        break;
                    case 1:
                        UpdateMouseDisplay((MouseButton)m_myBinds[i].value, i);
                        break;
                    case 2:
                        UpdateGamepadDisplay((ButtonType)m_myBinds[i].value, i);
                        break;
                    case 3:
                        UpdateStickDisplay((StickType)m_myBinds[i].value, i);
                        break;

                    default:
                        break;
                }
            }
        }
        hasQueuedRefresh = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isListening != -1)
        {
            int listener = m_isListening;
            if (m_stickOnly)
            {
                StickType stickResult = InputManager.instance.IsAnyGameStickInput(InputManager.instance.GetAnyGamePad());
                if (stickResult != StickType.NONE)
                {
                    m_isListening = -1;
                    UpdateStickDisplay(stickResult, listener);
                }
                MouseButton buttonResult = InputManager.instance.IsAnyMouseButtonDown();
                if (buttonResult != MouseButton.NONE)
                {
                    m_isListening = -1;

                    if(m_myBinds.Count > listener)
                        UpdateStickDisplay((StickType)m_myBinds[listener].value, listener);
                }
            }
            else
            {
                KeyType keyResult = InputManager.instance.IsAnyKeyDown();
                ButtonType padResult = InputManager.instance.IsAnyGamePadInput(InputManager.instance.GetAnyGamePad());
                MouseButton buttonResult = InputManager.instance.IsAnyMouseButtonDown();
                if (keyResult != KeyType.NONE && !m_controllerOnly)
                {
                    m_isListening = -1;
                    UpdateKeyboardDisplay(keyResult, listener);
                }
                else if (padResult != ButtonType.NONE && !m_keyboardOnly)
                {
                    m_isListening = -1;
                    UpdateGamepadDisplay(padResult, listener);
                }
                else if (buttonResult != MouseButton.NONE && !m_controllerOnly)
                {
                    m_isListening = -1;
                    UpdateMouseDisplay(buttonResult, listener);
                }
                else
                {
                    int i = 0;
                }
            }

            if (m_isListening == -1)
            {
                UpdateInputManager();
            }
        }
        else if(InputManager.instance.bindHasUpdated)
        {
            hasQueuedRefresh = true;
        }

        if(hasQueuedRefresh && !InputManager.instance.bindHasUpdated)
        {
            Start();
        }
    }

    private void UpdateInputManager()
    {
        InputManager.instance.SetBinds(m_bindID, m_myBinds.ToArray());
    }

    private void UpdateKeyboardDisplay(KeyType keyResult, int listener)
    {

        if (keyResult != KeyType.NONE && keyResult != KeyType.TILDE)
            m_textSlot[listener].text = InputManager.instance.GetKeyString(keyResult);
        else
        {
            m_isListening = listener;
            return;
        }

        if (listener >= m_myBinds.Count)
            m_myBinds.Add(new InputManager.Bind(typeof(KeyType), (int)keyResult));
        else
            m_myBinds[listener] = new InputManager.Bind(typeof(KeyType), (int)keyResult);
        m_textSlot[listener].gameObject.SetActive(true);
    }

    private void UpdateGamepadDisplay(ButtonType padResult, int listener)
    {
        if (padResult != ButtonType.NONE)
            m_imageSlot[listener].sprite = InputManager.instance.GetGamepadSprite(padResult);
        else
        {
            m_isListening = listener;
            return;
        }

        if (listener >= m_myBinds.Count)
            m_myBinds.Add(new InputManager.Bind(typeof(ButtonType), (int)padResult));
        else
            m_myBinds[listener] = new InputManager.Bind(typeof(ButtonType), (int)padResult);

        m_imageSlot[listener].gameObject.SetActive(true);
        m_textSlot[listener].gameObject.SetActive(false);
    }

    private void UpdateMouseDisplay(MouseButton buttonResult, int listener)
    {
        if (buttonResult != MouseButton.NONE)
            m_textSlot[listener].text = InputManager.instance.GetMouseButtonString(buttonResult);
        else
        {
            m_isListening = listener;
            return;
        }

        if (listener >= m_myBinds.Count)
            m_myBinds.Add(new InputManager.Bind(typeof(MouseButton), (int)buttonResult));
        else
            m_myBinds[listener] = new InputManager.Bind(typeof(MouseButton), (int)buttonResult);
        m_textSlot[listener].gameObject.SetActive(true);
    }

    private void UpdateStickDisplay(StickType stickResult, int listener)
    {
        if (stickResult != StickType.NONE)
            m_imageSlot[listener].sprite = InputManager.instance.GetGameStickSprite(stickResult);
        else
        {
            m_isListening = listener;
            return;
        }


        if (listener >= m_myBinds.Count)
            m_myBinds.Add(new InputManager.Bind(typeof(StickType), (int)stickResult));
        else
            m_myBinds[listener] = new InputManager.Bind(typeof(StickType), (int)stickResult);

        m_textSlot[listener].gameObject.SetActive(false);
        m_imageSlot[listener].gameObject.SetActive(true);
    }

    public void StartListening(int listener)
    {
        m_isListening = listener;
        CleanListener(listener);
        m_textSlot[listener].text = "...";
    }

    private void CleanListener(int listener)
    {
        m_imageSlot[listener].gameObject.SetActive(false);
        m_textSlot[listener].gameObject.SetActive(true);
        m_textSlot[listener].text = "";
    }

    public void ResetDisplay()
    {
        Start();
    }
}
