using ActorSystem.AI;
using ActorSystem.AI.Components;
using ActorSystem.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_Attack : State
{
    private int selectedAttack;
    private AttackData attackData;

    private bool hasAttacked = false;

    public State_Attack(StateMachine _user, int attackID = -1) : base(_user) 
    { 
        if(attackID >= 0)
        {
            selectedAttack = attackID;
            attackData = m_myActor.GetAttack(attackID);
        }   
    }
    public override void Start()
    {
        m_myUser.m_activeStateText = "ATTACK";

        if (selectedAttack < 0 && m_myActor.m_states.Contains(Type.IDLE))
        {
            m_myUser.SetState(new State_Idle(m_myUser));
        }
    }

    public override void Update()
    {
        if(m_myActor.m_myBrain.m_arms != null && !m_myActor.m_myBrain.m_arms.m_activeAttack.HasValue)
        {
            //Search for target
            if (!hasAttacked)
            {
                m_myActor.m_myBrain.BeginAttack(selectedAttack);
                hasAttacked = true;
            }
            else
            {
                //user out of range, try again
                if (m_myActor.m_states.Contains(Type.MOVE_TO_TARGET) && m_myActor.m_target != null)
                {
                    m_myUser.SetState(new State_MoveToTarget(m_myUser));
                }
                else if(m_myActor.m_states.Contains(Type.FLEE_FROM_TARGET) && m_myActor.m_target != null)
                {
                    m_myUser.SetState(new State_FleeFromTarget(m_myUser));
                }
                else if (m_myActor.m_states.Contains(Type.IDLE))
                {
                    m_myUser.SetState(new State_Idle(m_myUser));
                }
            }
            
        }
    }

    public override void End()
    {
        m_myActor.m_myBrain.EndAttack();
    }
}
