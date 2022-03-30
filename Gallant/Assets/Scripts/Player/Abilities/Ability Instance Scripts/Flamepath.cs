using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamepath : BaseAbilityPath
{
    protected override void AddStatusEffect(StatusEffectContainer _container)
    {
        _container.AddStatusEffect(new BurnStatus(m_data.damage, m_data.duration));
    }
}
