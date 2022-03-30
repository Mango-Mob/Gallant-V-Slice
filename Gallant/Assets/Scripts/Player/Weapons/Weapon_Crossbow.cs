using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Crossbow : WeaponBase
{
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/CrossbowBolt");
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
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, m_hand);
    }
    public override void WeaponRelease()
    {
        WeaponFunctionality();
    }
    public override void WeaponAltFunctionality() { }
    public override void WeaponAltRelease() { }
}
