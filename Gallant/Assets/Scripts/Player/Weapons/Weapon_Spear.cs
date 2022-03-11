using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Spear : WeaponBase
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
        LongMeleeAttack(m_weaponData, transform.position);
    }

    private void OnDrawGizmos()
    {
        Vector3 capsulePos = Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_weaponData.hitCenterOffset;
        Gizmos.DrawWireSphere(capsulePos, m_weaponData.hitSize);
        Gizmos.DrawWireSphere(capsulePos + playerController.playerMovement.playerModel.transform.forward * m_weaponData.hitSize, m_weaponData.hitSize);
    }
    public override void WeaponRelease() { }
}
