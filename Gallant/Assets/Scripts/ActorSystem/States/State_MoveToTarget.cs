using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_MoveToTarget : State
{
    public State_MoveToTarget(StateMachine _user) : base(_user) { }

    public override void Start()
    {
        m_myUser.m_activeStateText = "MOVING TO TARGET";

        m_myActor.SetTargetLocation(m_myActor.m_target.transform.position, true);
    }

    public override void Update()
    {
        //Transition To Idle
        if (m_myActor.m_target == null)
        {
            if (m_myActor.m_states.Contains(Type.IDLE))
            {
                m_myUser.SetState(new State_Idle(m_myUser));
            }
            return;
        }

        //Transition To Attack
        if(Vector3.Distance(m_myActor.transform.position, m_myActor.m_target.transform.position) > m_myActor.m_myBrain.m_idealDistance)
        {
            m_myActor.SetTargetLocation(m_myActor.m_target.transform.position, true);
        }
        else
        {
            m_myActor.SetTargetVelocity(Vector3.zero);
        }
        
        if(m_myActor.m_states.Contains(Type.ATTACK))
        {
            int id = m_myActor.m_myBrain.GetNextAttack();
            if(id >= 0)
            {
                m_myUser.SetState(new State_Attack(m_myUser, id));
            }
        }
    }

    public override void End()
    {

    }
}
