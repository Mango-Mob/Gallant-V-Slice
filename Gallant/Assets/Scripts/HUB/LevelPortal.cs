using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class LevelPortal : MonoBehaviour
{
    public string m_portalDestination = "";
    public enum PortalRequire { NONE, SWAMP, CASTLE};
    public PortalRequire require;
    public GameObject gate;

    public Color portalColor;
    private Color flairColor;
    private Interactable m_myInterface;

    public SpriteRenderer icon;
    public VisualEffect portalMain;
    public VisualEffect portalFlair;
    public VisualEffect portalBurst;
    public SoloAudioAgent m_audio;
    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();

        SetColor(portalColor);
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (require)
        {
            case PortalRequire.NONE:
                return;
            case PortalRequire.SWAMP:
                if(GameManager.m_saveInfo.m_completedSwamp == 0)
                {
                    this.enabled = false;
                    gate.SetActive(false);
                    GetComponent<Collider>().enabled = false;
                }
                break;
            case PortalRequire.CASTLE:
                if (GameManager.m_saveInfo.m_completedCastle == 0)
                {
                    this.enabled = false;
                    gate.SetActive(false);
                    GetComponent<Collider>().enabled = false;
                }
                break;
            default:
                break;
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
        m_audio.Play();
        NavigationManager.Instance.Clear(true);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();
        GameManager.m_saveInfo.m_startedRun = false;
        LevelManager.Instance.LoadNewLevel(m_portalDestination);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = gate.activeInHierarchy;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = false;
    }

    

    

    
}
