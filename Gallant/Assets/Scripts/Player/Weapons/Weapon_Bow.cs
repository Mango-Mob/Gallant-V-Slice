using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Bow : WeaponBase
{
    private bool m_chargingShot = false;
    private float m_charge = 0.0f;
    private float m_chargeRate = 0.5f;
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/BowArrow");
        m_objectAltPrefab = Resources.Load<GameObject>("WeaponProjectiles/ChargedBowArrow");
        base.Awake();
    }

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        if (m_weaponObject != null)
            m_weaponObject.name = "Bow";
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
        if (m_chargingShot && m_charge < 1.0f)
        {
            m_charge += Time.deltaTime * m_chargeRate * playerController.animator.GetFloat(m_hand == Hand.LEFT ? "LeftAttackSpeed" : "RightAttackSpeed");
            m_charge = Mathf.Clamp(m_charge, 0.0f, 1.0f);

            if (m_charge >= 1.0f)
            {
                playerController.playerAudioAgent.PlayOrbPickup();
            }
        }
        if (m_chargingShot)
        {
            playerController.playerResources.ChangeStamina(-m_weaponData.m_altAttackStaminaCost * Time.deltaTime);
        }
    }
    public override void WeaponFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponHit(Weapon.BOW, 1);

        ShootProjectile(m_weaponObject.transform.position, m_weaponData, Hand.RIGHT);
    }
    public override void WeaponRelease()
    {

    }
    public override void WeaponAltFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        m_chargingShot = true;
    }
    public override void WeaponAltRelease()
    {
        playerController.playerAudioAgent.PlayWeaponHit(Weapon.BOW, 1);

        m_chargingShot = false;
        m_charge = Mathf.Clamp(m_charge, 0.3f, 1.0f);
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, Hand.LEFT, m_charge, true);
        m_charge = 0.0f;
    }
}
