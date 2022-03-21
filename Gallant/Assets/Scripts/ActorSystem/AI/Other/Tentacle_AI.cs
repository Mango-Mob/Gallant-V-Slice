using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Other
{
    public class Tentacle_AI : MonoBehaviour
    {
        public float m_acceleration = 12f;
        public float m_stoppingDist = 0f;
        public bool isLantern = false;
        public bool emergeOnAwake = true;

        public Vector3 m_idealLocation { get; set; }

        public bool isVisible = false;

        private Vector3 m_velocity;
        
        private Animator m_myAnimator;
        private void Awake()
        {
            m_myAnimator = GetComponent<Animator>();
            m_idealLocation = transform.position;

            if (emergeOnAwake)
                Emerge();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void Emerge()
        {
            m_myAnimator.SetBool("Visible", true);
            m_idealLocation = transform.position;
            m_velocity = Vector3.zero;
        }

        public void Submerge()
        {
            m_myAnimator.SetBool("Visible", false);
        }

        public void TeleportToIdeal()
        {
            transform.position = m_idealLocation;
        }

        public void SetVisible(bool status)
        {
            isVisible = status;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_idealLocation, 0.25f);
        }

        private void FixedUpdate()
        {
            if (!isVisible)
                return;

            Vector3 direction = m_idealLocation - transform.position;
            direction.y = 0;

            if (direction == Vector3.zero)
                return;

            if(direction.magnitude > m_stoppingDist)
            {
                m_velocity += m_acceleration * direction.normalized * Time.fixedDeltaTime;
            }
            else
            {
                float decel = -m_velocity.magnitude / (direction.magnitude / (m_velocity.magnitude * 0.5f));
                m_velocity += decel * direction.normalized * Time.fixedDeltaTime;
            }

            if(direction.magnitude < 0.25f)
            {
                m_velocity = Vector3.zero;
                TeleportToIdeal();
            }

            transform.position += m_velocity * Time.fixedDeltaTime;
        }
    }
}
