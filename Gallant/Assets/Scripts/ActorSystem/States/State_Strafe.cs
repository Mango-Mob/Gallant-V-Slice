using UnityEngine;
using UnityEngine.AI;

public class State_Strafe : State
{
    public State_Strafe(StateMachine _user) : base(_user) { }

    private bool isStrafingRight = false;
    private float strafeTime = 2.0f;
    public override void Start()
    {
        m_myUser.m_activeStateText = "STRAFING TARGET";
        isStrafingRight = Random.Range(0, 10000) < 0.5 * 10000; // 50% prob
        m_myActor.m_myBrain.m_legs.OverrideStopDist(m_myActor.m_myData.radius);
        m_myActor.m_myBrain.m_legs.m_agent.speed = m_myActor.m_myBrain.m_legs.m_baseSpeed * 0.85f;
    }

    public override void Update()
    {
        strafeTime -= Time.deltaTime;
        if(strafeTime <= 0)
        {
            isStrafingRight = !isStrafingRight;
            strafeTime = 2.0f;
        }
        Vector3 right = m_myActor.transform.right;

        Vector3 sampleALoc = m_myActor.transform.position + right * (1 + m_myActor.m_myData.radius);
        Vector3 sampleBLoc = m_myActor.transform.position - right * (1 + m_myActor.m_myData.radius);

        Vector3 targetLoc;
        if(isStrafingRight)
        {
            //Get Nav target when strafing right
            if(!Extentions.NavMeshOverlapSphere(sampleALoc, m_myActor.m_myData.radius, 1 << LayerMask.NameToLayer("Attackable"), out targetLoc))
            {
                //No valid location
                isStrafingRight = false;
                targetLoc = m_myActor.transform.position;
            }
        }
        else
        {
            //Get Nav target when strafing left
            if (!Extentions.NavMeshOverlapSphere(sampleBLoc, m_myActor.m_myData.radius, 1 << LayerMask.NameToLayer("Attackable"), out targetLoc))
            {
                //No valid location
                isStrafingRight = true;
                targetLoc = m_myActor.transform.position;
            }
        }

        m_myActor.SetTargetLocation(targetLoc, false);
        m_myActor.SetTargetOrientaion(m_myActor.m_target.transform.position);

        if (m_myActor.m_states.Contains(Type.ATTACK))
        {
            int id = m_myActor.m_myBrain.GetNextAttack();
            if (!m_myActor.m_myBrain.m_animator.IsMutexSet())
            {
                if (id >= 0)
                {
                    m_myUser.SetState(new State_Attack(m_myUser, id));
                    return;
                }
            }
        }
        if (Vector3.Distance(targetLoc, m_myActor.m_target.transform.position) > m_myActor.m_myBrain.m_legs.m_baseStopDist * 1.15f)
        {
            if (m_myActor.m_states.Contains(Type.MOVE_TO_TARGET))
            {
                m_myUser.SetState(new State_MoveToTarget(m_myUser));
            }
        }
        else if (Vector3.Distance(m_myActor.transform.position, m_myActor.m_target.transform.position) < m_myActor.m_myBrain.m_legs.m_baseStopDist * 0.95)
        {
            if (m_myActor.m_states.Contains(Type.FLEE_FROM_TARGET))
            {
                m_myUser.SetState(new State_FleeFromTarget(m_myUser));
            }
        }
    }
    public override void End()
    {
        m_myActor.m_myBrain.m_legs.m_agent.speed = m_myActor.m_myBrain.m_legs.m_baseSpeed;
        m_myActor.m_myBrain.m_legs.RefreshStopDist();
    }
}