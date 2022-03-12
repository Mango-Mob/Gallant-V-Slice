using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Staff : Weapon_Sword
{
    new private void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        if (m_weaponObject.GetComponentInChildren<MeshRenderer>().materials.Length > 1 && m_weaponData.abilityData != null)
            m_weaponObject.GetComponentInChildren<MeshRenderer>().materials[1].color = m_weaponData.abilityData.droppedEnergyColor;
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
    }
    public override void WeaponRelease() { }
}
