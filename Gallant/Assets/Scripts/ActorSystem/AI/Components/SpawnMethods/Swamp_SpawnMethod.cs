using UnityEngine;

namespace ActorSystem.AI.Components.SpawnMethods
{
    public class Swamp_SpawnMethod : Actor_SpawnMethod
    {
        public float m_spawnDepth = 1.0f;
        public float m_width = 1.0f;
        public float m_height = 2.0f;
        public float m_acceleration;
        public float m_deceleration;
        protected Vector3 m_velocity;
        protected Vector3 m_direction;
        //public float m_duration = 1.5f;
        //private Vector3 m_start;
        //private Vector3 m_end;
        //private Vector3 m_endDirection;
        //
        //private bool m_started = false;
        //private float m_time;
        //
        //private float m_waitTime;
        private bool m_spawnning = false;
        private Vector3 m_endLocation;
        //
        //private bool spawnedTrigger = false;
        public override void StartSpawn(Vector3 start, Vector3 end, Vector3 navMeshPoint)
        {
            m_velocity = Vector3.zero;
            m_direction = (end - start).normalized;
            m_endLocation = end;
            transform.position = start + (start - end).normalized * m_spawnDepth;
            
            Vector3 midPoint = (start + end) / 2.0f;
            Vector3 castForward = (navMeshPoint - start).normalized;
            RaycastHit hit;
            if (Physics.Raycast(midPoint, castForward, out hit, 2.0f))
            {
                transform.forward = -hit.normal;
                transform.position += hit.normal * m_width/2.0f;
            }

            //Play animation
            GetComponent<Actor_Brain>().m_animator.SetEnabled(true);
            GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", true);
            GetComponent<Actor_Brain>().m_animator.PlayAnimation("Spawn_Loop");
            m_spawnning = true;
        }

        //
        protected override void Update()
        {
            base.Update();
            if(m_spawnning && Vector3.Distance(m_endLocation, transform.position) < m_height/2.0f)
            {
                GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", false);

                m_spawnning = false;
            }

        }

        public void StopSpawning()
        {
            m_velocity = Vector3.zero;
            GetComponent<Actor_Brain>().SetEnabled(true);
        }

        protected void FixedUpdate()
        {
            if (m_spawnning)
            {
                m_velocity += m_direction * m_acceleration * Time.fixedDeltaTime;
                transform.position += m_velocity * Time.fixedDeltaTime;
            }
            else if (m_velocity != Vector3.zero)
            {
                m_velocity -= m_direction * m_deceleration * Time.fixedDeltaTime;
                transform.position += m_velocity * Time.fixedDeltaTime;
            }
        }

        //protected override void Update()
        //{
        //    if (!m_started)
        //        return;
        //
        //    base.Update();
        //    m_waitTime -= Time.deltaTime;
        //
        //    float lerp = (m_waitTime / m_startTime);
        //    
        //    if(m_waitTime < 0)
        //    {
        //        if(!spawnedTrigger)
        //        {
        //            GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", true);
        //            GetComponent<Actor_Brain>().m_animator.PlayAnimation("Spawn");
        //        }
        //        if(m_time > 0.8f * m_duration)
        //        {
        //            GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", false);
        //            GetComponent<Actor_Brain>().m_animator.m_setDelay = 0.5f;
        //        }
        //
        //        transform.position = MathParabola.Parabola(m_start, m_end, m_height, m_time/m_duration);
        //        transform.forward = m_endDirection;
        //        m_time += Time.deltaTime;
        //
        //        if (m_time > m_duration)
        //        {
        //            transform.position = m_end;
        //            m_started = false;
        //            EndSpawn();
        //        }
        //    }
        //    else
        //    {
        //        transform.position = new Vector3(transform.position.x, m_start.y - lerp, transform.position.z);
        //    }
        //}
    }
}
