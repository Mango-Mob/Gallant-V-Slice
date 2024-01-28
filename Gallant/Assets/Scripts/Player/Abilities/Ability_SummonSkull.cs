using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem.AI;

/****************
 * Ability_SummonSkull: Summon skull ability
 * @author : William de Beer
 * @file : Ability_SummonSkull.cs
 * @year : 2022
 */
public class Ability_SummonSkull : AbilityBase
{
    public GameObject m_minionPrefab;
    private List<Actor> m_hitList = new List<Actor>();
    
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
    public override void AbilityOnKill(GameObject _target)
    {
        if (m_minionPrefab != null)
        {
            playerController.playerAudioAgent.FirewaveLaunch();

            GameObject projectile = Instantiate(m_minionPrefab, _target.transform.position, Quaternion.identity);

        }
    }
}

