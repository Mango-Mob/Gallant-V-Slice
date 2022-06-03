using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    public CollectableData m_data { get; set; }

    public AudioClip m_pickupSound;

    private bool m_ShowUI = false;
    private bool m_hasBeenCollected;

    private Interactable m_interact;
    public void Awake()
    {
        m_interact = GetComponentInChildren<Interactable>();
    }
    // Update is called once per frame
    void Update()
    {
        m_interact.m_isReady = m_ShowUI && !GameManager.Instance.IsInCombat;
    }

    public void Collect()
    {
        PlayerPrefs.SetInt(m_data.collectableID, 2);
        NarrativeManager.Instance.m_collectableStatus[m_data] = true;
        AudioManager.Instance.PlayAudioTemporary(transform.position, m_pickupSound);
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
