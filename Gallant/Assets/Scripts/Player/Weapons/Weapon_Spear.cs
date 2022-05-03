using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Spear : WeaponBase
{
    new private void Awake()
    {
        m_objectAltPrefab = Resources.Load<GameObject>("WeaponProjectiles/CrossbowBolt");
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
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        LongMeleeAttack(m_weaponData, transform.position);
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, Hand.LEFT);
    }
    public override void WeaponAltRelease() { }
    private void OnDrawGizmos()
    {
        Vector3 capsulePos = Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_weaponData.hitCenterOffset;
        Gizmos.DrawWireSphere(capsulePos, m_weaponData.hitSize);
        Gizmos.DrawWireSphere(capsulePos + playerController.playerMovement.playerModel.transform.forward * m_weaponData.hitSize, m_weaponData.hitSize);
    }
}
