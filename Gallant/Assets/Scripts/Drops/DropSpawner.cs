using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class DropSpawner : MonoBehaviour
{
    public enum DropType
    {
        WEAPON,
        UPGRADE,
        SPELLBOOK,
        SPECIFIC_WEAPON
    }

    [SerializeField] private Vector3 m_spawnLoc;
    [SerializeField] private DropType m_dropType;

    [Header("Weapon Information")]
    [Range(1, 9)] [SerializeField] private int m_weaponLevel = 1;
    [SerializeField] private Weapon m_weaponType;
    [SerializeField] private Ability m_abilityType;
    [Range(1, 3)] [SerializeField] private int m_abilityPowerLevel = 1;
    [SerializeField] private WeaponData m_weaponData;

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
            case DropType.SPECIFIC_WEAPON:
                GameManager.LoadSaveInfoFromFile();
                switch (m_weaponData.weaponType)
                {
                    case Weapon.GREATSWORD:
                        if (GameManager.m_saveInfo.m_completedMagma != 0)
                        {
                            WeaponData newData = ScriptableObject.CreateInstance<WeaponData>();
                            newData.Clone(m_weaponData);

                            GameObject droppedSpecificWeapon = DroppedWeapon.CreateDroppedWeapon(transform.position + m_spawnLoc, newData);
                            InfoDisplay display4 = droppedSpecificWeapon.GetComponentInChildren<InfoDisplay>();

                            display4.m_weaponData = newData;

                            droppedSpecificWeapon.GetComponent<DroppedWeapon>().m_weaponMoves = false;
                        }
                        else
                        {
                            gameObject.SetActive(false);
                        }
                        break;
                    case Weapon.BRICK:
                        WeaponData newData2 = ScriptableObject.CreateInstance<WeaponData>();
                        newData2.Clone(m_weaponData);

                        GameObject droppedSpecificWeapon2 = DroppedWeapon.CreateDroppedWeapon(transform.position + m_spawnLoc, newData2);
                        InfoDisplay display5 = droppedSpecificWeapon2.GetComponentInChildren<InfoDisplay>();

                        display5.m_weaponData = newData2;

                        droppedSpecificWeapon2.GetComponent<DroppedWeapon>().m_weaponMoves = false;
                        break;
                    default:
                        break;
                }

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
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position + m_spawnLoc, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
