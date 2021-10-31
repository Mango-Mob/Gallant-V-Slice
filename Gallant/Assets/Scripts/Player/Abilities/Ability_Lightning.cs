using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_Firewave: Firewave ability
 * @author : William de Beer
 * @file : Ability_Firewave.cs
 * @year : 2021
 */
public class Ability_Lightning : AbilityBase
{
    public GameObject m_boltPrefab;

    private void Start()
    {
        m_boltPrefab = Resources.Load<GameObject>("Abilities/ChainLightning");
    }
    public override void AbilityFunctionality()
    {
        Debug.Log("Lightning go bzz");
        if (m_boltPrefab != null)
        {
            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_boltPrefab,
                modelTransform.position + 0.5f * modelTransform.forward + transform.up,
                modelTransform.rotation);
            
            projectile.GetComponent<ChainLightning>().m_data = m_data;
            projectile.GetComponent<ChainLightning>().m_handTransform = 
                m_attachedHand == Hand.LEFT ? GetComponent<Player_Attack>().m_leftHandTransform : GetComponent<Player_Attack>().m_rightHandTransform;
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

