﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Sword : WeaponBase
{
    new private void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
    }
    public override void WeaponFunctionality()
    {
        MeleeAttack(m_weaponData, transform.position);
    }
}
