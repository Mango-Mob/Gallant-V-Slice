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
    private void Start()
    {
        m_thornsVFXPrefab = Resources.Load<GameObject>("Abilities/TempThornsVFX");
    }
    public override void AbilityFunctionality()
    {

    }
    public override void AbilityPassive()
    {

    }
    public override void AbilityOnHitRecieved(GameObject _attacker, float _damage)
    {
        if (_attacker != null)
            playerController.playerAttack.DamageTarget(_attacker, _damage * m_data.effectiveness);
        Instantiate(m_thornsVFXPrefab, transform.position + transform.up, Quaternion.Euler(-90, 0, 0));
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

