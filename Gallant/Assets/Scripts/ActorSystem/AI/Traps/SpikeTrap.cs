using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class SpikeTrap : MonoBehaviour
    {
        [Header("Attack Variables")]
        [Tooltip("In Seconds")]
        public float m_defaultSpeed = 1.0f;
        public float m_postDetectDelay = 0.5f;
        public float m_postAttackDelay = 0.5f;

        [Header("Detect Variables")]
        public bool m_isDetectTrap = false;
        public LayerMask m_detectLayers;

        [Header("Damage Variables")]
        public float m_baseDamage;
        public LayerMask m_damageLayers;
        public BoxCollider m_damageCollider;

        public bool isFinished { get { return m_animator.GetBool("Ready"); } }
        private bool hasDetected = false;
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
                ExtendSpikes(m_defaultSpeed);
                hasDetected = false;
            }
        }

        public void ExtendSpikes(float timer = 0.0f)
        {
            if(timer <= 0.0f)
            {
                m_animator.SetFloat("InvertTimeScale", -1.0f);  
            }
            else
            {
                m_animator.SetFloat("InvertTimeScale", 1.0f/timer);
            }
            m_animator.SetTrigger("Extend");
        }

        public void DealDamage()
        {
            GetComponentInChildren<Spike>().m_damage = m_baseDamage;
            m_delay = m_postAttackDelay;
        }

        private void OnTriggerStay(Collider other)
        {
            if(m_isDetectTrap && m_delay <= 0 && m_detectLayers == (m_detectLayers | (1 << other.gameObject.layer)))
            {
                m_delay = m_postDetectDelay;
                hasDetected = true;
            }
        }
    }
}
