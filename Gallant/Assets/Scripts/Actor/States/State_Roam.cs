using Actor.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_Roam : State
{
    public State_Roam(StateMachine _user) : base(_user) { }

    public override void Start()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        userAsEnemy.m_currentStateDisplay = "ROAM";

        float distance = 10f;
        Vector3 currPos = userAsEnemy.transform.position;
        Vector3 targetPos;
        NavMeshHit hit;

        do
        {
            targetPos = currPos + Random.insideUnitSphere * distance;
        } while (!NavMesh.SamplePosition(targetPos, out hit, 1.0f, 1));

        userAsEnemy.m_legs.SetTargetLocation(hit.position, true);
    }

    public override void Update()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        if (userAsEnemy.m_animator != null && userAsEnemy.m_animator.m_hasVelocity)
            userAsEnemy.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", userAsEnemy.m_legs.localVelocity.normalized, 0.25f);

        //Check if target exists, then transition to it.
        if (userAsEnemy.m_target != null && userAsEnemy.m_myData.m_states.Contains(Type.MOVE_TO_TARGET))
        {
            m_myUser.SetState(new State_MoveToTarget(m_myUser));
            return;
        }

        if (userAsEnemy.m_legs.IsResting())
        {
            if(userAsEnemy.m_myData.m_states.Contains(Type.IDLE))
            {
                m_myUser.SetState(new State_Idle(m_myUser));
            }
        }
    }

    public override void End()
    {

    }
}
