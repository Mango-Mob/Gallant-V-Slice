using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor_Legs : MonoBehaviour
{
    protected NavMeshAgent m_agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Halt()
    {
        m_agent.isStopped = true;
    }
}
