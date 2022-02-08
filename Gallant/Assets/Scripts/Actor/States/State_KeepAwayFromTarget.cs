using Actor.AI;
using Actor.AI.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State_KeepAwayFromTarget : State
{
    public State_KeepAwayFromTarget(StateMachine _user) : base(_user) { }

    private float m_delay = 1.5f; //In seconds
    public override void Start()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        userAsEnemy.m_currentStateDisplay = "KEEP DISTANCE";

        if (userAsEnemy.m_animator != null && userAsEnemy.m_animator.m_hasVelocity)
            userAsEnemy.m_animator.SetVector2("VelocityHorizontal", "VelocityVertical", Vector2.zero, 0.25f);

        userAsEnemy.m_legs?.Halt();
    }

    public override void Update()
    {
        Enemy userAsEnemy = (m_myUser as Enemy);
        if (userAsEnemy.m_animator != null && userAsEnemy.m_animator.m_hasVelocity)
            userAsEnemy.m_animator.SetVector3("VelocityHorizontal", "",  "VelocityVertical", userAsEnemy.m_legs.localVelocity.normalized, 0.25f);

        if (userAsEnemy.m_target == null)
        {
            m_myUser.SetState(new State_Idle(m_myUser));
            return;
        }
            
        float dist = Vector3.Distance(userAsEnemy.m_target.transform.position, userAsEnemy.transform.position);
        
        if(dist < userAsEnemy.m_idealDistance)
        {
            //MOVE
            Vector3 direct = userAsEnemy.transform.position - userAsEnemy.m_target.transform.position;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(userAsEnemy.transform.position + direct.normalized, out hit, 0.5f, ~0))
            {
                userAsEnemy.m_legs.SetTargetLocation(hit.position, true);
            }
            else if (NavMesh.SamplePosition(userAsEnemy.transform.position - direct.normalized, out hit, 0.5f, ~0))
            {
                userAsEnemy.m_legs.SetTargetLocation(hit.position, true);
            }
        }
        else
        {
            userAsEnemy.m_legs.SetTargetRotation(Quaternion.LookRotation((userAsEnemy.m_target.transform.position - userAsEnemy.transform.position).normalized, Vector3.up));
            m_delay -= Time.deltaTime;

            //Check if there is a target to move to.
            userAsEnemy.m_legs.SetTargetRotation(Quaternion.LookRotation(userAsEnemy.m_target.transform.position - userAsEnemy.transform.position, Vector3.up));
            if (userAsEnemy.m_myData.m_states.Contains(Type.ATTACK))
            {
                List<Actor_Attack> currentAttacks = new List<Actor_Attack>(userAsEnemy.m_myAttacks);
                currentAttacks.Sort(new AttackPrioritySort(userAsEnemy));

                foreach (var attack in currentAttacks)
                {
                    if (attack.IsWithinRange(userAsEnemy, LayerMask.NameToLayer("Player")) && attack.IsAvailable())
                    {
                        userAsEnemy.SetState(new State_Attack(m_myUser, attack));
                        return;
                    }
                }
            }
            else
            {
                userAsEnemy.SetState(new State_Idle(m_myUser));
            }
        }       
    }

    public override void End()
    {

    }
}
