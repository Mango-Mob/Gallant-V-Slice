using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Dagger : WeaponBase
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
    }
    public override void WeaponFunctionality()
    {
        
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        
    }
    public override void WeaponAltRelease() { }
}
