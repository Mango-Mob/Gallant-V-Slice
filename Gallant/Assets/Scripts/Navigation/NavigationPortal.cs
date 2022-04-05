﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NavigationPortal : MonoBehaviour
{
    public bool GenerateOnAwake = false;
    public SceneData[] m_dataToGenerateFrom;
    public SceneData m_endNode;

    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;
    private Interactable m_myInterface;

    private void Start()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
        if(GenerateOnAwake && m_dataToGenerateFrom.Length != 0)
        {
            NavigationManager.Instance.m_sceneData = m_dataToGenerateFrom;
            NavigationManager.Instance.SetEnd(m_endNode);
            NavigationManager.Instance.Generate(6, 2, 6);
            NavigationManager.Instance.UpdateMap(0);
        }
    }

    public void Update()
    {
        GetComponent<Collider>().enabled = !NavigationManager.Instance.IsVisible;
        m_keyboardInput.transform.parent.gameObject.SetActive(m_myInterface.m_isReady);
        m_keyboardInput.gameObject.SetActive(m_myInterface.m_isReady && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_myInterface.m_isReady && InputManager.Instance.isInGamepadMode);
    }

    public void Interact()
    {
        NavigationManager.Instance.SetVisibility(true);
        m_myInterface.m_isReady = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = !GameManager.Instance.IsInCombat;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = false;
    }
}