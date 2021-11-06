using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * AbilityBase: Base class for ability
 * @author : William de Beer
 * @file : AbilityBase.cs
 * @year : 2021
 */
public abstract class AbilityBase : MonoBehaviour
{
    [Header("Ability Information")]
    public AbilityData m_data;
    public Hand m_attachedHand; 
    public bool m_isPassive { get; protected set; } = false;
    public bool m_canUse { get; private set; } = true;
    private float m_cooldownTimer = 0.0f;
    protected Player_Controller playerController;
    protected void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }
    public void Update()
    {
        if (!m_canUse)
            m_cooldownTimer -= Time.deltaTime;

        m_canUse = m_cooldownTimer <= 0.0f;

        AbilityPassive();
    }
    public void TriggerAbility()
    {
        AbilityFunctionality();
    }
    public abstract void AbilityFunctionality();
    public abstract void AbilityPassive();
    public abstract void AbilityOnHitRecieved(GameObject _attacker, float _damage);
    public abstract void AbilityOnHitDealt(GameObject _target, float _damage);
    public abstract void AbilityOnBeginRoll();
    public abstract void AbilityWhileRolling();
    public abstract void AbilityOnEndRoll();

    public void StartCooldown()
    {
        m_cooldownTimer = m_data.cooldownTime * playerController.playerStats.m_abilityCD;
        m_canUse = false;
    }

    public float GetCooldownTime()
    {
        return m_cooldownTimer / (m_data.cooldownTime * playerController.playerStats.m_abilityCD);
    }
}
