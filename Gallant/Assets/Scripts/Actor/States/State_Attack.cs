using Actor.AI;
using Actor.AI.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_Attack : State
{

    private AttackData selectedAttack;
    private bool hasAttacked = false;

    public State_Attack(StateMachine _user, AttackData attack = null) : base(_user) 
    { 
        if(attack != null)
        {
            selectedAttack = attack;
        }   
    }

    public static void AttemptTransition(StateMachine _user)
    {
        Enemy userAsEnemy = (_user as Enemy);
        List<AttackData> currentAttacks = new List<AttackData>(userAsEnemy.m_myAttacks);

        currentAttacks.Sort(new AttackPrioritySort());
        for (int i = currentAttacks.Count - 1; i >= 0; i--)
        {
            if (!currentAttacks[i].IsReady || !currentAttacks[i].IsOverlaping(_user.transform, LayerMask.NameToLayer("Player")))
            {
                currentAttacks.RemoveAt(i);
            }
        }

        if (currentAttacks.Count > 0)
        {
            _user.SetState(new State_Attack(_user, currentAttacks[0]));
        }
    }

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

        if(userAsEnemy.m_activeAttack == null)
        {
            //Search for target
            if (!hasAttacked && selectedAttack.IsOverlaping(m_myUser.transform, LayerMask.NameToLayer("Player")))
            {
                userAsEnemy.BeginAttack(selectedAttack);
                hasAttacked = true;
            }
            else
            {
                //user out of range, try again
                if (userAsEnemy.m_myData.m_states.Contains(Type.MOVE_TO_TARGET) && userAsEnemy.m_target != null)
                {
                    userAsEnemy.SetState(new State_MoveToTarget(m_myUser));
                }
                else if(userAsEnemy.m_myData.m_states.Contains(Type.MOVE_TO_TARGET) && userAsEnemy.m_target != null)
                {
                    userAsEnemy.SetState(new State_KeepAwayFromTarget(m_myUser));
                }
                else if (userAsEnemy.m_myData.m_states.Contains(Type.IDLE))
                {
                    userAsEnemy.SetState(new State_Idle(m_myUser));
                }
            }
            
        }
    }

    public override void End()
    {
        (m_myUser as Enemy).m_activeAttack = null;
    }
}
