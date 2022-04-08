using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Bow : WeaponBase
{
    private bool m_chargingShot = false;
    private float m_charge = 0.0f;
    private float m_chargeRate = 3.0f;
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/BowArrow");
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
        if (m_chargingShot && m_charge < 1.0f)
        {
            m_charge += Time.deltaTime * m_chargeRate * playerController.animator.GetFloat(m_hand == Hand.LEFT ? "LeftAttackSpeed" : "RightAttackSpeed");
            m_charge = Mathf.Clamp(m_charge, 0.0f, 1.0f);

            if (m_charge >= 1.0f)
            {
                playerController.playerAudioAgent.PlayOrbPickup();
            }
        }
    }
    public override void WeaponFunctionality()
    {
        m_chargingShot = true;
    }
    public override void WeaponRelease()
    {
        playerController.playerAudioAgent.PlayWeaponHit(Weapon.BOW, 1);

        m_chargingShot = false;
        m_charge = Mathf.Clamp(m_charge, 0.3f, 1.0f);
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, m_hand, m_charge, true);
        m_charge = 0.0f;
    }
    public override void WeaponAltFunctionality()
    {
        WeaponFunctionality();
    }
    public override void WeaponAltRelease() 
    {
        WeaponRelease();
    }
}
