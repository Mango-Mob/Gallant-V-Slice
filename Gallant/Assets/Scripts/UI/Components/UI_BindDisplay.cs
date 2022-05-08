using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BindDisplay : UI_Element
{
    public string m_bindKey;
    public GameObject m_keyboardDisplay;
    public GameObject m_gamepadDisplay;

    private Text m_keyText;
    private TMP_Text m_keyMesh;
    private Image m_button;
    private void Awake()
    {
        if (m_keyboardDisplay != null)
        {
            m_keyText = m_keyboardDisplay.GetComponent<Text>();
            m_keyMesh = m_keyboardDisplay.GetComponent<TMP_Text>();
        }
        if (m_gamepadDisplay != null)
            m_button = m_gamepadDisplay.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_keyText != null)
            m_keyText.text = InputManager.Instance.GetBindString(m_bindKey);

        if (m_keyMesh != null)
            m_keyMesh.text = InputManager.Instance.GetBindString(m_bindKey);

        Sprite sprite;
        if (InputManager.Instance.isInGamepadMode)
        {
            sprite = InputManager.Instance.GetBindImage(m_bindKey, true);
        }
        else
        {
            sprite = InputManager.Instance.GetBindImage(m_bindKey);
        }

        m_button.sprite = sprite;
        m_keyboardDisplay.SetActive(sprite == null);
        m_gamepadDisplay.SetActive(sprite != null);
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {

    }

    public override void OnMouseUpEvent()
    {

    }
}
