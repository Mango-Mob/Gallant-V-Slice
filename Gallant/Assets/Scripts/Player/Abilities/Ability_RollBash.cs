using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_RollBash: Roll bash ability
 * @author : William de Beer
 * @file : Ability_RollBash.cs
 * @year : 2022
 */
public class Ability_RollBash : AbilityBase
{
    public GameObject m_objectPrefab;
    private GameObject m_lastProjectile;

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

        m_objectPrefab = Resources.Load<GameObject>("Abilities/Rollbash");
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
        if (m_objectPrefab != null)
        {
            playerController.playerAudioAgent.FirewaveLaunch();
            Transform modelTransform = playerController.playerMovement.playerModel.transform;
            
            m_lastProjectile = Instantiate(m_objectPrefab, transform);
            m_lastProjectile.transform.position += 0.6f * transform.up;

            if (m_lastProjectile.GetComponent<Rollbash>() != null)
            {
                m_lastProjectile.GetComponent<Rollbash>().m_data = (m_synergyData != null) ? m_synergyData : m_data;
                m_lastProjectile.GetComponent<Rollbash>().playerController = playerController;
            }
        }
    }
    public override void AbilityWhileRolling()
    {

    }
    public override void AbilityOnEndRoll()
    {
        if (m_lastProjectile != null)
            m_lastProjectile.GetComponent<Rollbash>().Destruct();
    }
}

