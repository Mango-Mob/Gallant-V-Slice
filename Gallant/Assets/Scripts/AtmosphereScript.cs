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