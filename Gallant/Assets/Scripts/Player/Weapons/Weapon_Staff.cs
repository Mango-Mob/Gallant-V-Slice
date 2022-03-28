using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Staff : Weapon_Sword
{
    new private void Awake()
    {
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
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality() { }
    public override void WeaponAltRelease() { }
}
