using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamepath : BaseAbilityPath
{
    private SoloAudioAgent m_audioAgent;
    private float m_startVolume;
    private void Start()
    {
        m_audioAgent = GetComponentInChildren<SoloAudioAgent>();
        if (m_audioAgent)
            m_startVolume = m_audioAgent.localVolume;
    }
    private void Update()
    {
        if (m_audioAgent)
            m_audioAgent.localVolume = Mathf.Clamp01(m_startVolume * (1.0f - (m_lifeTimer / m_data.lifetime)));
    }
    protected override void AddStatusEffect(StatusEffectContainer _container)
    {
        _container.AddStatusEffect(new BurnStatus(m_data.damage * playerController.playerStats.m_abilityDamage, m_data.duration));
    }
}
