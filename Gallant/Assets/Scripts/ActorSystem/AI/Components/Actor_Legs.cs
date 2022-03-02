using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/****************
 * Actor_Legs : An navmesh accessor to be used by an Actor.
 * @author : Michael Jordan
 * @file : Actor_Legs.cs
 * @year : 2021
 */

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class Actor_Legs : Actor_Component
    {
        [HideInInspector]
        public float m_baseSpeed;

        [Range(0.0f, 2.0f)]
        public float m_speedModifier = 1.0f;
        public bool m_isKnocked = false;
        private bool m_isSeekingMesh = false;
        //External Accessors
        public Vector3 velocity { get { return (m_isKnocked) ? Vector3.zero : m_agent.velocity; } }
        public Vector3 localVelocity { get { return (m_isKnocked) ? Vector3.zero : Quaternion.AngleAxis(transform.rotation.eulerAngles.y, -Vector3.up) * m_agent.velocity; } }

        //Statistics:
        public float m_angleSpeed = 45f;

        //Accessables:
        public NavMeshAgent m_agent { get; protected set; }
        protected Rigidbody m_body;

        //Target Orientation
        protected Vector3 m_targetPosition;
        protected Quaternion m_targetRotation;

        private float m_delayTimer = 0f;
        private Vector3 m_lastPosition;
        // Start is called before the first frame update
        void Awake()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_body = GetComponent<Rigidbody>();
            m_body.isKinematic = true;
            m_agent.updateRotation = false;
            m_lastPosition = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            m_delayTimer = Mathf.Clamp(m_delayTimer - Time.deltaTime, 0f, 1f);

            if(m_delayTimer > 0)
                return;

            if (m_agent.enabled && m_agent.isOnNavMesh)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_angleSpeed * m_speedModifier * Time.deltaTime);
                m_agent.destination = m_targetPosition;
                m_agent.speed = m_baseSpeed * m_speedModifier;
            }
            if(!m_agent.isOnNavMesh && !m_isKnocked)
            {
                NavMeshHit hit;
                if (NavMesh.FindClosestEdge(transform.position, out hit, m_agent.areaMask))
                {
                    if(IsGrounded())
                        m_body.velocity += transform.position.DirectionTo(hit.position).normalized * m_agent.acceleration;
                }
            }
        }

        public void FixedUpdate()
        {
            if(!m_agent.updatePosition)
                m_agent.Warp(transform.position);

            if(m_isKnocked)
            {
                m_body.velocity -= (m_body.velocity.normalized * m_agent.acceleration * Time.fixedDeltaTime);
            }
            
            Debug.Log(m_body.velocity.magnitude);
            m_agent.updatePosition = !m_isKnocked && !m_isSeekingMesh;
            m_body.isKinematic = !m_isKnocked && !m_isSeekingMesh;
            
            if (m_isKnocked && (m_body.velocity.magnitude < 1.0f || Vector3.Distance(m_lastPosition, transform.position) < 0.005f))
            {
                m_isKnocked = false;
                m_delayTimer = 0.25f;
            }
            m_lastPosition = transform.position;
        }

        public void SetTargetVelocity(Vector3 moveVector)
        {
            if (moveVector.magnitude == 0)
            {
                Halt();
                return;
            }

            if(m_agent.enabled && m_agent.isOnNavMesh)
            {
                m_agent.isStopped = false;
                m_agent.velocity = moveVector;
            }
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
            if(status)
            {
                m_agent.enabled = true;

                NavMeshHit hit;
                if (!m_agent.updatePosition && NavMesh.SamplePosition(transform.position, out hit, m_agent.radius, NavMesh.GetAreaFromName("Walkable")))
                {
                    m_agent.Warp(hit.position);
                    m_agent.updatePosition = true;
                    m_body.isKinematic = true;
                }
                else if (!m_agent.updatePosition)
                {
                    m_body.isKinematic = false;
                }
            }
            else
            {
                m_agent.enabled = false;
                m_body.isKinematic = true;
            }
        }

        /*********************
         * IsResting : Is the actor's legs currently resting?
         * @author : Michael Jordan
         * @return : (bool) true if the actor's legs are either not moving or if the velocity value is small enough.
         */
        public bool IsResting()
        {
            return (m_agent.velocity.magnitude < 0.15f && Vector3.Distance(m_targetPosition, transform.position) < 0.5f) || m_agent.isStopped;
        }

        /*********************
         * Halt : Stops the actor's legs.
         * @author : Michael Jordan
         */
        public void Halt()
        {
            if (m_agent.enabled && m_agent.isOnNavMesh)
                m_agent.isStopped = true;
        }

        /*******************
        * SetTargetLocation : Sets the target destination of the navmesh
        * @author : Michael Jordan
        * @param : (Vector3) position in world space of the target destination.
        * @param : (bool) if the actor should set it's target rotation to look towards the destination (default = false).
        */
        public void SetTargetLocation(Vector3 target, bool lookAtTarget = false)
        {
            if (!m_agent.enabled || !m_agent.isOnNavMesh)
                return;

            m_agent.isStopped = false;
            m_targetPosition = target;

            Vector3 direction = m_targetPosition - transform.position;
            direction.y = 0;

            if (lookAtTarget)
                SetTargetRotation(Quaternion.LookRotation(direction.normalized, Vector3.up));
        }

        /*******************
        * SetTargetRotation : Sets the target rotation of the actor.
        * @author : Michael Jordan
        * @param : (Quaternion) target rotation of the actor.
        */
        public void SetTargetRotation(Quaternion rotation)
        {
            m_targetRotation = rotation;
        }

        /*******************
        * SetTargetRotation : Gets the total angle of rotation to looktowards a GameObject.
        * @author : Michael Jordan
        * @param : (GameObject) target to calculate towards.
        */
        public float GetAngleTowards(GameObject _target)
        {
            if (_target == null)
                return 0;

            Quaternion lookAt = Quaternion.LookRotation((_target.transform.position - transform.position).normalized, Vector3.up);
            return Quaternion.Angle(transform.rotation, lookAt);
        }


        public void DrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if (m_agent != null)
            {
                for (int i = 0; i < m_agent.path.corners.Length; i++)
                {
                    if (i == 0)
                        Gizmos.DrawLine(transform.position, m_agent.path.corners[i]);
                    else
                        Gizmos.DrawLine(m_agent.path.corners[i - 1], m_agent.path.corners[i]);
                }
                Gizmos.color = (m_agent.updatePosition) ? Color.green : Color.red;
                Gizmos.DrawSphere(m_agent.transform.position, 0.25f);
                Gizmos.DrawLine(transform.position, transform.position + m_agent.velocity);
            }

            if(m_body != null)
            {
                Gizmos.color = new Color(255, 215, 0);
                Gizmos.DrawLine(transform.position, transform.position + m_body.velocity);
            }
        }

        public bool IsGrounded()
        {
            return Physics.OverlapSphere(transform.position, m_agent.radius, 1 << LayerMask.NameToLayer("Default")).Length > 0;
        }

        public void KnockBack(Vector3 force)
        {
            SetTargetVelocity(force);
            if (!m_isKnocked)
            {
                m_isKnocked = true;
                m_body.isKinematic = false;
                return;
            }
            m_body.velocity = force;
        }
    }
}

