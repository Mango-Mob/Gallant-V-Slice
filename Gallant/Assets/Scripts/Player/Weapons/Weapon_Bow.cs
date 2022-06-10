using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Bow : WeaponBase
{
    private bool m_chargingShot = false;
    private float m_charge = 0.0f;
    private float m_chargeRate = 0.8f;
    private bool m_particlesPlaying = false;
    private BowString m_bowString;
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

        m_bowString = GetComponentInChildren<BowString>();
        m_bowString.m_bindTransform = playerController.playerAttack.m_leftHandTransform;
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();

        float speedMult = playerController.animator.GetFloat(m_hand == Hand.LEFT ? "LeftAttackSpeed" : "RightAttackSpeed");
        if (m_chargingShot && m_charge < 1.0f)
        {
            m_charge += Time.deltaTime * m_chargeRate * speedMult;
            m_charge = Mathf.Clamp(m_charge, 0.0f, 1.0f);

            if (m_charge >= 1.0f)
            {
                playerController.playerAudioAgent.PlayOrbPickup();
            }
        }
        if (m_chargingShot)
        {
            if (!playerController.animator.GetBool("UsingLeft"))
                m_chargingShot = false;
            playerController.playerResources.ChangeStamina(-12.0f * Time.deltaTime * speedMult);

            if (playerController.playerResources.m_stamina <= 0.0f)
            {
                playerController.animator.SetBool("LeftAttackHeld", false);
                m_chargingShot = false;
            }
        }
        else
        {
            m_charge = 0.0f;
        }

        if (m_charge >= 1.0f && !m_particlesPlaying)
        {
            foreach (var particle in m_weaponTrailParticles)
            {
                particle.Play();
            }
            m_particlesPlaying = true;
        }
        else if (m_charge < 1.0f && m_particlesPlaying)
        {
            foreach (var particle in m_weaponTrailParticles)
            {
                particle.Stop();
            }
            m_particlesPlaying = false;
        }
        
        if (m_bowString != null)
        {
            m_bowString.m_lerp = Mathf.Clamp01(m_bowString.m_lerp + (m_chargingShot ? 15.0f : -1.0f) * Time.deltaTime);
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
        m_charge = Mathf.Clamp(m_charge, 0.1f, 1.0f);
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, Hand.LEFT, m_charge, true);
        m_charge = 0.0f;
    }
}
