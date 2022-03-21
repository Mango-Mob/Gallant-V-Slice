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
    [SerializeField] private Material m_flameRollMaterial;
    [SerializeField] private Material m_rockRollMaterial;

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

        bool m_leftPassive = SetButtons(Hand.LEFT, m_leftAbilityType);
        bool m_rightPassive = SetButtons(Hand.RIGHT, m_rightAbilityType);

        m_frameBase.SetActive((m_leftPassive || m_rightPassive));
    }

    private bool SetButtons(Hand _hand, Ability _ability)
    {
        switch (_ability)
        {
            case Ability.ICE_ROLL:
                SetButtonMaterial(_hand, m_iceMaterial);
                break;
            case Ability.HP_BUFF:
                SetButtonMaterial(_hand, m_barrierMaterial);
                break;
            case Ability.THORNS:
                SetButtonMaterial(_hand, m_thornsMaterial);
                break;
            case Ability.FLAME_ROLL:
                SetButtonMaterial(_hand, m_flameRollMaterial);
                break;
            case Ability.ROLL_BASH:
                SetButtonMaterial(_hand, m_rockRollMaterial);
                break;
            default:
                SetButtonMaterial(_hand);
                return false;
        }
        return true;
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
