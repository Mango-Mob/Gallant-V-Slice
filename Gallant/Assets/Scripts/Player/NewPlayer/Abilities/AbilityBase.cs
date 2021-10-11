using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();

    [Header("Ability Information")]
    public AbilityData m_data;
    private bool m_canUse = true;

    public void TriggerAbility()
    {
        if (m_canUse)
        {
            OnAbilityUse.Invoke(m_data.cooldownTime);
            AbilityFunctionality();
            StartCooldown();
        }
    }

    public abstract void AbilityFunctionality();

    void StartCooldown()
    {
        StartCoroutine(Cooldown());
        IEnumerator Cooldown()
        {
            m_canUse = false;
            yield return new WaitForSeconds(m_data.cooldownTime);
            m_canUse = true;
        }
    }
}
