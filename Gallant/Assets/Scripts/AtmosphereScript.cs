using Actor.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereScript : MonoBehaviour
{
    public JukeboxAgent m_normalAgent;
    public JukeboxAgent m_combatAgent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(m_combatAgent.IsPlaying() && EnemyManager.instance.m_subscribed.Count == 0)
        {
            EndCombat();
        }
        if (m_normalAgent.IsPlaying() && EnemyManager.instance.m_subscribed.Count > 0)
        {
            StartCombat();
        }

    }
    public void StartCombat()
    {
        m_normalAgent.Stop();
        m_combatAgent.Play((uint)Random.Range(0, m_combatAgent.audioClips.Count));
    }

    public void EndCombat()
    {
        m_combatAgent.Stop();
        m_normalAgent.Play((uint)Random.Range(0, m_normalAgent.audioClips.Count));
    }
}