using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereScript : MonoBehaviour
{
    public JukeboxAgent m_normalAgent;
    public JukeboxAgent m_combatAgent;

    public bool IsCombatPlaying { get{ return m_combatAgent.IsPlaying(); } }

    // Start is called before the first frame update
    void Start()
    {
        m_normalAgent.Play((uint)Random.Range(0, m_normalAgent.audioClips.Count));
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