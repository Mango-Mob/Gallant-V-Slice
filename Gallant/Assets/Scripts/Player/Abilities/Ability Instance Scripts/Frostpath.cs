using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Frostpath : BaseAbilityPath
{
    protected override void AddStatusEffect(StatusEffectContainer _container)
    {
        _container.AddStatusEffect(new SlowStatus(m_data.effectiveness, m_data.duration));
    }
}
