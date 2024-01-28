using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

/****************
 * Ability_VacuumRoll: Vacuum roll ability
 * @author : William de Beer
 * @file : Ability_VacuumRoll.cs
 * @year : 2021
 */
public class Ability_VacuumRoll : AbilityBase
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

        m_pathPrefab = Resources.Load<GameObject>("Abilities/Vacuum");
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
            playerController.playerAudioAgent.Iceroll();
            Transform modelTransform = playerController.playerMovement.playerModel.transform;
            
            m_lastProjectile = Instantiate(m_pathPrefab,
                modelTransform.position + 0.5f * modelTransform.forward,
                modelTransform.rotation);

            m_lastProjectile.transform.forward = modelTransform.forward;

            if (m_lastProjectile.GetComponent<Vacuum>() != null)
            {
                m_lastProjectile.GetComponent<Vacuum>().m_data = (m_synergyData != null) ? m_synergyData : m_data;
                m_lastProjectile.GetComponent<Vacuum>().SetEdgePoint(playerController.GetFloorPosition());
            }
        }
    }
    public override void AbilityWhileRolling()
    {
        if (m_lastProjectile != null)
            m_lastProjectile.GetComponent<Vacuum>().SetEdgePoint(playerController.GetFloorPosition());
    }
    public override void AbilityOnEndRoll()
    {
        if (m_lastProjectile != null)
            m_lastProjectile?.GetComponent<Vacuum>()?.StartLife();
    }
    public override void AbilityOnKill(GameObject _target)
    {

    }
}

