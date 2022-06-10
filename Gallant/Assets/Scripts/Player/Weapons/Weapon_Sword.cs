using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Sword : WeaponBase
{
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("VFX/WeaponSwings/Sword Trail");
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

        GameObject VFX = SpawnVFX(m_objectPrefab, transform.position + transform.up, playerController.playerMovement.playerModel.transform.rotation);
        VFX.transform.localScale *= (m_weaponData.hitCenterOffset + m_weaponData.hitSize) * 2.0f;
        VFX.transform.SetParent(transform);

        switch (playerController.animator.GetInteger("ComboCount"))
        {
            case 0:
                VFX.transform.localScale = new Vector3(VFX.transform.localScale.x * -1.0f,
                    VFX.transform.localScale.y,
                    VFX.transform.localScale.z);

                VFX.transform.Rotate(new Vector3(0.0f, 60.0f, 0.0f));
                break;
            case 1:
                VFX.transform.Rotate(new Vector3(0.0f, 15.0f, 0.0f));
                break;
            case 2:
                VFX.transform.localScale = new Vector3(VFX.transform.localScale.x * -1.0f,
                    VFX.transform.localScale.y,
                    VFX.transform.localScale.z);

                VFX.transform.Rotate(new Vector3(-100.0f, 0.0f, 95.0f));

                VFX.transform.position += m_weaponData.hitCenterOffset * playerController.playerMovement.playerModel.transform.forward * 0.3f;
                break;
            default:
                break;
        }

        MeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight);
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        if (m_attackReady)
        {
            GameObject VFX = SpawnVFX(m_objectPrefab, transform.position + transform.up, playerController.playerMovement.playerModel.transform.rotation);
            VFX.transform.localScale *= (m_weaponData.hitCenterOffset + m_weaponData.hitSize) * 1.0f;
            VFX.transform.position += m_weaponData.hitCenterOffset * playerController.playerMovement.playerModel.transform.forward;
            VFX.transform.SetParent(transform);

            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            MeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight);
            m_attackReady = false;
        }
        else
        {
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType, 2);
            m_attackReady = true;
        }

        //MeleeAttack(m_weaponData, transform.position);
        //Transform modelTransform = playerController.playerMovement.playerModel.transform;
        //playerController.playerMovement.ApplyDashMovement(-modelTransform.forward * m_weaponData.m_dashSpeed * m_weaponData.m_speed * m_weaponData.m_altSpeedMult, m_weaponData.m_dashDuration / (m_weaponData.m_speed * m_weaponData.m_altSpeedMult), modelTransform.forward);

    }
    public override void WeaponAltRelease() { }
}
