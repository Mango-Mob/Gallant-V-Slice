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

        m_myActor.m_legs.SetTargetLocation(m_myActor.m_target.transform.position, true);
    }

    public override void Update()
    {
        if (m_myActor.m_animator != null && m_myActor.m_animator.m_hasVelocity)
            m_myActor.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", m_myActor.m_legs.localVelocity.normalized);

        if (m_myActor.m_target == null)
        {
            if (m_myActor.m_myData.m_states.Contains(Type.IDLE))
            {
                m_myActor.SetState(new State_Idle(m_myActor));
            }
            else
            {
                m_myActor.m_legs.SetTargetLocation(m_myActor.transform.position, true);
            }
            return;
        }

        if(Vector3.Distance(m_myActor.transform.position, m_myActor.m_target.transform.position) > m_myActor.m_idealDistance)
        {
            m_myActor.m_legs.SetTargetLocation(m_myActor.m_target.transform.position, true);
        }
        else
        {
            m_myActor.m_legs.Halt();
        }
        
        if(m_myActor.m_myData.m_states.Contains(Type.ATTACK))
        {
            List<Actor_Attack> currentAttacks = new List<Actor_Attack>(m_myActor.m_myAttacks);
            currentAttacks.Sort(new AttackPrioritySort(m_myActor));

            foreach (var attack in currentAttacks)
            {
                if (attack.IsWithinRange(m_myActor, LayerMask.NameToLayer("Player")) && attack.IsAvailable())
                {
                    m_myActor.SetState(new State_Attack(m_myActor, attack));
                    return;
                }
            }
        }
    }

    public override void End()
    {

    }
}
