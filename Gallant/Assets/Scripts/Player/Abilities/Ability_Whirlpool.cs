using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_Whirlpool: Whirl pool ability
 * @author : William de Beer
 * @file : Ability_SandMissile.cs
 * @year : 2021
 */
public class Ability_Whirlpool : AbilityBase
{
    public GameObject m_boltPrefab;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = false;
    }
    new private void Start()
    {
        base.Start();
        m_boltPrefab = Resources.Load<GameObject>("Abilities/WhirlpoolProjectile");
    }
    public override void AbilityFunctionality()
    {
        if (m_boltPrefab != null)
        {
            playerController.playerAudioAgent.Lightning();
            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_boltPrefab,
                m_handTransform.position + 0.5f * modelTransform.forward,
                modelTransform.rotation);

            projectile.GetComponent<BaseAbilityProjectile>().m_data = m_data;
            projectile.GetComponent<BaseAbilityProjectile>().playerController = playerController;
        }
    }
    public override void AbilityPassive()
    {

    }
    public override void AbilityOnHitRecieved(GameObject _attacker, float _damage)
    {

    }
    public override void AbilityOnHitDealt(GameObject _target, float _damage)
    {

    }
    public override void AbilityOnBeginRoll()
    {

    }
    public override void AbilityWhileRolling()
    {

    }
    public override void AbilityOnEndRoll()
    {

    }
}

