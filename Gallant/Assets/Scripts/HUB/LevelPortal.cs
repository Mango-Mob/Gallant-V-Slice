using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour
{
    public string m_portalDestination = "";
    public string m_prefRequire = "";
    public GameObject gate;

    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;
    private Interactable m_myInterface;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
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
        m_keyboardInput.transform.parent.gameObject.SetActive(m_myInterface.m_isReady);
        m_keyboardInput.gameObject.SetActive(m_myInterface.m_isReady && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_myInterface.m_isReady && InputManager.Instance.isInGamepadMode);
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
