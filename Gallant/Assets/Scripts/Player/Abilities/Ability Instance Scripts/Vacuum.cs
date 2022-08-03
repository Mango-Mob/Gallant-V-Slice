using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Vacuum : BaseAbilityPath
{
    protected override void ActorEffect(Actor _actor)
    {
        Vector3 direction = (transform.position - _actor.transform.position).normalized;
        _actor.KnockbackActor(direction * m_data.effectiveness);
    }
    protected override void AddStatusEffect(StatusEffectContainer _container)
    {
        _container.AddStatusEffect(new SlowStatus(0.5f, m_data.duration));
    }
    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
