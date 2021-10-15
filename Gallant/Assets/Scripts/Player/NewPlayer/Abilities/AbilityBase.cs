﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    [Header("Ability Information")]
    public AbilityData m_data;
    private bool m_canUse = true;
    private float m_cooldownTimer = 0.0f;

    public void Update()
    {
        if (!m_canUse)
            m_cooldownTimer -= Time.deltaTime;

        m_canUse = m_cooldownTimer <= 0.0f;

        AbilityPassive();
    }
    public void TriggerAbility()
    {
        if (m_canUse)
        {
            AbilityFunctionality();
            StartCooldown();
        }
    }

    public abstract void AbilityFunctionality();
    public abstract void AbilityPassive();

    void StartCooldown()
    {
        m_cooldownTimer = m_data.cooldownTime;
        m_canUse = false;
    }

    public float GetCooldownTime()
    {
        return m_cooldownTimer / m_data.cooldownTime;
    }
}
