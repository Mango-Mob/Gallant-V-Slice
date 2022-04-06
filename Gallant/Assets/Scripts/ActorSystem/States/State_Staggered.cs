using ActorSystem.AI;
using UnityEngine;
public class State_Staggered : State
{
    public State_Staggered(StateMachine _user) : base(_user) { }

    public override void Start()
    {
        (m_myUser as Actor).SetTargetVelocity(Vector3.zero);
        (m_myUser as Actor).m_myBrain.m_animator.SetTrigger("Stagger");
    }

    public override void Update()
    {
        if((m_myUser as Actor).m_myBrain.m_currStamina == (m_myUser as Actor).m_myBrain.m_startStamina)
        {
            if (m_myActor.m_target != null && m_myActor.m_states.Contains(Type.MOVE_TO_TARGET))
            {
                m_myActor.SetState(new State_MoveToTarget(m_myUser));
                return;
            }
            else
            {
                m_myActor.SetState(new State_Idle(m_myUser));
                return;
            }
        }
        
        (m_myUser as Actor).m_myBrain.RegenStamina(3f);
    }

    public override void End()
    {
        (m_myUser as Actor).m_myBrain.m_animator.SetTrigger("Stagger_Reset");
    }
}