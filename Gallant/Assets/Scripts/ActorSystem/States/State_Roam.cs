﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_Roam : State
{
    public State_Roam(StateMachine _user) : base(_user) { }

    public override void Start()
    {
        m_myUser.m_activeStateText = "ROAM";

        float distance = 10f;
        Vector3 currPos = m_myUser.transform.position;
        Vector3 targetPos;
        NavMeshHit hit;

        do
        {
            targetPos = currPos + Random.insideUnitSphere * distance;
        } while (!NavMesh.SamplePosition(targetPos, out hit, 1.0f, 1));

        m_myActor.SetTargetLocation(hit.position, true);
    }

    public override void Update()
    {
        //Check if target exists, then transition to it.
        if (m_myActor.m_target != null && m_myActor.m_states.Contains(Type.MOVE_TO_TARGET))
        {
            m_myUser.SetState(new State_MoveToTarget(m_myUser));
            return;
        }

        if (m_myActor.m_myBrain.m_legs.IsResting())
        {
            if(m_myActor.m_states.Contains(Type.IDLE))
            {
                m_myUser.SetState(new State_Idle(m_myUser));
            }
        }
    }

    public override void End()
    {

    }
}