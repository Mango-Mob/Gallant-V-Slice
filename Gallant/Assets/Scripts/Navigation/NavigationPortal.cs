using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class NavigationPortal : MonoBehaviour
{
    public bool GenerateOnAwake = false;
    public LevelData[] m_levelToGenerate;

    public Color portalColor;
    private Color flairColor;
    private Interactable m_myInterface;

    public GameObject m_portalMainObj;

    public SpriteRenderer icon;
    public VisualEffect portalMain;
    public VisualEffect portalFlair;
    public VisualEffect portalBurst;
    public SoloAudioAgent m_audio;

    public Animator m_portalAnimator;

    private void Start()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
        m_portalAnimator = GetComponent<Animator>();
        if (GenerateOnAwake && m_levelToGenerate != null)
        {
            int select = UnityEngine.Random.Range(0, m_levelToGenerate.Length);
            NavigationManager.Instance.Generate(m_levelToGenerate[select]);
            NavigationManager.Instance.UpdateMap(0);
            SetColor(m_levelToGenerate[select].m_portalColor);
        }
        else if (NavigationManager.Instance.m_generatedLevel != null)
        {
            SetColor(NavigationManager.Instance.m_generatedLevel.m_portalColor);
        }
        else
        {
            SetColor(portalColor);
        }
    }

    public void Update()
    {
        m_portalMainObj.SetActive(!GameManager.Instance.IsInCombat && !RewardManager.Instance.IsVisible);
        m_portalAnimator.SetBool("IsVisible", !GameManager.Instance.IsInCombat && !RewardManager.Instance.IsVisible);
        GetComponent<Collider>().enabled = !GameManager.Instance.IsInCombat && !NavigationManager.Instance.IsVisible;
    }

    public void Interact()
    {
        m_audio.Play();
        NavigationManager.Instance.SetVisibility(true);
        m_myInterface.m_isReady = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if((m_myInterface != null && other.gameObject.layer == LayerMask.NameToLayer("Player")))
            m_myInterface.m_isReady = m_portalMainObj.activeInHierarchy;
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_myInterface != null && other.gameObject.layer == LayerMask.NameToLayer("Player"))
            m_myInterface.m_isReady = false;
    }
}