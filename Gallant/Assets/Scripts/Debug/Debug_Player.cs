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

    [Space(20.0f)]
    // Ability
    [SerializeField] private TMP_Dropdown m_abilitySelectInput;
    [SerializeField] private TMP_InputField m_abilityLevelInput;

    [Space(20.0f)]
    [SerializeField] private Button m_leftHandButton;
    [SerializeField] private Button m_dropButton;
    [SerializeField] private Button m_rightHandButton;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateWeapon()
    {
        DroppedWeapon.CreateDroppedWeapon(GameManager.Instance.m_player.transform.position,
            WeaponData.GenerateSpecificWeapon(1, (Weapon)m_weaponSelectInput.value, (Ability)m_weaponSelectInput.value, 1));
    }
}
