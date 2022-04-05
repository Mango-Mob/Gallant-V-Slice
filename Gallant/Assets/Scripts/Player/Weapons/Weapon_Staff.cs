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

        projectile.GetComponent<BasePlayerProjectile>().m_overrideHitVFXColor = true;
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality() 
    {
        GameObject vfx = ConeAttack(m_weaponObject.transform.position, m_weaponData, m_hand, m_pushAngle);

        if (m_weaponData.abilityData != null)
        {
            ParticleSystem[] particleSystems = vfx.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particle.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(m_weaponData.abilityData.droppedEnergyColor);
            }
        }
    }
    public override void WeaponAltRelease() { }
}
