using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamePatch : BaseSkillObject
{
    protected override void ApplyToActor(Actor _actor)
    {

    }
    protected override void AddStatusEffect(StatusEffectContainer _container)
    {
        _container.AddStatusEffect(new BurnStatus(m_strength, m_lifetime));
    }
}
