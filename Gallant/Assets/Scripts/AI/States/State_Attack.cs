using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class State_Attack : State
{
    public State_Attack(Actor _user, Actor_Attack attack = null) : base(_user) 
    { 
        if(attack != null)
            m_myActor.m_activeAttack = attack; 
    }

    private bool hasAttacked = false;
    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "ATTACK";

        if (m_myActor.m_myAttacks.Count == 0 && m_myActor.m_myData.m_states.Contains(Type.IDLE))
        {
            m_myActor.SetState(new State_Idle(m_myActor));
        }

        m_myActor.m_legs.Halt();
        m_myActor.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", Vector3.zero);
    }

    public override void Update()
    {
        if(!hasAttacked)
        {
            if(m_myActor.m_activeAttack != null)
            {
                m_myActor.m_activeAttack.BeginAttack(m_myActor);
                hasAttacked = true;
            }
            else
            {
                foreach (var attack in m_myActor.m_myAttacks)
                {
                    if (attack.IsWithinRange(m_myActor, LayerMask.NameToLayer("Player")))
                    {
                        m_myActor.m_activeAttack = attack;
                        m_myActor.m_activeAttack.BeginAttack(m_myActor);
                        hasAttacked = true;
                        return;
                    }
                }
                if (m_myActor.m_myData.m_states.Contains(Type.MOVE_TO_TARGET) && m_myActor.m_target != null)
                {
                    m_myActor.SetState(new State_MoveToTarget(m_myActor));
                }
                else if (m_myActor.m_myData.m_states.Contains(Type.IDLE))
                {
                    m_myActor.SetState(new State_Idle(m_myActor));
                }
            }
        }

        if(m_myActor.m_activeAttack == null && hasAttacked)
        {
            if(m_myActor.m_myData.m_states.Contains(Type.IDLE))
            {
                m_myActor.SetState(new State_Idle(m_myActor));
            }
            else
            {
                hasAttacked = false;
            }
        }
    }

    public override void End()
    {
        m_myActor.m_activeAttack = null;
    }
}
