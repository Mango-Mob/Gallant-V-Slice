using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NavigationPortal : MonoBehaviour
{
    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;
    private Interactable m_myInterface;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
    }

    public void Update()
    {
        m_keyboardInput.transform.parent.gameObject.SetActive(m_myInterface.m_isReady);
        m_keyboardInput.gameObject.SetActive(m_myInterface.m_isReady && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_myInterface.m_isReady && InputManager.Instance.isInGamepadMode);
    }

    public void Interact()
    {
        NavigationManager.Instance.SetVisibility(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = false;
    }
}