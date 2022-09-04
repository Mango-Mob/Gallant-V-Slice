using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class Weapon_Dagger : WeaponBase
{
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/DaggerAttack");
        m_objectAltPrefab = Resources.Load<GameObject>("WeaponProjectiles/DaggerPull");
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
        Transform rightHand = playerController.playerAttack.m_rightHandTransform;

        //GameObject VFX = Instantiate(m_objectPrefab, rightHand.position, Quaternion.identity);
        GameObject VFX = ShootProjectile(rightHand.position, m_weaponData, Hand.RIGHT);

        switch (playerController.animator.GetInteger("ComboCount"))
        {
            case 0:

                break;
            case 1:
                VFX.transform.localScale = new Vector3(VFX.transform.localScale.x * -1.0f,
                    VFX.transform.localScale.y,
                    VFX.transform.localScale.z);
                break;
            default:
                break;
        }

        VFX.transform.forward = playerController.playerMovement.playerModel.transform.forward;
        VFX.transform.SetParent(rightHand);

        m_weaponObject.SetActive(false);

        MeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight);
    }
    public override void WeaponRelease() { }

    GameObject m_thrownWeapon;
    public override void WeaponAltFunctionality()
    {
        m_thrownWeapon = ThrowWeapon(m_weaponObject.transform.position, m_weaponData, Hand.LEFT);
    }
    public override void WeaponAltRelease()
    {
        //playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType, 2);
        isDashing = true;
        Transform modelTransform = playerController.playerMovement.playerModel.transform;

        Vector3 forward = (m_thrownWeapon.transform.position - m_weaponObject.transform.position).normalized;

        Vector3 velocity = forward * m_weaponData.m_dashSpeed * m_weaponData.m_speed * m_weaponData.m_altSpeedMult;
        float distance = (m_thrownWeapon.transform.position - m_weaponObject.transform.position).magnitude - 0.4f;
        float duration = distance / velocity.magnitude;

        playerController.playerMovement.ApplyDashMovement(velocity, duration, forward);
    }

    public override string GetWeaponName()
    {
        return "Staff";
    }
}
