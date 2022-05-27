using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class Actor_Leap : Actor_Legs
    {
        public float accel = 8;
        public float minHeight = 2;
        public float maxJumpDist = 8;

        [Range(0, 1f)]
        public float endPointStart = 0.9f;
        private float speed = 0;

        private Actor_Animator m_animator;
        private Vector3 startPos;
        private Vector3 endPos;
        private float remainDist = 0;
        private float startDist = 0;
        private bool isLeaping = false;
        private float rotMod = 1.0f;
        public LeapState m_currentState = LeapState.Ready;
        public enum LeapState { Ready, Start, Landing, Waiting };
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            m_animator = GetComponentInChildren<Actor_Animator>();
            m_agent.updatePosition = false;
            m_agent.updateRotation = false;
            m_canRotate = false;
            startPos = transform.position;
            endPos = transform.position;
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (!m_agent.isOnNavMesh)
                return;

            m_agent.isStopped = true;
            m_animator.SetInteger("LeapingPart", (int)m_currentState);

            if (isLeaping)
            {
                transform.position = MathParabola.Parabola(startPos, endPos, Mathf.Max(Vector3.Distance(startPos, endPos) / 2, minHeight), 1.0f - remainDist/startDist);
            }

            if(m_currentState == LeapState.Waiting && Quaternion.Angle(transform.rotation, m_targetRotation) < 5f)
            {
                m_currentState = LeapState.Start;
            }
        }

        protected override void FixedUpdate()
        {
            if (isLeaping)
            {
                float mod = (remainDist / startDist < 0.5f) ? 0.75f : 1.35f;
                speed += accel * mod * Time.fixedDeltaTime;
                remainDist = Mathf.Clamp(remainDist - speed * Time.fixedDeltaTime, 0, float.MaxValue);

                if (remainDist <= startDist * (1.0f - endPointStart))
                {
                    transform.position = endPos;
                    m_currentState = LeapState.Landing;
                    return;
                }

                m_rotationDirection = 0;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_rotationSpeed * m_speedModifier * rotMod * Time.fixedDeltaTime);
                m_rotationDirection = (Vector3.Dot(transform.right, m_targetRotation * Vector3.forward) > 0) ? 1.0f : -1.0f;
                base.FixedUpdate();
            }
        }

        /*******************
        * SetTargetLocation : Sets the target destination of the navmesh
        * @author : Michael Jordan
        * @param : (Vector3) position in world space of the target destination.
        * @param : (bool) if the actor should set it's target rotation to look towards the destination (default = false).
        */
        public override void SetTargetLocation(Vector3 target, bool lookAtTarget = false)
        {
            if (!m_agent.enabled || !m_agent.isOnNavMesh || isLeaping)
                return;

            Vector3 direction = target - transform.position;
            direction.y = 0;
            Vector3 destination = transform.position + direction.normalized * Mathf.Min(maxJumpDist * m_speedModifier, direction.magnitude);
            m_targetPosition = destination;

            //if (lookAtTarget)
            {
                SetTargetRotation(Quaternion.LookRotation(direction.normalized, Vector3.up)); 
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, m_agent.radius, m_agent.areaMask))
            {
                endPos = hit.position;
                m_currentState = LeapState.Waiting;
            }
        }

        public void StartLeap()
        {
            isLeaping = true;
            speed = m_baseSpeed;
            startPos = transform.position;
            startDist = MathParabola.ParabolaDistance(startPos, endPos, Mathf.Max(Vector3.Distance(startPos, endPos) / 2, minHeight), 10);
            remainDist = startDist;
            rotMod = 1.0f;
            GetComponent<Collider>().enabled = false;
        }
        
        public void EndLeap()
        {
            GetComponent<Collider>().enabled = true;
            m_currentState = LeapState.Ready;
            m_agent.Warp(transform.position);
            isLeaping = false;
        }

        public void StartRotationLeap()
        {
            endPos = transform.position;
            m_currentState = LeapState.Start;
            rotMod = 3.0f;
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if(isLeaping)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(endPos, 0.25f);
                Vector3 pos1 = startPos;
                for (int i = 1; i < 10; i++)
                {
                    Vector3 pos2 = MathParabola.Parabola(startPos, endPos, Mathf.Max(Vector3.Distance(startPos, endPos) / 2, minHeight), i * 1/10);
                    Gizmos.DrawLine(pos1, pos2);
                    pos1 = pos2;
                }
            }
        }
    }
}
