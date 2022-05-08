﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class LevelPortal : MonoBehaviour
{
    public string m_portalDestination = "";
    public string m_prefRequire = "";
    public GameObject gate;

    public Color portalColor;
    private Color flairColor;
    private Interactable m_myInterface;

    public SpriteRenderer icon;
    public VisualEffect portalMain;
    public VisualEffect portalFlair;
    public VisualEffect portalBurst;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();

        SetColor(portalColor);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_prefRequire == "")
            return;

        if(PlayerPrefs.GetInt(m_prefRequire, 0) != 1)
        {
            this.enabled = false;
            gate.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
    }

    public void Update()
    {
        GetComponent<Collider>().enabled = !NavigationManager.Instance.IsVisible;
    }

    public void SetColor(Color color)
    {
        float darkenAmount = 50;
        portalColor = color;
        flairColor = new Color(Mathf.Clamp(color.r - darkenAmount, 0, 255), Mathf.Clamp(color.g - darkenAmount, 0, 255), Mathf.Clamp(color.b - darkenAmount, 0, 255));

        icon.color = portalColor;
        portalMain.SetVector4("Particle Color", portalColor);
        portalBurst.SetVector4("Particle Color", flairColor);
        portalFlair.SetVector4("Particle Color", flairColor);
    }

    public void Interact()
    {
        NavigationManager.Instance.Clear(true);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();

        LevelManager.Instance.LoadNewLevel(m_portalDestination);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = !GameManager.Instance.IsInCombat;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = false;
    }

    

    

    
}
