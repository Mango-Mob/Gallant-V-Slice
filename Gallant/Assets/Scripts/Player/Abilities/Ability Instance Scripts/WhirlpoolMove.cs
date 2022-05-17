using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WhirlpoolMove : BaseAbilityProjectile
{
    [SerializeField] private float m_slowdownRate = 15.0f;
    [SerializeField] private Whirlpool m_whirlpool;

    new private void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        m_speed = Mathf.Max(0.0f, m_speed - m_slowdownRate * Time.deltaTime);
    }
    protected override void DetonateProjectile(bool hitTarget = false)
    {
        m_whirlpool.animator.SetTrigger("NadoEnd");
        m_whirlpool.EndEvent();
        BaseDetonateProjectile(hitTarget);
        Destroy(gameObject);
    }
}
