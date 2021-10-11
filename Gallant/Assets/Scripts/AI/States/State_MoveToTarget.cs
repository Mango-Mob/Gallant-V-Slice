using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_MoveToTarget : State
{
    public State_MoveToTarget(Actor _user) : base(_user) { }

    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "MOVING TO TARGET";

        m_myActor.legs.SetTargetLocation(m_myActor.m_target.transform.position, true);
    }

    public override void Update()
    {
        m_myActor.animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", m_myActor.legs.localVelocity.normalized);

        if (m_myActor.m_target == null)
        {
            if (m_myActor.m_myData.m_states.Contains(Type.IDLE))
            {
                m_myActor.SetState(new State_Idle(m_myActor));
            }
            else
            {
                m_myActor.legs.SetTargetLocation(m_myActor.transform.position, true);
            }
            return;
        }
        m_myActor.legs.SetTargetLocation(m_myActor.m_target.transform.position, true);
    }

    public override void End()
    {

    }
}
