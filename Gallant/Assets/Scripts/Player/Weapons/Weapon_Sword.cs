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
        if (!playerController.animator.GetBool("UsingLeft") && m_attackReady)
        {
            m_attackReady = false;
        }
    }
    public override void WeaponFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        MeleeAttack(m_weaponData, transform.position);
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        if (m_attackReady)
        {
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            MeleeAttack(m_weaponData, transform.position);
            m_attackReady = false;
        }
        else
        {
            m_attackReady = true;
        }

        //MeleeAttack(m_weaponData, transform.position);
        //Transform modelTransform = playerController.playerMovement.playerModel.transform;
        //playerController.playerMovement.ApplyDashMovement(-modelTransform.forward * m_weaponData.m_dashSpeed * m_weaponData.m_speed * m_weaponData.m_altSpeedMult, m_weaponData.m_dashDuration / (m_weaponData.m_speed * m_weaponData.m_altSpeedMult), modelTransform.forward);

    }
    public override void WeaponAltRelease() { }
}
