using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassChanger : MonoBehaviour
{
    public ClassData m_classData;
    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;

    private Interactable m_myInterface;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
        m_myInterface.m_interactFunction.AddListener(Change);
    }
    private void Update()
    {
        m_keyboardInput.transform.parent.gameObject.SetActive(m_myInterface.m_isReady);
        m_keyboardInput.gameObject.SetActive(m_myInterface.m_isReady && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_myInterface.m_isReady && InputManager.Instance.isInGamepadMode);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerPrefs.GetInt("RunActive") != 1 && m_classData)
        {
            if (other.GetComponent<Player_Controller>())
            {
                m_myInterface.m_isReady = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (PlayerPrefs.GetInt("RunActive") != 1 && m_classData)
        {
            if (other.GetComponent<Player_Controller>())
            {
                m_myInterface.m_isReady = false;
            }
        }
    }

    public void Change()
    {
        GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_classData);
    }
}
