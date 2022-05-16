using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Hammer : Weapon_Sword
{
    private GameObject m_vfxPrefab;
    new private void Awake()
    {
        m_vfxPrefab = Resources.Load<GameObject>("VFX/GroundSlamVFX");
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
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality() 
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);

        GroundSlam(m_weaponData, transform.position, Hand.NONE, new StunStatus(2.0f));
        playerController.playerAudioAgent.PlayWeaponHit(m_weaponData.weaponType);
        GameObject newObject = Instantiate(m_vfxPrefab, transform.position + playerController.playerMovement.playerModel.transform.forward * m_weaponData.altHitCenterOffset, Quaternion.identity);
        newObject.transform.localScale *= m_weaponData.altHitSize * 4.0f;
    }
    public override void WeaponAltRelease() { }
}
