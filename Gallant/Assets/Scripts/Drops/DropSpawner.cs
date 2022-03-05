﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawner : MonoBehaviour
{
    [SerializeField] private Vector3 m_spawnLoc;

    [Header("Weapon Information")]
    [Range(1, 9)] [SerializeField] private int m_weaponLevel = 1;
    [SerializeField] private Weapon m_weaponType;
    [SerializeField] private Ability m_abilityType;
    [Range(1, 3)] [SerializeField] private int m_abilityPowerLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        DroppedWeapon.CreateDroppedWeapon(transform.position + m_spawnLoc, WeaponData.GenerateSpecificWeapon(m_weaponLevel, m_weaponType, m_abilityType, m_abilityPowerLevel));
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
