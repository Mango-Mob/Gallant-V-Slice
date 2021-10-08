using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbilityBase : MonoBehaviour
{
    public class MyFloatEvent : UnityEvent<float> { }
    public MyFloatEvent OnAbilityUse = new MyFloatEvent();

    [Header("Ability Information")]
    public string m_title;
    public Sprite m_icon;
    public float m_cooldownTime = 1.0f;
    private bool m_canUse = true;

    public void TriggerAbility()
    {
        
        if (m_canUse)
        {
            OnAbilityUse.Invoke(m_cooldownTime);
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
            yield return new WaitForSeconds(m_cooldownTime);
            m_canUse = true;
        }
    }
}
