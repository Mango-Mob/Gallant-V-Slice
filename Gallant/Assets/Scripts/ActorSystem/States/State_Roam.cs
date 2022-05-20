using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_Roam : State
{
    public State_Roam(StateMachine _user) : base(_user) { }

    public Vector3 targetLoc;
    public override void Start()
    {
        m_myUser.m_activeStateText = "ROAM";

        float distance = 10f;
        Vector3 currPos = m_myUser.transform.position;
        NavMeshHit hit;

        if(NavMesh.SamplePosition(currPos + Random.insideUnitSphere * distance, out hit, distance, NavMesh.AllAreas))
        {
            targetLoc = hit.position;
        }
        else
        {
            m_myUser.SetState(new State_Idle(m_myUser));
        }
    }

    public override void Update()
    {
        m_myActor.SetTargetLocation(targetLoc, true);

        //Check if target exists, then transition to it.
        if (m_myActor.m_target != null && m_myActor.m_states.Contains(Type.MOVE_TO_TARGET))
        {
            m_myUser.SetState(new State_MoveToTarget(m_myUser));
            return;
        }

        if (m_myActor.m_myBrain.m_legs.IsResting())
        {
            if(m_myActor.m_states.Contains(Type.IDLE))
            {
                m_myUser.SetState(new State_Idle(m_myUser, 3f));
            }
        }
    }

    public override void End()
    {

    }
}
