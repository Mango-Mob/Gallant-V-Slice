using Actor.AI;
using Actor.AI.Components;
using System.Linq;
using UnityEngine;

public class State_Attack : State
{
    public State_Attack(StateMachine _user, Actor_Attack attack = null) : base(_user) 
    { 
        if(attack != null)
            (m_myUser as Enemy).m_activeAttack = attack; 
    }

    private bool hasAttacked = false;
    public override void Start()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        userAsEnemy.m_currentStateDisplay = "ATTACK";

        if (userAsEnemy.m_myAttacks.Count == 0 && userAsEnemy.m_myData.m_states.Contains(Type.IDLE))
        {
            m_myUser.SetState(new State_Idle(m_myUser));
        }

        userAsEnemy.m_legs.Halt();

        if (userAsEnemy.m_animator != null && userAsEnemy.m_animator.m_hasVelocity)
            userAsEnemy.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", Vector3.zero);
    }

    public override void Update()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        if (!hasAttacked)
        {
            if(userAsEnemy.m_activeAttack != null)
            {
                userAsEnemy.m_activeAttack.BeginAttack(userAsEnemy);
                hasAttacked = true;
            }
            else
            {
                foreach (var attack in userAsEnemy.m_myAttacks)
                {
                    if (attack.IsWithinRange(userAsEnemy, LayerMask.NameToLayer("Player")))
                    {
                        userAsEnemy.m_activeAttack = attack;
                        userAsEnemy.m_activeAttack.BeginAttack(userAsEnemy);
                        hasAttacked = true;
                        return;
                    }
                }
                if (userAsEnemy.m_myData.m_states.Contains(Type.MOVE_TO_TARGET) && userAsEnemy.m_target != null)
                {
                    userAsEnemy.SetState(new State_MoveToTarget(m_myUser));
                }
                else if (userAsEnemy.m_myData.m_states.Contains(Type.IDLE))
                {
                    userAsEnemy.SetState(new State_Idle(m_myUser));
                }
            }
        }
        
        if((m_myUser as Enemy).m_activeAttack == null && hasAttacked)
        {
            if((m_myUser as Enemy).m_myData.m_states.Contains(Type.IDLE))
            {
                m_myUser.SetState(new State_Idle(m_myUser));
            }
            else
            {
                hasAttacked = false;
            }
        }
    }

    public override void End()
    {
        (m_myUser as Enemy).m_activeAttack = null;
    }
}
