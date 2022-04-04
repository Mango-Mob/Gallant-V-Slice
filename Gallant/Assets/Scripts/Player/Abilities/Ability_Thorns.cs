using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_Firewave: Firewave ability
 * @author : William de Beer
 * @file : Ability_Firewave.cs
 * @year : 2021
 */
public class Ability_Thorns : AbilityBase
{
    public GameObject m_thornsVFXPrefab;
    new private void Awake()
    {
        base.Awake();
        m_isPassive = true;
    }
    new private void Start()
    {
        base.Start();
        m_thornsVFXPrefab = Resources.Load<GameObject>("Abilities/ThornsBarrierVFX");
    }
    public override void AbilityFunctionality()
    {

    }
    public override void AbilityPassive()
    {

    }
    public override void AbilityOnHitRecieved(GameObject _attacker, float _damage)
    {
        GameObject barrier = Instantiate(m_thornsVFXPrefab, transform);
        playerController.playerAudioAgent.ReflectiveShield();

        AbilityData dataToUse = m_synergyData != null ? m_synergyData : m_data;

        if (_attacker != null)
        {
            playerController.playerAttack.DamageTarget(_attacker, _damage * dataToUse.effectiveness, 0, 0, CombatSystem.DamageType.Ability);
            barrier.transform.forward = (_attacker.transform.position - transform.position).normalized;
            barrier.GetComponent<ThornsVFX>().target = _attacker;
        }

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

