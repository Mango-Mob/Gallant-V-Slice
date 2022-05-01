using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractDisplay : MonoBehaviour
{
    public float timer = 0f;

    [SerializeField] private Image m_icon;
    [SerializeField] private Text m_character;
    [SerializeField] private Image m_timerImage;

    // Update is called once per frame
    public void Update()
    {
        m_timerImage.fillAmount = timer;

        if (InputManager.Instance.isInGamepadMode)
        {
            m_icon.sprite = InputManager.Instance.GetBindImage("Interact", true);
            m_icon.gameObject.SetActive(true);
            m_character.gameObject.SetActive(false);
        }
        else
        {
            Sprite keyIcon = InputManager.Instance.GetBindImage("Interact", false);
            if(keyIcon != null)
            {
                m_icon.sprite = keyIcon;
                m_icon.gameObject.SetActive(true);
                m_character.gameObject.SetActive(false);
            }
            else
            {
                m_character.text = InputManager.Instance.GetBindString("Interact");
                m_icon.gameObject.SetActive(false);
                m_character.gameObject.SetActive(true);
            }
        }
    }
}
