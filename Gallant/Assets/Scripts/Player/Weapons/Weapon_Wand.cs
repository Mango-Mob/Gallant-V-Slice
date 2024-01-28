using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;
public class Weapon_Wand : WeaponBase
{
    float m_pushAngle = 25.0f;
    private GameObject m_tomeObject;
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/StaffArcaneBolt");
        m_objectAltPrefab = Resources.Load<GameObject>("WeaponProjectiles/WandLaser");

        m_overrideHitVFXPrefab = Resources.Load<GameObject>("VFX/ArcaneHit");
        m_overrideAltHitVFXPrefab = Resources.Load<GameObject>("VFX/ArcaneHit");

        m_isVFXColored = true;
        m_isAltVFXColored = true;
        base.Awake();
    }

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        MeshRenderer[] meshRenderers = m_weaponObject.GetComponentsInChildren<MeshRenderer>();
        int meshCount = meshRenderers.Length;

        if (meshCount > 1 && m_weaponData.abilityData != null)
        {
            meshRenderers[1].material.color = m_weaponData.abilityData.droppedEnergyColor;
            meshRenderers[meshCount - 2].material.color = m_weaponData.abilityData.droppedEnergyColor;
        }

        for (int i = 0; i < m_weaponObject.transform.childCount; i++)
        {
            if (m_weaponObject.transform.GetChild(i).name == "Tome")
            {
                m_tomeObject = m_weaponObject.transform.GetChild(i).gameObject;
                m_tomeObject.transform.SetParent(playerController.playerAttack.m_leftHandTransform);

                m_tomeObject.transform.localPosition = Vector3.zero;
                m_tomeObject.transform.localRotation = Quaternion.identity;
            }
        }
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();

        if (m_beamObject)
        {
            playerController.playerResources.ChangeStamina(-35.0f * Time.deltaTime);
        }

        if ((playerController.playerAttack.GetCurrentUsedHand() == Hand.NONE && !playerController.animator.GetBool("UsingLeft")) || playerController.Stamina <= 0.0f)
        {
            WeaponAltRelease();
            playerController.animator.SetBool("UsingLeft", false);
        }
    }
    private void OnDestroy()
    {
        Destroy(m_weaponObject);
        Destroy(m_tomeObject);
    }
    public override void WeaponFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);

        GameObject projectile = ShootProjectile(m_weaponObject.transform.position, m_weaponData, Hand.RIGHT);

        if (m_weaponData.abilityData != null)
        {
            ParticleSystem[] particleSystems = projectile.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particle.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(m_weaponData.abilityData.droppedEnergyColor);
            }
        }
    }
    public override void WeaponRelease() { }

    GameObject m_beamObject;
    public override void WeaponAltFunctionality() 
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);

        if (m_beamObject)
            Destroy(m_beamObject);

        m_beamObject = ShootProjectile(transform.position + playerController.playerMovement.playerModel.transform.forward + Vector3.up * playerController.playerAttack.m_swingHeight,
            m_weaponData, Hand.LEFT);
        m_beamObject.transform.SetParent(playerController.playerMovement.playerModel.transform);
    }
    public override void WeaponAltRelease() 
    {
        if (m_beamObject)
            m_beamObject.GetComponent<BasePlayerProjectile>().Destruct();
    }
}
