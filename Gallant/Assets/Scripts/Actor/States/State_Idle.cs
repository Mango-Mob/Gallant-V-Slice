using Actor.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : State
{
    public State_Idle(StateMachine _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        userAsEnemy.m_currentStateDisplay = "IDLE";

        if (userAsEnemy.m_animator != null && userAsEnemy.m_animator.m_hasVelocity)
            userAsEnemy.m_animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero, 0.25f);

        userAsEnemy.m_legs?.Halt();
    }

    public override void Update()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        m_delay -= Time.deltaTime;

        //Check if there is a target to move to.
        if(userAsEnemy.m_target != null && userAsEnemy.m_myData.m_states.Contains(Type.MOVE_TO_TARGET))
        {
            userAsEnemy.SetState(new State_MoveToTarget(m_myUser));
            return;
        }

        if(m_delay <= 0)
        {
            if (userAsEnemy.m_myData.m_states.Contains(Type.ROAM))
            {
                userAsEnemy.SetState(new State_Roam(m_myUser));
            }
        }

        if(userAsEnemy.m_target != null && userAsEnemy.m_myData.m_states.Contains(Type.KEEP_AWAY_FROM_TARGET))
        {
            userAsEnemy.SetState(new State_KeepAwayFromTarget(m_myUser));
        }
    }

    public override void End()
    {

    }
}
