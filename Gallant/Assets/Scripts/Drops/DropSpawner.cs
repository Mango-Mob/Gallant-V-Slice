using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    public enum DropType
    {
        WEAPON,
        UPGRADE,
        SPELLBOOK,
    }

    [SerializeField] private Vector3 m_spawnLoc;
    [SerializeField] private DropType m_dropType;

    [Header("Weapon Information")]
    [Range(1, 9)] [SerializeField] private int m_weaponLevel = 1;
    [SerializeField] private Weapon m_weaponType;
    [SerializeField] private Ability m_abilityType;
    [Range(1, 3)] [SerializeField] private int m_abilityPowerLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        switch (m_dropType)
        {
            case DropType.WEAPON:
                GameObject droppedWeapon = DroppedWeapon.CreateDroppedWeapon(transform.position + m_spawnLoc, WeaponData.GenerateSpecificWeapon(m_weaponLevel, m_weaponType, m_abilityType, m_abilityPowerLevel));
                WeaponData weaponData = droppedWeapon.GetComponentInChildren<DroppedWeapon>().m_weaponData;
                InfoDisplay display1 = droppedWeapon.GetComponentInChildren<InfoDisplay>();

                display1.m_weaponData = weaponData;

                if (weaponData.abilityData != null)
                    GetComponentInChildren<SpriteRenderer>().sprite = weaponData.abilityData.abilityIcon;

                break;
            case DropType.UPGRADE:
                GameObject droppedUpgrade = DroppedWeapon.CreateWeaponUpgrade(transform.position + m_spawnLoc);
                break;
            case DropType.SPELLBOOK:
                GameObject droppedSpellbook = DroppedWeapon.CreateSpellUpgrade(transform.position + m_spawnLoc, AbilityData.LoadAbilityData(m_abilityType, m_abilityPowerLevel));
                AbilityData abilityData = droppedSpellbook.GetComponentInChildren<DroppedWeapon>().m_abilityData;
                InfoDisplay display3 = droppedSpellbook.GetComponentInChildren<InfoDisplay>();

                display3.m_abilityData = abilityData;
                break;
            default:
                break;
        }
    }

    public void Configure(int _level, Weapon _weapon, Ability _ability, int _abilityLevel)
    {
        m_weaponLevel = _level;
        m_weaponType = _weapon;
        m_abilityType = _ability;
        m_abilityPowerLevel = _abilityLevel;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
