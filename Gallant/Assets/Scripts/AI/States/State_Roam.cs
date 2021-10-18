using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_Roam : State
{
    public State_Roam(Actor _user) : base(_user) { }

    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "ROAM";

        float distance = 10f;
        Vector3 currPos = m_myActor.transform.position;
        Vector3 targetPos;
        NavMeshHit hit;

        do
        {
            targetPos = currPos + Random.insideUnitSphere * distance;
        } while (!NavMesh.SamplePosition(targetPos, out hit, 1.0f, 1));

        m_myActor.m_legs.SetTargetLocation(hit.position, true);
    }

    public override void Update()
    {
        m_myActor.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", m_myActor.m_legs.localVelocity.normalized);

        //Check if target exists, then transition to it.
        if (m_myActor.m_target != null && m_myActor.m_myData.m_states.Contains(Type.MOVE_TO_TARGET))
        {
            m_myActor.SetState(new State_MoveToTarget(m_myActor));
            return;
        }

        if (m_myActor.m_legs.IsResting())
        {
            if(m_myActor.m_myData.m_states.Contains(Type.IDLE))
            {
                m_myActor.SetState(new State_Idle(m_myActor));
            }
        }
    }

    public override void End()
    {

    }
}
