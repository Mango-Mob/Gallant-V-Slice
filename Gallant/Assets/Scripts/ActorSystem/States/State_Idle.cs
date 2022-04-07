using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : State
{
    public State_Idle(StateMachine _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        m_myUser.m_activeStateText = "IDLE";
        m_myActor.SetTargetVelocity(Vector3.zero);
    }

    public override void Update()
    {
        m_delay -= Time.deltaTime;

        //Check if there is a target to move to.
        if(m_myActor.m_target != null && m_myActor.m_states.Contains(Type.MOVE_TO_TARGET))
        {
            m_myActor.SetState(new State_MoveToTarget(m_myUser));
            return;
        }
        else if (m_myActor.m_target != null && m_myActor.m_states.Contains(Type.FLEE_FROM_TARGET))
        {
            m_myActor.SetState(new State_FleeFromTarget(m_myUser));
            return;
        }

        if (m_delay <= 0)
        {
            if (m_myActor.m_states.Contains(Type.ROAM))
            {
                m_myActor.SetState(new State_Roam(m_myUser));
            }
        }
    }

    public override void End()
    {

    }
}
