using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPassiveVFX : MonoBehaviour
{
    private Ability m_leftAbilityType = Ability.NONE;
    private Ability m_rightAbilityType = Ability.NONE;

    private GameObject m_activeLeftVFX;
    private GameObject m_activeRightVFX;

    [SerializeField] private float m_offHandRingSizeMult = 0.75f;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_flamerollPrefab;
    [SerializeField] private GameObject m_frostrollPrefab;
    [SerializeField] private GameObject m_rockrollPrefab;
    [SerializeField] private GameObject m_thornsPrefab;

    [Header("Old Variables")]
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

    public void ChangeActiveVFX(Hand _hand, Ability _ability)
    {
        DeactivateVFX(_hand);
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
        ActivateVFX(_hand, _ability);
    }

    public void HideVFX(Hand _hand, bool _hidden)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_activeLeftVFX)
                    m_activeLeftVFX.SetActive(!_hidden);
                break;
            case Hand.RIGHT:
                if (m_activeRightVFX)
                    m_activeRightVFX.SetActive(!_hidden);
                break;
            default:
                return;
        }
    }

    private void ActivateVFX(Hand _hand, Ability _ability)
    {
        GameObject vfxPrefab;
        switch (_ability)
        {
            case Ability.ICE_ROLL:
                vfxPrefab = m_frostrollPrefab;
                break;
            case Ability.THORNS:
                vfxPrefab = m_thornsPrefab;
                break;
            case Ability.FLAME_ROLL:
                vfxPrefab = m_flamerollPrefab;
                break;
            case Ability.ROLL_BASH:
                vfxPrefab = m_rockrollPrefab;
                break;
            default:
                return;
        }

        if (vfxPrefab == null)
            return;

        switch (_hand)
        {
            case Hand.LEFT:
                m_activeLeftVFX = Instantiate(vfxPrefab, transform);
                m_activeLeftVFX.transform.localPosition = Vector3.zero;
                m_activeLeftVFX.transform.localScale *= m_offHandRingSizeMult;
                break;
            case Hand.RIGHT:
                m_activeRightVFX = Instantiate(vfxPrefab, transform);
                m_activeRightVFX.transform.localPosition = Vector3.zero;
                break;
        }
    }
    private void DeactivateVFX(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_activeLeftVFX == null)
                    return;
                Destroy(m_activeLeftVFX);
                break;
            case Hand.RIGHT:
                if (m_activeRightVFX == null)
                    return;
                Destroy(m_activeRightVFX);
                break;
        }
    }

    //*****************\\
    // This took me 5  \\
    //  long minutes.  \\
    //*****************\\
    //**OLD*FUNCTIONS**\\
    //*****************\\
    //********|********\\
    //********|********\\
    //*******\|/*******\\
    //********V********\\
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
