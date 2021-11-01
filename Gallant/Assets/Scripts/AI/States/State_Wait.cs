using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Wait : State
{
    public State_Wait(Actor _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        m_myActor.m_currentStateDisplay = "WAIT";

        if (m_myActor.m_animator != null && m_myActor.m_animator.m_hasVelocity)
            m_myActor.m_animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero, 0.25f);

        m_myActor.m_legs?.Halt();
    }

    public override void Update()
    {
        
        m_delay -= Time.deltaTime;

        //Check if there is a target to move to.
        if(m_myActor.m_target != null)
        {
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
        }
    }

    public override void End()
    {

    }
}
