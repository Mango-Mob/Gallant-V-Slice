﻿using ActorSystem.AI.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_FleeFromTarget : State
{
    public State_FleeFromTarget(StateMachine _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        m_myUser.m_activeStateText = "FLEE (idle)";

        m_myActor.SetTargetVelocity(Vector3.zero);
    }

    public override void Update()
    {
        if(m_myActor.m_target == null)
        {
            //No target, ignore and move to idle
            m_myUser.SetState(new State_Idle(m_myUser));
            return;
        }
        
        float idealDist = m_myActor.m_myBrain.m_legs.m_agent.stoppingDistance;
        float currDist = Vector3.Distance(m_myActor.transform.position, m_myActor.m_target.transform.position);
        Vector3 forward = (m_myActor.m_target.transform.position - m_myActor.transform.position).normalized;
        Vector3 sampleLoc = Vector3.zero;
        NavMeshHit sampleHit;

        if(currDist < idealDist * 0.85f)
        {
            m_myUser.m_activeStateText = "FLEE (fleeing)";
            sampleLoc = m_myActor.transform.position + forward * (currDist - idealDist);
        }
        else if(currDist > idealDist * 1.25f)
        {
            m_myUser.m_activeStateText = "FLEE (closing dist)";
            sampleLoc = m_myActor.transform.position + forward * (currDist - idealDist);
        }

        NavMesh.SamplePosition(sampleLoc, out sampleHit, m_myActor.m_myData.radius * 2f, ~0);
        if (sampleHit.hit)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(sampleLoc, sampleHit.position, ~0, path))
            {
                m_myActor.SetTargetLocation(sampleHit.position, true);
            }
        }
        else
        {
            m_myUser.m_activeStateText = "FLEE (waiting)";
            m_myActor.SetTargetOrientaion(m_myActor.m_target.transform.position);
            if (m_myActor.m_states.Contains(Type.ATTACK))
            {
                int id = m_myActor.m_myBrain.GetNextAttack();
                if (id >= 0)
                {
                    m_myUser.SetState(new State_Attack(m_myUser, id));
                }
            }
        }
        //if (userAsActor.m_animator != null && userAsActor.m_animator.m_hasVelocity)
        //    userAsActor.m_animator.SetVector3("VelocityHorizontal", "",  "VelocityVertical", userAsActor.m_legs.localVelocity.normalized, 0.25f);
        //
        //if (userAsActor.m_target == null)
        //{
        //    m_myUser.SetState(new State_Idle(m_myUser));
        //    return;
        //}
        //    
        //float dist = Vector3.Distance(userAsActor.m_target.transform.position, userAsActor.transform.position);
        //
        //if(dist < userAsActor.m_idealDistance)
        //{
        //    //MOVE
        //    Vector3 direct = userAsActor.transform.position - userAsActor.m_target.transform.position;
        //    NavMeshHit hit;
        //    if(NavMesh.SamplePosition(userAsActor.transform.position + direct.normalized, out hit, 0.5f, ~0))
        //    {
        //        userAsActor.m_legs.SetTargetLocation(hit.position, true);
        //    }
        //    else if (NavMesh.SamplePosition(userAsActor.transform.position - direct.normalized, out hit, 0.5f, ~0))
        //    {
        //        userAsActor.m_legs.SetTargetLocation(hit.position, true);
        //    }
        //}
        //else
        //{
        //    userAsActor.m_legs.SetTargetRotation(Quaternion.LookRotation((userAsActor.m_target.transform.position - userAsActor.transform.position).normalized, Vector3.up));
        //    m_delay -= Time.deltaTime;
        //
        //    //Check if there is a target to move to.
        //    userAsActor.m_legs.SetTargetRotation(Quaternion.LookRotation(userAsActor.m_target.transform.position - userAsActor.transform.position, Vector3.up));
        //    if (m_myActor.m_states.Contains(Type.ATTACK))
        //    {
        //        State_Attack.AttemptTransition(m_myUser);
        //    }
        //    else
        //    {
        //        m_myUser.SetState(new State_Idle(m_myUser));
        //    }
        //}       
    }

    public override void End()
    {

    }
}