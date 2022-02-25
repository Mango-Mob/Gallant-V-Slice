using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_Barrier: Frost evade ability
 * @author : William de Beer
 * @file : Ability_FrostEvade.cs
 * @year : 2021
 */
public class Ability_Barrier : AbilityBase
{
    public GameObject m_projectilePrefab;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = false;
        m_isSynergyAvailable = true;
    }
    new private void Start()
    {
        base.Start();
        m_projectilePrefab = Resources.Load<GameObject>("Abilities/BarrierProjectile");
        playerController = GetComponent<Player_Controller>();
    }
    new private void Update()
    {
        base.Update();
        if (playerController.playerResources.m_barrier > 0.0f)
        {
            m_isPassive = false;
        }
        else
        {
            m_isPassive = true;
        }
    }
    public override void AbilityFunctionality()
    {
        if (playerController.playerResources.m_barrier > 0.0f && m_projectilePrefab != null)
        {
            playerController.playerAudioAgent.BarrierLaunch();
            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_projectilePrefab,
                modelTransform.position + 0.5f * modelTransform.forward + transform.up,
                modelTransform.rotation);

            projectile.GetComponent<BarrierProjectile>().m_barrierValue = playerController.playerResources.m_barrier;
            projectile.GetComponent<BarrierProjectile>().m_data = m_data;

            playerController.playerResources.ResetBarrier();
        }
    }
    public override void AbilityPassive()
    {
        playerController.playerResources.m_barrierDecayRate = m_data.duration;
    }
    public override void AbilityOnHitRecieved(GameObject _attacker, float _damage)
    {

    }
    public override void AbilityOnHitDealt(GameObject _target, float _damage)
    {
        AbilityData dataUsed = (m_synergyData != null) ? m_synergyData : m_data;
        playerController.playerResources.ChangeBarrier(_damage * dataUsed.effectiveness);
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

