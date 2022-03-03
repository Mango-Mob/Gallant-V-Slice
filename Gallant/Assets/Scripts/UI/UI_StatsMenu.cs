using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatsMenu : UI_Element
{
    private bool m_active = false;
    public float m_deactiveOffset = 300.0f;

    [SerializeField] private Image m_background;

    [Header("Rune Info")]
    public Transform m_runeGroup;
    public GameObject m_listObject;
    public GameObject m_runeInfoPrefab;
    private float m_runeStartPosX = 0.0f;

    [Header("Weapon Info")]
    public Transform m_weaponGroup;
    public WeaponInfoDisplay m_leftWeaponDisplay;
    public WeaponInfoDisplay m_rightWeaponDisplay;

    private float m_weaponStartPosX = 0.0f;

    private List<RuneInfo> m_runeList = new List<RuneInfo>();
    private float m_offsetLerp = 0.0f;
    private Player_Stats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<Player_Stats>();

        m_runeStartPosX = m_runeGroup.position.x;
        m_weaponStartPosX = m_weaponGroup.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.IsKeyDown(KeyType.TAB) || InputManager.Instance.IsGamepadButtonDown(ButtonType.SELECT, InputManager.Instance.GetAnyGamePad()))
            ToggleActive();

        m_offsetLerp = Mathf.Clamp(m_offsetLerp + (m_active ? 1.0f : -1.0f) * Time.deltaTime * 5.0f, 0.0f, 1.0f);

        Color backgroundColor = m_background.color;
        backgroundColor.a = 0.85f * m_offsetLerp;
        m_background.color = backgroundColor;

        m_runeGroup.position = new Vector3(m_runeStartPosX + Mathf.Lerp(-m_deactiveOffset, 0, m_offsetLerp), m_runeGroup.position.y, m_runeGroup.position.z);
        m_weaponGroup.position = new Vector3(m_weaponStartPosX + Mathf.Lerp(m_deactiveOffset, 0, m_offsetLerp), m_weaponGroup.position.y, m_weaponGroup.position.z);
    }

    public void UpdateWeaponInfo(Hand _hand, WeaponData _data)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeaponDisplay.LoadWeapon(_data);
                break;
            case Hand.RIGHT:
                m_rightWeaponDisplay.LoadWeapon(_data);
                break;
        }
    }
    public void ToggleActive()
    {
        m_active = !m_active;
    }

    public void UpdateList()
    {
        foreach (var effect in playerStats.m_effects)
        {
            bool runeInfoExists = false;
            foreach (var rune in m_runeList)
            {
                if (rune.m_effect == effect.Key)
                {
                    SetRuneInfo(rune, effect.Key);
                    runeInfoExists = true;
                    break;
                }
            }
            if (!runeInfoExists)
            {
                RuneInfo newObject = Instantiate(m_runeInfoPrefab, m_listObject.transform).GetComponent<RuneInfo>();
                newObject.m_effect = effect.Key;
                m_runeList.Add(newObject);
                
                SetRuneInfo(newObject, effect.Key);
            }
        }
    }
    public void SetRuneInfo(RuneInfo _runeInfo, ItemEffect _effect)
    {
        switch (_effect)
        {
            case ItemEffect.NONE:
                break;
            case ItemEffect.MOVE_SPEED:
                _runeInfo.m_name.text = "Movement Speed";
                _runeInfo.m_number.text = (playerStats.m_movementSpeed).ToString("0.0%");
                break;
            case ItemEffect.ABILITY_CD:
                _runeInfo.m_name.text = "Ability Cooldown";
                _runeInfo.m_number.text = (playerStats.m_abilityCD).ToString("0.0%");
                break;
            case ItemEffect.ATTACK_SPEED:
                _runeInfo.m_name.text = "Attack Speed";
                _runeInfo.m_number.text = (playerStats.m_attackSpeed).ToString("0.0%");
                break;
            case ItemEffect.DAMAGE_RESISTANCE:
                _runeInfo.m_name.text = "Damage Resistance";
                _runeInfo.m_number.text = (playerStats.m_damageResistance).ToString("0.0%");
                break;
            case ItemEffect.MAX_HEALTH_INCREASE:
                _runeInfo.m_name.text = "Max Health";
                _runeInfo.m_number.text = (playerStats.m_maximumHealth).ToString("0.0%");
                break;
            default:
                break;
        }
    }
    //private void Is
    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        //Do nothing
    }

    public override void OnMouseUpEvent()
    {
        //Do nothing
    }
}
