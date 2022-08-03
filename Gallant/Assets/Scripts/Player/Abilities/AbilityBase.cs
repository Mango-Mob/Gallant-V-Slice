using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public enum PassiveType
{
    PASSIVE,
    HIT_RECIEVED,
    HIT_DEALT,
    BEGIN_ROLL,
    WHILE_ROLLING,
    END_ROLL,
}

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
    public Transform m_handTransform;
    public bool m_isPassive { get; protected set; } = false;
    public bool m_canUse { get; private set; } = true;
    public float m_cooldownTimer { get; private set; } = 0.0f;
    protected Player_Controller playerController;
    protected GameObject m_lastProjectile;


    [Header("Double Synergy")]
    public AbilityData m_synergyData;
    public bool m_isSynergyAvailable { get; protected set; } = false;

    protected void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }
    protected void Start()
    {
        m_cooldownTimer = m_data.lastCooldown;

        if (m_attachedHand == Hand.LEFT)
            m_handTransform = playerController.playerAttack.m_leftHandTransform;
        else
            m_handTransform = playerController.playerAttack.m_rightHandTransform;
    }
    public void Update()
    {
        //if (!m_canUse)
        //    m_cooldownTimer -= Time.deltaTime;

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
        m_cooldownTimer = m_data.cooldownTime;
        m_canUse = false;
    }
    public void ReduceCooldown(float _amount)
    {
        m_cooldownTimer -= _amount;
    }

    public float GetCooldownTime()
    {
        if (m_data.isPassive)
            return 1.0f;

        float cooldownTime = (m_cooldownTimer / (m_data.cooldownTime));
        if (cooldownTime >= 1.0f)
            return 0.0f;

        return 1.0f - cooldownTime;
    }
}
