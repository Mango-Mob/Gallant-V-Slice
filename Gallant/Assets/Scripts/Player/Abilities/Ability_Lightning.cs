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

    new private void Awake()
    {
        base.Awake();
        m_isPassive = false;
    }
    new private void Start()
    {
        base.Start();
        m_boltPrefab = Resources.Load<GameObject>("Abilities/ChainLightning");
    }
    public override void AbilityFunctionality()
    {
        Debug.Log("Lightning go bzz");
        if (m_boltPrefab != null)
        {
            playerController.playerAudioAgent.Lightning();
            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_boltPrefab,
                m_handTransform.position + 0.5f * modelTransform.forward,
                modelTransform.rotation);
            
            projectile.GetComponent<ChainLightning>().m_data = m_data;
            projectile.GetComponent<ChainLightning>().m_user = playerController;
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

