using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : State
{
    public State_Idle(Actor _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "IDLE";
        m_myActor.legs.Halt();
    }

    public override void Update()
    {
        m_myActor.animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero);
        m_delay -= Time.deltaTime;
        if(m_delay <= 0)
        {
            if (m_myActor.m_myData.m_states.Contains(Type.ROAM))
            {
                m_myActor.SetState(new State_Roam(m_myActor));
            }
        }
    }

    public override void End()
    {

    }
}
