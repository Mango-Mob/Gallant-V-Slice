using ActorSystem.AI.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_KeepAwayFromTarget : State
{
    public State_KeepAwayFromTarget(StateMachine _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        m_myUser.m_activeStateText = "KEEP DISTANCE";

        m_myActor.SetTargetVelocity(Vector3.zero);
    }

    public override void Update()
    {
        //if (userAsActor.m_animator != null && userAsActor.m_animator.m_hasVelocity)
        //    userAsActor.m_animator.SetVector3("VelocityHorizontal", "",  "VelocityVertical", userAsActor.m_legs.localVelocity.normalized, 0.25f);
        //
        //if (userAsActor.m_target == null)
        //{
        //    m_myUser.SetState(new State_Idle(m_myUser));
        //    return;
        //}
        //    
        //float dist = Vector3.Distance(userAsActor.m_target.transform.position, userAsActor.transform.position);
        //
        //if(dist < userAsActor.m_idealDistance)
        //{
        //    //MOVE
        //    Vector3 direct = userAsActor.transform.position - userAsActor.m_target.transform.position;
        //    NavMeshHit hit;
        //    if(NavMesh.SamplePosition(userAsActor.transform.position + direct.normalized, out hit, 0.5f, ~0))
        //    {
        //        userAsActor.m_legs.SetTargetLocation(hit.position, true);
        //    }
        //    else if (NavMesh.SamplePosition(userAsActor.transform.position - direct.normalized, out hit, 0.5f, ~0))
        //    {
        //        userAsActor.m_legs.SetTargetLocation(hit.position, true);
        //    }
        //}
        //else
        //{
        //    userAsActor.m_legs.SetTargetRotation(Quaternion.LookRotation((userAsActor.m_target.transform.position - userAsActor.transform.position).normalized, Vector3.up));
        //    m_delay -= Time.deltaTime;
        //
        //    //Check if there is a target to move to.
        //    userAsActor.m_legs.SetTargetRotation(Quaternion.LookRotation(userAsActor.m_target.transform.position - userAsActor.transform.position, Vector3.up));
        //    if (m_myActor.m_states.Contains(Type.ATTACK))
        //    {
        //        State_Attack.AttemptTransition(m_myUser);
        //    }
        //    else
        //    {
        //        m_myUser.SetState(new State_Idle(m_myUser));
        //    }
        //}       
    }

    public override void End()
    {

    }
}
