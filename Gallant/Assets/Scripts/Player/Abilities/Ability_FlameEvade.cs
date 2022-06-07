using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_FlameEvade: Flame evade ability
 * @author : William de Beer
 * @file : Ability_FrostEvade.cs
 * @year : 2021
 */
public class Ability_FlameEvade : AbilityBase
{
    public GameObject m_pathPrefab;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = true;
        m_isSynergyAvailable = true;
    }
    new private void Start()
    {
        base.Start();
        m_isPassive = true;

        m_pathPrefab = Resources.Load<GameObject>("Abilities/Flamepath");
        playerController = GetComponent<Player_Controller>();
    }
    public override void AbilityFunctionality()
    {

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
        if (m_pathPrefab != null)
        {
            playerController.playerAudioAgent.FireRoll();
            Transform modelTransform = playerController.playerMovement.playerModel.transform;
            
            m_lastProjectile = Instantiate(m_pathPrefab,
                modelTransform.position + 0.5f * modelTransform.forward,
                modelTransform.rotation);

            if (m_lastProjectile.GetComponent<Flamepath>() != null)
            {
                m_lastProjectile.GetComponent<Flamepath>().m_data = (m_synergyData != null) ? m_synergyData : m_data;
                m_lastProjectile.GetComponent<Flamepath>().SetEdgePoint(playerController.GetFloorPosition());
            }
        }
    }
    public override void AbilityWhileRolling()
    {
        if (m_lastProjectile != null)
            m_lastProjectile?.GetComponent<Flamepath>()?.SetEdgePoint(playerController.GetFloorPosition());
    }
    public override void AbilityOnEndRoll()
    {
        if (m_lastProjectile != null)
            m_lastProjectile?.GetComponent<Flamepath>()?.StartLife();
    }
}

