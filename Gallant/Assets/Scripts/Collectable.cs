using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public CollectableData m_data;
    public GameObject m_keyboardInput;
    public GameObject m_gamePadInput;

    public AudioClip m_pickupSound;

    public bool m_testMode = false;

    private bool m_ShowUI = false;
    private bool m_hasBeenCollected;
    // Start is called before the first frame update
    void Start()
    {
        m_hasBeenCollected = PlayerPrefs.GetInt(m_data.collectableID, 0) == 1;
        if(m_hasBeenCollected && !m_testMode)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_keyboardInput.SetActive(m_ShowUI && !InputManager.instance.isInGamepadMode);
        m_gamePadInput.SetActive(m_ShowUI && InputManager.instance.isInGamepadMode);
    }

    public void Collect()
    {
        PlayerPrefs.SetInt(m_data.collectableID, 1);
        AudioManager.instance.PlayAudioTemporary(transform.position, m_pickupSound);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_ShowUI = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_ShowUI = false;
        }
    }
}
