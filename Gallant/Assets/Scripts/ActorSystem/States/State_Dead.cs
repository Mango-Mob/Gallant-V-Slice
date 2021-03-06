
using ActorSystem.AI;
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
        m_myActor.m_myBrain.m_legs.SetEnabled(false);

         if(m_myActor.m_myBrain.m_ragDoll == null)
        {
            m_myActor.m_myBrain.m_animator?.PlayAnimation("Death");
            m_myActor.m_myBrain?.m_animator.SetTrigger("Cancel");
        }
        else
        {
            m_myActor.m_myBrain.m_animator.SetEnabled(false);
            m_myActor.m_myBrain.m_ragDoll.EnableRagdoll();
        }

        foreach (var material in m_myActor.m_myBrain.m_materials)
        {
            material.StartDisolve();
        }
    }

    public override void Update()
    {
        m_timer += Time.deltaTime;
        m_myActor.Kill();
        if (!m_myActor.m_myBrain.m_isDisolving)
        {
            m_myActor.DestroySelf();
        }

        if(m_timer > ActorManager.Instance.m_actorDeathTime)
        {
            m_myActor.DestroySelf();
        }
    }

    public override void End()
    {
        
    }
}
