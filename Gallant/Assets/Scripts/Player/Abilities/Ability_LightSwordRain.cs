using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Ability_Firewave: Firewave ability
 * @author : William de Beer
 * @file : Ability_Firewave.cs
 * @year : 2021
 */
public class Ability_LightSwordRain : AbilityBase
{
    public GameObject m_rainPrefab;
    public GameObject lastObject;

    new private void Awake()
    {
        base.Awake();
        m_isPassive = false;
    }
    new private void Start()
    {
        base.Start();
        m_rainPrefab = Resources.Load<GameObject>("Abilities/LightSwordRain");
    }
    public override void AbilityFunctionality()
    {
        if (m_rainPrefab != null)
        {
            playerController.playerAudioAgent.FirewaveLaunch();

            Transform modelTransform = playerController.playerMovement.playerModel.transform;

            GameObject projectile = Instantiate(m_rainPrefab,
                modelTransform.position,
                modelTransform.rotation);

            projectile.GetComponent<LightSwordRain>().m_data = m_data;

            lastObject = projectile;
        }
        else
        {
            Debug.Log("Prefab not found");
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
    public override void AbilityOnKill(GameObject _target)
    {

    }
}

