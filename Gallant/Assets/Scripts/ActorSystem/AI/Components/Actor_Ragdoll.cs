using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_Ragdoll : Actor_Component
    {
        public Collider m_mainCollider;

        private Collider m_parentCollider;
        private List<Collider> m_colliders;
        private List<Rigidbody> m_bodies;

        private bool m_enabledRag = false;

        public Vector3 velocity { get { if (m_mainCollider == null) return Vector3.zero; return m_mainCollider.GetComponent<Rigidbody>().velocity; } set { m_mainCollider.GetComponent<Rigidbody>().velocity = value; } }

        private void Awake()
        {
            m_parentCollider = GetComponentInParent<Collider>();
            m_colliders = new List<Collider>(GetComponentsInChildren<Collider>());
            m_bodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
            DisableRagdoll();
        }

        public override void SetEnabled(bool status)
        {
            
        }

        public void DisableRagdoll()
        {
            m_enabledRag = false;
            m_parentCollider.enabled = true;
            foreach (var collider in m_colliders)
            {
                collider.enabled = m_enabledRag;
            }
            foreach (var body in m_bodies)
            {
                body.isKinematic = !m_enabledRag;
            }
        }

        public void EnableRagdoll()
        {
            m_enabledRag = true;
            m_parentCollider.enabled = false;
            foreach (var collider in m_colliders)
            {
                collider.enabled = m_enabledRag;
            }
            foreach (var body in m_bodies)
            {
                body.isKinematic = !m_enabledRag;
            }
        }
    }
}