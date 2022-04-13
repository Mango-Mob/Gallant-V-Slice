using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Crossbow : WeaponBase
{
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/CrossbowBolt");
        m_objectAltPrefab = Resources.Load<GameObject>("WeaponProjectiles/CrossbowBoltAlt");
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
        if (!playerController.animator.IsInTransition(playerController.animator.GetLayerIndex("Arm")))
        {
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            ShootProjectile(m_weaponObject.transform.position, m_weaponData, m_hand);
        }
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        ShootProjectile(m_weaponObject.transform.position, m_weaponData, m_hand);
    }
    public override void WeaponAltRelease() { }
}
