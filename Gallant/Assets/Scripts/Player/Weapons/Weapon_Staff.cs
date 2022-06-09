using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Staff : WeaponBase
{
    float m_pushAngle = 25.0f;
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/StaffArcaneBolt");
        m_objectAltPrefab = Resources.Load<GameObject>("VFX/ArcanePush");

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
            meshRenderers[meshCount - 1].material.color = m_weaponData.abilityData.droppedEnergyColor;
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
    }
    public override void WeaponFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);

        GameObject projectile = ShootProjectile(m_weaponObject.transform.position, m_weaponData, m_hand);

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
    public override void WeaponAltFunctionality() 
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);

        ConeAttack(m_weaponObject.transform.position, m_weaponData, m_hand, m_pushAngle);

        GameObject VFX = SpawnVFX(m_objectAltPrefab, transform.position + transform.up, playerController.playerMovement.playerModel.transform.rotation);
        VFX.transform.localScale *= (m_weaponData.altHitCenterOffset + m_weaponData.altHitSize) * 1.0f;
        VFX.transform.SetParent(transform);

        VFX.transform.position += playerController.playerMovement.playerModel.transform.forward * 1.5f;
    }
    public override void WeaponAltRelease() { }
}
