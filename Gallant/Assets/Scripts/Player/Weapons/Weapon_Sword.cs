using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Sword : WeaponBase
{
    new private void Awake()
    {
        m_objectAltPrefab = Resources.Load<GameObject>("WeaponProjectiles/BoomerangProjectile");
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
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        MeleeAttack(m_weaponData, transform.position);
        Transform modelTransform = playerController.playerMovement.playerModel.transform;
        playerController.playerMovement.ApplyDashMovement(-modelTransform.forward * m_weaponData.m_dashSpeed, m_weaponData.m_dashDuration, modelTransform.forward);

        //ThrowBoomerang(m_weaponObject.transform.position, m_weaponData, m_hand);
    }
    public override void WeaponAltRelease() { }
}
