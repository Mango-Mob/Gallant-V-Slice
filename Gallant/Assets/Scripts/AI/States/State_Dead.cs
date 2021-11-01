using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class State_Dead : State
{
    public State_Dead(Actor _user) : base(_user) { }

    private SkinnedMeshRenderer mesh;
    private float m_timer = 0f;
    private float m_secondTimer = 0f;
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
        
        mesh = m_myActor.GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public override void Update()
    {
        m_secondTimer += Time.deltaTime;
        if (mesh != null && mesh.material.name.Contains("Disolve"))
        {
            m_timer += Time.deltaTime;
            float maxTime = 4.5f;
            float disolveVal = 1.0f - m_timer / maxTime;

            mesh.material.SetFloat("Fade", disolveVal);

            if (m_timer > maxTime)
                m_myActor.DestroySelf();
        }

        if(m_secondTimer > 5.0f)
        {
            m_myActor.DestroySelf();
        }
    }

    public override void End()
    {
        
    }
}
