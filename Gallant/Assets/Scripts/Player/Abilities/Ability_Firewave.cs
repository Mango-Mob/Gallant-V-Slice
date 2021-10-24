using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_Firewave: Firewave ability
 * @author : William de Beer
 * @file : Ability_Firewave.cs
 * @year : 2021
 */
public class Ability_Firewave : AbilityBase
{
    public GameObject m_wavePrefab;
    private Player_Controller playerController;

    private void Start()
    {
        m_wavePrefab = Resources.Load<GameObject>("Abilities/FirewaveProjectile");
        playerController = GetComponent<Player_Controller>();
    }
    public override void AbilityFunctionality()
    {
        Debug.Log("Fire go woosh");
        if (m_wavePrefab != null)
        {
            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_wavePrefab,
                modelTransform.position + 0.5f * modelTransform.forward + transform.up,
                modelTransform.rotation);
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
    public override void AbilityOnRoll()
    {

    }
    public override void AbilityWhileRolling()
    {

    }
}

