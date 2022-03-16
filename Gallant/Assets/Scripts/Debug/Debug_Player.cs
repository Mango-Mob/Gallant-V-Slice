using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Debug_Player : MonoBehaviour
{
    [Header("Create Weapon UI")]
    // Weapon
    [SerializeField] private TMP_Dropdown m_weaponSelectInput;
    [SerializeField] private TMP_InputField m_weaponLevelInput;
    private int m_weaponLevel = 1;

    [Space(20.0f)]
    // Ability
    [SerializeField] private TMP_Dropdown m_abilitySelectInput;
    [SerializeField] private TMP_InputField m_abilityLevelInput;
    [SerializeField] private Button[] m_stars;
    private int m_abilityPowerLevel = 1;

    // Cheats
    [SerializeField] private Toggle m_godModeToggle;

    // Start is called before the first frame update
    void Start()
    {
        m_weaponSelectInput.ClearOptions();
        m_abilitySelectInput.ClearOptions();
        TMP_Dropdown.OptionData data;

        foreach (var weapon in System.Enum.GetNames(typeof(Weapon)))
        {
            data = new TMP_Dropdown.OptionData(weapon);
            m_weaponSelectInput.options.Add(data);
        }
        foreach (var ability in System.Enum.GetNames(typeof(Ability)))
        {
            data = new TMP_Dropdown.OptionData(ability);
            m_abilitySelectInput.options.Add(data);
        }

        m_weaponSelectInput.value = 1;
        m_abilitySelectInput.value = 1;
        SetPowerLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_godModeToggle != null)
        {
            GameManager.Instance.m_player.GetComponent<Player_Controller>().SetGodMode(m_godModeToggle.isOn);
        }
    }

    public void CreateWeapon()
    {
        DroppedWeapon.CreateDroppedWeapon(GameManager.Instance.m_player.transform.position,
            WeaponData.GenerateSpecificWeapon(m_weaponLevel, (Weapon)m_weaponSelectInput.value, (Ability)m_abilitySelectInput.value, m_abilityPowerLevel));
    }

    public void SavePlayerInfoToFile()
    {
        GameManager.SavePlayerInfoToFile();
    }
    public void LoadPlayerInfoFromFile()
    {
        GameManager.LoadPlayerInfoFromFile();
    }

    public void SetPowerLevel(int _power)
    {
        m_abilityPowerLevel = _power;

        for (int i = 0; i < 3; i++)
        {
            m_stars[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        for (int i = 0; i < _power; i++)
        {
            m_stars[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        }
    }
    public void IncreaseWeaponLevel(int _increase)
    {
        m_weaponLevel = Mathf.Clamp(m_weaponLevel + _increase, 1, 99);
        m_weaponLevelInput.text = m_weaponLevel.ToString();
    }

    public void UpgradeWeaponLevel(bool _left)
    {
        if (_left)
        {
            GameManager.Instance.m_player.GetComponent<Player_Controller>().UpgradeWeapon(Hand.LEFT);
        }
        else
        {
            GameManager.Instance.m_player.GetComponent<Player_Controller>().UpgradeWeapon(Hand.RIGHT);
        }
    }

    private void ToggleGodMode(bool _active)
    {
        Debug.Log(_active);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().SetGodMode(_active);
    }
}
