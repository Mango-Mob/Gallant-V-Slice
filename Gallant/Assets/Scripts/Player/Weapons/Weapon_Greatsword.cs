using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Greatsword : Weapon_Sword
{
    private GameObject m_vfxPrefab;
    bool isDashing = false;
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("VFX/WeaponSwings/Spear Spin");
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
        if (!playerController.animator.GetBool("UsingLeft") && isDashing)
        {
            isDashing = false;
        }
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        isDashing = true;
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType, 2);
        Transform modelTransform = playerController.playerMovement.playerModel.transform;
        playerController.playerMovement.ApplyDashMovement(modelTransform.forward * m_weaponData.m_dashSpeed * m_weaponData.m_speed * m_weaponData.m_altSpeedMult, m_weaponData.m_dashDuration / (m_weaponData.m_speed * m_weaponData.m_altSpeedMult), modelTransform.forward);
    }
    public override void WeaponAltRelease()
    {
        GameObject VFX = SpawnVFX(m_objectPrefab, transform.position + transform.up, playerController.playerMovement.playerModel.transform.rotation);
        VFX.transform.localScale *= (m_weaponData.hitCenterOffset + m_weaponData.hitSize) * 0.6f;
        VFX.transform.SetParent(transform);

        VFX.transform.localScale = new Vector3(VFX.transform.localScale.x * -1.0f,
            VFX.transform.localScale.y,
            VFX.transform.localScale.z);

        isDashing = false;
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        MeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight, Hand.LEFT);
    }
}
