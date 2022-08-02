using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_SummonMinion: Summon minion ability
 * @author : William de Beer
 * @file : Ability_SummonMinion.cs
 * @year : 2022
 */
public class Ability_SummonMinion : AbilityBase
{
    public GameObject m_minionPrefab;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = false;
    }
    new private void Start()
    {
        base.Start();
        m_minionPrefab = Resources.Load<GameObject>("Abilities/TerribleMinion");
    }
    public override void AbilityFunctionality()
    {
        if (m_minionPrefab != null)
        {
            playerController.playerAudioAgent.FirewaveLaunch();

            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            Vector3 targetPosition = transform.position + transform.up + modelTransform.forward * 0.5f;
            if (Physics.CheckSphere(targetPosition, 0.5f, playerController.playerMovement.m_groundLayerMask))
            {
                targetPosition = transform.position;
            }
            else
            {
                RaycastHit[] hits = Physics.RaycastAll(targetPosition, -transform.up, 2.0f, playerController.playerMovement.m_groundLayerMask);

                if (hits.Length > 0)
                {
                    targetPosition = hits[0].point;
                }
                else
                {
                    targetPosition = transform.position;
                }
            }

            GameObject projectile = Instantiate(m_minionPrefab, targetPosition, modelTransform.rotation);

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

