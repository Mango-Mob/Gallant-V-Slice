﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class State_Dead : State
{
    public State_Dead(StateMachine _user) : base(_user) { }

    private float m_timer = 0f;
    public override void Start()
    {
        m_myUser.m_activeStateText = "DEAD";

        m_myActor.SetTargetVelocity(Vector3.zero);

        m_myActor.m_myBrain.m_animator?.PlayAnimation("Death");
        m_myActor.m_myBrain.m_material.StartDisolve();
    }

    public override void Update()
    {
        m_timer += Time.deltaTime;
        m_myActor.Kill();
        if (m_myActor.m_myBrain.m_material != null && !m_myActor.m_myBrain.m_material.m_isDisolving)
        {
            m_myActor.DestroySelf();
        }

        if(m_timer > 5.0f)
        {
            m_myActor.DestroySelf();
        }
    }

    public override void End()
    {
        
    }
}