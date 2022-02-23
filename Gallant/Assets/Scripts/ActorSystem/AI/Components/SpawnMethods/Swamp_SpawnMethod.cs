using UnityEngine;

namespace ActorSystem.AI.Components.SpawnMethods
{
    public class Swamp_SpawnMethod : Actor_SpawnMethod
    {
        public float m_height;
        public float m_duration = 1.5f;
        private Vector3 m_start;
        private Vector3 m_end;
        private Vector3 m_endDirection;

        private bool m_started = false;
        private float m_time;

        private float m_waitTime;
        private float m_startTime;

        private bool spawnedTrigger = false;
        public override void StartSpawn(Vector3 start, Vector3 end, Vector3 finalForward)
        {
            m_started = true;
            m_waitTime = Random.Range(1.5f, 4f);
            m_startTime = m_waitTime;
            m_time = 0.0f;
            m_start = start;
            m_end = end;
            m_endDirection = (finalForward - new Vector3(0, finalForward.y, 0)).normalized;

            GetComponent<Actor_Brain>().m_animator.enabled = true;
            GetComponent<Actor_Brain>().m_animator.PlayAnimation("Swimming");
            transform.position = m_start;
            transform.forward = m_endDirection;
        }

        protected override void Update()
        {
            if (!m_started)
                return;

            base.Update();
            m_waitTime -= Time.deltaTime;

            float lerp = (m_waitTime / m_startTime);
            
            if(m_waitTime < 0)
            {
                if(!spawnedTrigger)
                {
                    GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", true);
                    GetComponent<Actor_Brain>().m_animator.PlayAnimation("Spawn");
                }
                if(m_time > 0.8f * m_duration)
                {
                    GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", false);
                    GetComponent<Actor_Brain>().m_animator.m_setDelay = 0.5f;
                }

                transform.position = MathParabola.Parabola(m_start, m_end, m_height, m_time/m_duration);
                transform.forward = m_endDirection;
                m_time += Time.deltaTime;

                if (m_time > m_duration)
                {
                    transform.position = m_end;
                    m_started = false;
                    EndSpawn();
                }
            }
            else
            {
                transform.position = new Vector3(transform.position.x, m_start.y - lerp, transform.position.z);
            }
        }
    }
}
