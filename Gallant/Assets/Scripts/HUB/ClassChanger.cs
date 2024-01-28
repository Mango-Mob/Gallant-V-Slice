using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class ClassChanger : MonoBehaviour
{
    public ClassData m_classData;

    private Interactable m_myInterface;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
        if (GameManager.m_saveInfo.m_startedRun)
        {
            GetComponentInChildren<Outlinable>().enabled = false;
            m_myInterface.display.gameObject.SetActive(false);
            Destroy(m_myInterface);
            Destroy(this);
            return;
        }

        m_myInterface.m_interactFunction.AddListener(Change);
    }
    private void Update()
    {
        m_myInterface.m_usable = !GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isKneeling;
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
        GameManager.Instance.m_player.GetComponent<Player_Controller>().StartKneel(m_classData);
    }
}
