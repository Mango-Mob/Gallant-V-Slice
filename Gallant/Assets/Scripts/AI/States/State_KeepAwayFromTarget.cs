using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_KeepAwayFromTarget : State
{
    public State_KeepAwayFromTarget(Actor _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "KEEP DISTANCE";

        if (m_myActor.m_animator != null && m_myActor.m_animator.m_hasVelocity)
            m_myActor.m_animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero, 0.25f);

        m_myActor.m_legs?.Halt();
    }

    public override void Update()
    {
        if (m_myActor.m_animator != null && m_myActor.m_animator.m_hasVelocity)
            m_myActor.m_animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero, 0.25f);

        if (m_myActor.m_target == null)
        {
            m_myActor.SetState(new State_Idle(m_myActor));
            return;
        }
            
        float dist = Vector3.Distance(m_myActor.m_target.transform.position, m_myActor.transform.position);
        
        if(dist < m_myActor.m_idealDistance)
        {
            //MOVE
            Vector3 direct = m_myActor.transform.position - m_myActor.m_target.transform.position;
            m_myActor.m_legs.SetTargetLocation(m_myActor.transform.position + direct.normalized * (m_myActor.m_idealDistance - dist), true);
        }
        else
        {
            m_myActor.m_legs.SetTargetRotation(Quaternion.LookRotation((m_myActor.m_target.transform.position - m_myActor.transform.position).normalized, Vector3.up));
            m_delay -= Time.deltaTime;

            //Check if there is a target to move to.
            m_myActor.m_legs.SetTargetRotation(Quaternion.LookRotation(m_myActor.m_target.transform.position - m_myActor.transform.position, Vector3.up));
            if (m_myActor.m_myData.m_states.Contains(Type.ATTACK))
            {
                List<Actor_Attack> currentAttacks = new List<Actor_Attack>(m_myActor.m_myAttacks);
                currentAttacks.Sort(new AttackPrioritySort(m_myActor));

                foreach (var attack in currentAttacks)
                {
                    if (attack.IsWithinRange(m_myActor, LayerMask.NameToLayer("Player")) && attack.IsAvailable())
                    {
                        m_myActor.SetState(new State_Attack(m_myActor, attack));
                        return;
                    }
                }
            }
            else
            {
                m_myActor.SetState(new State_Idle(m_myActor));
            }
        }       
    }

    public override void End()
    {

    }
}
