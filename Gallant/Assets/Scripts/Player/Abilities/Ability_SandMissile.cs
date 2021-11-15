using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_SandMissile: Sand missile ability
 * @author : William de Beer
 * @file : Ability_SandMissile.cs
 * @year : 2021
 */
public class Ability_SandMissile : AbilityBase
{
    public GameObject m_missilePrefab;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = false;
    }
    new private void Start()
    {
        base.Start();
        m_missilePrefab = Resources.Load<GameObject>("Abilities/SandmissileProjectile");
    }
    public override void AbilityFunctionality()
    {
        Debug.Log("Fire go woosh");
        if (m_missilePrefab != null)
        {
            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_missilePrefab,
                modelTransform.position + 0.5f * modelTransform.forward + transform.up,
                modelTransform.rotation);

            projectile.GetComponent<SandmissileProjectile>().m_data = m_data;
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

