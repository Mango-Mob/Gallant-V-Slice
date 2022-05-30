using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class SpikeTrap : MonoBehaviour
    {
        [Header("Attack Variables")]
        public float m_postDetectDelay = 0.5f;
        public float m_postAttackDelay = 0.5f;

        [Header("Detect Variables")]
        public bool m_isDetectTrap = false;
        public LayerMask m_detectLayers;

        [Header("Damage Variables")]
        public float m_baseDamage;
        public LayerMask m_damageLayers;
        public BoxCollider m_damageCollider;

        private bool hasDetected = true;
        private float m_delay = 0.0f;
        private Animator m_animator;

        public void Awake()
        {
            m_animator = GetComponentInChildren<Animator>();
        }

        public void Update()
        {
            if (m_delay > 0)
                m_delay -= Time.deltaTime;

            if(m_delay <= 0 && hasDetected)
            {
                m_animator.SetTrigger("Extend");
                hasDetected = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(m_isDetectTrap && m_delay <= 0 && m_detectLayers == (m_detectLayers | (1 << other.gameObject.layer)))
            {
                m_delay = m_postDetectDelay;
            }
        }

        private Collider[] GetDamagedColliders()
        {
            return Physics.OverlapBox(m_damageCollider.transform.position + m_damageCollider.center, m_damageCollider.size / 2f, transform.rotation, m_damageLayers);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetDamagedColliders().Length > 0 ? Color.green : Color.red;

            Gizmos.DrawCube(m_damageCollider.transform.position + m_damageCollider.center, m_damageCollider.size);
        }
    }
}
