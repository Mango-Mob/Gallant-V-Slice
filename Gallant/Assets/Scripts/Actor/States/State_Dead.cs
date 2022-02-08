using Actor.AI;
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
        Enemy userAsEnemy = (m_myUser as Enemy);
        userAsEnemy.m_currentStateDisplay = "DEAD";

        userAsEnemy.m_legs.Halt();

        if (userAsEnemy.m_animator != null)
        {
            if (userAsEnemy.m_animator.m_hasVelocity)
                userAsEnemy.m_animator.SetVector3("VelocityHorizontal", "", "VelocityVertical", Vector3.zero);

            userAsEnemy.m_animator.SetTrigger("Dead");
        }
        userAsEnemy.m_material?.StartDisolve();
    }

    public override void Update()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        m_timer += Time.deltaTime;
        userAsEnemy.m_legs.Halt();
        userAsEnemy.GetComponent<Collider>().enabled = false;
        if (userAsEnemy.m_material != null && !userAsEnemy.m_material.m_isDisolving)
        {
            userAsEnemy.DestroySelf();
        }

        if(m_timer > 5.0f)
        {
            userAsEnemy.DestroySelf();
        }
    }

    public override void End()
    {
        
    }
}
