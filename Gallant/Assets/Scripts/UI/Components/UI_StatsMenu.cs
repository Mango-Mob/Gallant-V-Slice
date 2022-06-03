using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_StatsMenu : UI_Element
{
    private bool m_active = false;
    public float m_deactiveOffset = 300.0f;

    [SerializeField] private CanvasGroup m_statCanvasGroup;
    [SerializeField] private CanvasGroup m_balanceCanvasGroup;

    [SerializeField] private Image m_background;
    [SerializeField] private TextMeshProUGUI m_currencyText;
    [SerializeField] private TextMeshProUGUI m_currencyTextShad;

    [Header("Currency Display")]
    [SerializeField] private float m_balanceChangeSpeed = 0.9f;
    private float m_displayedBalance = 0;
    private float m_balanceDisplayLerp = 0.0f;

    [Header("Rune Info")]
    public Transform m_runeGroup;
    public GameObject m_listObject;
    public GameObject m_runeInfoPrefab;
    private float m_runeStartPosX = 0.0f;

    [Header("Weapon Info")]
    public Transform m_weaponGroup;
    public InfoDisplay m_leftWeaponDisplay;
    public InfoDisplay m_rightWeaponDisplay;

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

        m_displayedBalance = PlayerPrefs.GetInt($"Player Balance {GameManager.m_saveSlotInUse}");
    }

    // Update is called once per frame
    void Update()
    {
        float balanceDifference = PlayerPrefs.GetInt($"Player Balance {GameManager.m_saveSlotInUse}") - m_displayedBalance;
        m_displayedBalance += (balanceDifference * Time.deltaTime * m_balanceChangeSpeed);

        m_currencyText.text = $"{Mathf.RoundToInt(m_displayedBalance)}";
        m_currencyTextShad.text = $"{Mathf.RoundToInt(m_displayedBalance)}";

        m_balanceDisplayLerp = Mathf.Clamp(m_balanceDisplayLerp + (Mathf.Abs(balanceDifference) > 0.5f ? 1.0f : -0.2f) * Time.deltaTime * 5.0f, 0.0f, 1.0f);

        if (InputManager.Instance.IsKeyDown(KeyType.TAB) || InputManager.Instance.IsGamepadButtonDown(ButtonType.SELECT, InputManager.Instance.GetAnyGamePad()))
            ToggleActive();

        m_offsetLerp = Mathf.Clamp(m_offsetLerp + (m_active ? 1.0f : -1.0f) * Time.deltaTime * 5.0f, 0.0f, 1.0f);

        m_statCanvasGroup.alpha = m_offsetLerp;
        m_statCanvasGroup.blocksRaycasts = m_statCanvasGroup.alpha != 0.0f;

        m_leftWeaponDisplay.gameObject.SetActive(m_statCanvasGroup.alpha != 0.0f && m_leftWeaponDisplay.m_weaponData != null);
        m_rightWeaponDisplay.gameObject.SetActive(m_statCanvasGroup.alpha != 0.0f && m_rightWeaponDisplay.m_weaponData != null);

        m_balanceCanvasGroup.alpha = Mathf.Max(m_balanceDisplayLerp, m_offsetLerp);
        //Color backgroundColor = m_background.color;
        //backgroundColor.a = 0.85f * m_offsetLerp;
        //m_background.color = backgroundColor;

        //m_runeGroup.position = new Vector3(m_runeStartPosX + Mathf.Lerp(-m_deactiveOffset, 0, m_offsetLerp), m_runeGroup.position.y, m_runeGroup.position.z);
        m_weaponGroup.position = new Vector3(m_weaponStartPosX + Mathf.Lerp(m_deactiveOffset, 0, m_offsetLerp), m_weaponGroup.position.y, m_weaponGroup.position.z);
    }

    public void UpdateWeaponInfo(Hand _hand, WeaponData _data)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_leftWeaponDisplay == null)
                {
                    Debug.LogWarning("Left Weapon Display is null");
                }
                m_leftWeaponDisplay.LoadWeapon(_data);
                break;
            case Hand.RIGHT:
                if (m_rightWeaponDisplay == null)
                {
                    Debug.LogWarning("Right Weapon Display is null");
                }
                m_rightWeaponDisplay.LoadWeapon(_data);
                break;
        }
    }

    public void ToggleActive()
    {
        m_active = !m_active;
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledAttacks = m_active;

        GetComponentInChildren<CharSheetManager>()?.SetActive(m_active);
    }

    public void UpdateList()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<Player_Stats>();
        if (playerStats == null)
            return;

        List<RuneInfo> removeList = new List<RuneInfo>();
        foreach (var rune in m_runeList)
        {
            bool foundEffect = false;
            foreach (var effect in playerStats.m_effects)
            {
                if (rune.m_effect == effect.Key)
                {
                    foundEffect = true;
                    continue;
                }
            }
            if (!foundEffect)
            {
                removeList.Add(rune);
            }
        }
        foreach (var item in removeList)
        {
            Destroy(item.gameObject);
            m_runeList.Remove(item);
        }

        foreach (var effect in playerStats.m_effects)
        {
            bool runeInfoExists = false;
            foreach (var rune in m_runeList)
            {
                if (rune.m_effect.effect == effect.Key.effect)
                {
                    SetRuneInfo(rune, effect.Key);
                    runeInfoExists = true;
                    break;
                }
            }
            if (!runeInfoExists && effect.Key.effect != ItemEffect.ARCANE_FOCUS)
            {
                RuneInfo newObject = Instantiate(m_runeInfoPrefab, m_listObject.transform).GetComponent<RuneInfo>();
                newObject.m_effect = effect.Key;
                m_runeList.Add(newObject);

                SetRuneInfo(newObject, effect.Key);
            }
        }

    }
    public void SetRuneInfo(RuneInfo _runeInfo, EffectData _effect)
    {
        switch (_effect.effect)
        {
            case ItemEffect.NONE:
                break;
            case ItemEffect.MOVE_SPEED:
                _runeInfo.m_name.text = "Movement Speed";
                _runeInfo.m_number.text = (playerStats.m_movementSpeed - _effect.m_default).ToString((playerStats.m_movementSpeed > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.ABILITY_CD:
                _runeInfo.m_name.text = "Ability Cooldown";
                _runeInfo.m_number.text = (playerStats.m_abilityCD - _effect.m_default).ToString((playerStats.m_abilityCD > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.ATTACK_SPEED:
                _runeInfo.m_name.text = "Attack Speed";
                _runeInfo.m_number.text = (playerStats.m_attackSpeed - _effect.m_default).ToString((playerStats.m_attackSpeed > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.DAMAGE_RESISTANCE:
                _runeInfo.m_name.text = "Damage Resistance";
                _runeInfo.m_number.text = (playerStats.m_damageResistance - _effect.m_default).ToString((playerStats.m_damageResistance > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.MAX_HEALTH_INCREASE:
                _runeInfo.m_name.text = "Max Health";
                _runeInfo.m_number.text = (playerStats.m_maximumHealth - _effect.m_default).ToString((playerStats.m_maximumHealth > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.PHYSICAL_DAMAGE:
                _runeInfo.m_name.text = "Attack Damage";
                _runeInfo.m_number.text = (playerStats.m_physicalDamage - _effect.m_default).ToString((playerStats.m_physicalDamage > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.ABILITY_DAMAGE:
                _runeInfo.m_name.text = "Magical Damage";
                _runeInfo.m_number.text = (playerStats.m_abilityDamage - _effect.m_default).ToString((playerStats.m_abilityDamage > _effect.m_default ? "+0.0%" : "0.0%"));
                break;
            case ItemEffect.PHYSICAL_DEFENCE:
                _runeInfo.m_name.text = "Armor";
                _runeInfo.m_number.text = (playerStats.m_physicalDefence - _effect.m_default).ToString((playerStats.m_physicalDefence > _effect.m_default ? "+0" : "0"));
                break;
            case ItemEffect.ABILITY_DEFENCE:
                _runeInfo.m_name.text = "Ward";
                _runeInfo.m_number.text = (playerStats.m_abilityDefence - _effect.m_default).ToString((playerStats.m_abilityDefence > _effect.m_default ? "+0" : "0"));
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
