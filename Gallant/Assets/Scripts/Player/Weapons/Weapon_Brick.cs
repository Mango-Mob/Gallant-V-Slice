using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Brick : Weapon_Sword
{
    private int m_currentAttack = 0;

    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("VFX/WeaponSwings/Sword Trail");
        //m_objectPrefab = Resources.Load<GameObject>("WeaponProjectiles/BoomerangProjectile");
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
    //public override string GetWeaponName()
    //{
    //    Weapon weapon = (Weapon)m_currentAttack;

    //    return weapon.ToString()[0] + weapon.ToString().Substring(1).ToLower(); ;
    //}
    //public override void WeaponFunctionality()
    //{
    //    playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
    //    MeleeAttack(m_weaponData, transform.position);


    //    //switch (m_currentAttack)
    //    //{
    //    //    case 1:
    //    //        LongMeleeAttack(m_weaponData, transform.position);
    //    //        break;
    //    //    case 2:
    //    //        ThrowWeapon(m_weaponObject.transform.position, m_weaponData, m_hand);
    //    //        break;
    //    //    default:
    //    //        MeleeAttack(m_weaponData, transform.position);
    //    //        break;
    //    //}
    //    //m_currentAttack = Random.Range(0, 3);
    //}
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality() 
    {
        //playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        //WeaponFunctionality();
    }
    public override void WeaponAltRelease() { }
}
