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
        m_combatAgent.Shuffle();
        m_combatAgent.Play(0);
    }

    public void EndCombat()
    {
        m_combatAgent.Stop();
        m_normalAgent.Shuffle();
        m_normalAgent.Play(0);
    }
}