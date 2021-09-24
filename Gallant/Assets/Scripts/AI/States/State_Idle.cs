using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : State
{
    public State_Idle(Actor _user) : base(_user) { }

    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "IDLE";
    }

    public override void Update()
    {
        m_myActor.legs.Halt();
        m_myActor.animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero);
    }

    public override void End()
    {

    }
}
