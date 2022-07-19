using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class Weapon_Shield : WeaponBase
{
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/ShieldProjectile");
        base.Awake();
    }

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        if (m_hand == Hand.LEFT)
        {
            m_weaponObject.transform.localScale *= -1.0f;
        }
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
        if (m_hand == Hand.LEFT && playerController.playerAttack.m_isBlocking && playerController.playerResources.m_isExhausted)
        {
            playerController.playerAudioAgent.PlayShieldBlock(); // Guard break audio
            playerController.animator.SetTrigger("HitPlayer");
            playerController.playerAttack.ToggleBlock(false);
        }
    }
    public override void WeaponFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        ThrowWeapon(m_weaponObject.transform.position, m_weaponData, m_hand);
    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        MeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight);
        BeginBlock();
    }
    public override void WeaponAltRelease() { }
}
