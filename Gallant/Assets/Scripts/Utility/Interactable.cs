using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public UnityEvent m_interactFunction;
    public bool m_isReady = false;
    public float m_holdDuration = 1.0f;

    public Image m_timeDisplay;
    private float m_timer = 0.0f;
    private Player_Controller m_player;
    // Start is called before the first frame update
    void Start()
    {
        m_player = GameManager.Instance.m_player.GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isReady && m_interactFunction != null && !m_player.m_isDisabledInput)
        {
            if(InputManager.Instance.IsBindPressed("Interact", InputManager.Instance.GetAnyGamePad()))
            {
                m_timer += Time.unscaledDeltaTime;
                if(m_timer >= m_holdDuration)
                {
                    Interact();
                    m_timer = 0.0f;
                }
            }
            else
            {
                m_timer = Mathf.Clamp(m_timer - Time.unscaledDeltaTime, 0.0f, m_holdDuration);
            }
        }
        else
        {
            m_timer = 0.0f;
        }

        if(m_timeDisplay != null)
            m_timeDisplay.fillAmount = m_timer / m_holdDuration;
    }

    public void Interact()
    {
        m_interactFunction.Invoke();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            m_isReady = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_isReady = false;
        }
    }
}
