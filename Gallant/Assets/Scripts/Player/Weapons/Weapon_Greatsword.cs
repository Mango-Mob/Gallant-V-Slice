using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Greatsword : Weapon_Sword
{
    private GameObject m_vfxPrefab;
    bool isDashing = false;
    new private void Awake()
    {
        m_vfxPrefab = Resources.Load<GameObject>("VFX/GroundSlamVFX");
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
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        if (!isDashing)
        {
            isDashing = true;
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            Transform modelTransform = playerController.playerMovement.playerModel.transform;
            playerController.playerMovement.ApplyDashMovement(modelTransform.forward * m_weaponData.m_dashSpeed * m_weaponData.m_speed * m_weaponData.m_altSpeedMult, m_weaponData.m_dashDuration / (m_weaponData.m_speed * m_weaponData.m_altSpeedMult), modelTransform.forward);
        }
        else
        {
            isDashing = false;
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            MeleeAttack(m_weaponData, transform.position);
        }
    }
    public override void WeaponAltRelease() { }
}
