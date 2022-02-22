using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Boomerang : WeaponBase
{
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("Abilities/BoomerangProjectile");
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
        ThrowBoomerang(m_weaponObject.transform.position, m_weaponData, m_hand);
    }
}
