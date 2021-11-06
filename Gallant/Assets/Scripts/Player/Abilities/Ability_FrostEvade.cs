using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_FrostEvade: Frost evade ability
 * @author : William de Beer
 * @file : Ability_FrostEvade.cs
 * @year : 2021
 */
public class Ability_FrostEvade : AbilityBase
{
    public GameObject m_pathPrefab;
    private GameObject m_lastProjectile;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = true;
    }
    private void Start()
    {
        m_isPassive = true;

        m_pathPrefab = Resources.Load<GameObject>("Abilities/Frostpath");
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
        Debug.Log("Ice go woosh");
        if (m_pathPrefab != null)
        {
            Transform modelTransform = playerController.playerMovement.playerModel.transform;
            
            m_lastProjectile = Instantiate(m_pathPrefab,
                modelTransform.position + 0.5f * modelTransform.forward,
                modelTransform.rotation);

            if (m_lastProjectile.GetComponent<Frostpath>() != null)
            {
                m_lastProjectile.GetComponent<Frostpath>().m_data = m_data;
                m_lastProjectile.GetComponent<Frostpath>().SetEdgePoint(transform.position);
            }
        }
    }
    public override void AbilityWhileRolling()
    {
        m_lastProjectile.GetComponent<Frostpath>().SetEdgePoint(transform.position);
    }
    public override void AbilityOnEndRoll()
    {
        m_lastProjectile.GetComponent<Frostpath>().StartLife();
    }
}

