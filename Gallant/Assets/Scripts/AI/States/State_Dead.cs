using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class State_Dead : State
{
    public State_Dead(Actor _user) : base(_user) { }

    private float m_timer = 0f;
    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "DEAD";

        m_myActor.m_legs.Halt();

        if (m_myActor.m_animator != null)
        {
            if (m_myActor.m_animator.m_hasVelocity)
                m_myActor.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", Vector3.zero);

            m_myActor.m_animator.SetTrigger("Dead");
        }
        m_myActor.m_material?.StartDisolve();
    }

    public override void Update()
    {
        m_timer += Time.deltaTime;
        m_myActor.m_legs.Halt();
        if (m_myActor.m_material != null && !m_myActor.m_material.m_isDisolving)
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
