using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPassiveVFX : MonoBehaviour
{
    private Ability m_leftAbilityType = Ability.NONE;
    private Ability m_rightAbilityType = Ability.NONE;

    [SerializeField] private GameObject m_frameBase;

    [Header("Buttons")]
    [SerializeField] private Renderer[] m_leftHandPassiveButtons;
    [SerializeField] private Renderer[] m_rightHandPassiveButtons;

    [Header("Material Textures")]
    [SerializeField] private Material m_iceMaterial;
    [SerializeField] private Material m_barrierMaterial;
    [SerializeField] private Material m_thornsMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetAbility(Hand _hand, Ability _ability)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftAbilityType = _ability;
                break;
            case Hand.RIGHT:
                m_rightAbilityType = _ability;
                break;
            default:
                return;
        }

        bool m_leftPassive = true;
        bool m_rightPassive = true;
        switch (m_leftAbilityType)
        {
            case Ability.ICE_ROLL:
                SetButtonMaterial(Hand.LEFT, m_iceMaterial);
                break;
            case Ability.HP_BUFF:
                SetButtonMaterial(Hand.LEFT, m_barrierMaterial);
                break;
            case Ability.THORNS:
                SetButtonMaterial(Hand.LEFT, m_thornsMaterial);
                break;
            default:
                SetButtonMaterial(Hand.LEFT);
                m_leftPassive = false;
                break;
        }

        switch (m_rightAbilityType)
        {
            case Ability.ICE_ROLL:
                SetButtonMaterial(Hand.RIGHT, m_iceMaterial);
                break;
            case Ability.HP_BUFF:
                SetButtonMaterial(Hand.RIGHT, m_barrierMaterial);
                break;
            case Ability.THORNS:
                SetButtonMaterial(Hand.RIGHT, m_thornsMaterial);
                break;
            default:
                SetButtonMaterial(Hand.RIGHT);
                m_rightPassive = false;
                break;
        }

        m_frameBase.SetActive((m_leftPassive || m_rightPassive));
    }

    private void SetButtonMaterial(Hand _hand, Material _material = null)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                foreach (var button in m_leftHandPassiveButtons)
                {
                    if (_material != null)
                    {
                        button.enabled = true;
                        button.material = _material;
                    }
                    else
                    {
                        button.enabled = false;
                    }
                }
                break;
            case Hand.RIGHT:
                foreach (var button in m_rightHandPassiveButtons)
                {
                    if (_material != null)
                    {
                        button.enabled = true;
                        button.material = _material;
                    }
                    else
                    {
                        button.enabled = false;
                    }
                }
                break;
            default:
                break;
        }
    }
}
