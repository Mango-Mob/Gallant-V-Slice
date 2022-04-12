using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostPatch : BaseSkillObject
{
    protected override void ApplyToActor(Actor _actor)
    {

    }
    protected override void AddStatusEffect(StatusEffectContainer _container)
    {
        _container.AddStatusEffect(new SlowStatus(m_strength, m_lifetime));
    }
}
