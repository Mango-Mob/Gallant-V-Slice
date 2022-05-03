using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soup : MonoBehaviour
{
    private Player_Controller playerController;
    private bool m_soupAvailable = true;
    [SerializeField] private GameObject m_soupVisuals;
    [SerializeField] private Interactable m_interactable;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
    }

    public void ActivateSoup()
    {
        if (!m_soupAvailable)
            return;

        playerController.playerResources.FullHeal();
        playerController.playerResources.ChangeAdrenaline(1);

        m_soupVisuals.SetActive(false);
        m_interactable.m_isReady = false;
        m_soupAvailable = false;

        //m_interactable.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
