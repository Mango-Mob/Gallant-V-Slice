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
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, m_hand);
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality() 
    {
        ConeAttack(m_weaponObject.transform.position, m_weaponData, m_hand, m_pushAngle);

    }
    public override void WeaponAltRelease() { }
}
