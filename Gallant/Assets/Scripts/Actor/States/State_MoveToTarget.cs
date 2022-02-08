﻿using Actor.AI;
using Actor.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_MoveToTarget : State
{
    public State_MoveToTarget(StateMachine _user) : base(_user) { }

    public override void Start()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        userAsEnemy.m_currentStateDisplay = "MOVING TO TARGET";

        userAsEnemy.m_legs.SetTargetLocation(userAsEnemy.m_target.transform.position, true);
    }

    public override void Update()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        if (userAsEnemy.m_animator != null && userAsEnemy.m_animator.m_hasVelocity)
            userAsEnemy.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", userAsEnemy.m_legs.localVelocity.normalized);

        if (userAsEnemy.m_target == null)
        {
            if (userAsEnemy.m_myData.m_states.Contains(Type.IDLE))
            {
                m_myUser.SetState(new State_Idle(m_myUser));
            }
            else
            {
                userAsEnemy.m_legs.SetTargetLocation(userAsEnemy.transform.position, true);
            }
            return;
        }

        if(Vector3.Distance(userAsEnemy.transform.position, userAsEnemy.m_target.transform.position) > userAsEnemy.m_idealDistance)
        {
            userAsEnemy.m_legs.SetTargetLocation(userAsEnemy.m_target.transform.position, true);
        }
        else
        {
            userAsEnemy.m_legs.Halt();
        }
        
        if(userAsEnemy.m_myData.m_states.Contains(Type.ATTACK))
        {
            List<AttackData> currentAttacks = new List<AttackData>(userAsEnemy.m_myAttacks);
            
            currentAttacks.Sort(new AttackPrioritySort());
            for (int i = currentAttacks.Count - 1; i >= 0; i--)
            {
                if (!currentAttacks[i].IsReady || !currentAttacks[i].IsOverlaping(m_myUser.transform, LayerMask.NameToLayer("Player")))
                {
                    currentAttacks.RemoveAt(i);
                }
            }

            if (currentAttacks.Count > 0)
            {
                m_myUser.SetState(new State_Attack(m_myUser, currentAttacks[0]));
            }
        }
    }

    public override void End()
    {

    }
}
