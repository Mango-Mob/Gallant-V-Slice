using UnityEngine;

namespace ActorSystem.AI.Components.SpawnMethods
{
    public class Swamp_SpawnMethod : Actor_SpawnMethod
    {
        public GameObject m_spawnVFX;
        public float m_spawnDelay;

        //public float m_postWaitTime = 0.5f;
        //public float m_spawnDepth = 1.0f;
        //public float m_width = 1.0f;
        //public float m_height = 2.0f;
        //public float m_acceleration;
        //public float m_deceleration;
        //
        //protected Vector3 m_velocity;
        //protected Vector3 m_direction;
        //protected Vector3 m_forwardCast;
        //private Vector3 m_endLocation;
        private bool m_hasResentlySpawnned;
        private float m_timer;
        private GameObject m_spawn;

        public override void StartSpawn(Vector3 start, Vector3 end, Vector3 navMeshPoint)
        {
            m_spawn = GameObject.Instantiate(m_spawnVFX, navMeshPoint, Quaternion.identity);

            //m_velocity = Vector3.zero;
            //m_direction = (end - start).normalized;
            //m_endLocation = end;
            //transform.position = start + (start - end).normalized * m_spawnDepth;
            //
            //Vector3 midPoint = (start + end) / 2.0f;
            //m_forwardCast = (navMeshPoint - end).normalized;
            //RaycastHit hit;
            //if (Physics.Raycast(midPoint, m_forwardCast, out hit, 2.0f))
            //{
            //    transform.forward = -hit.normal;
            //    transform.position += hit.normal * m_width/2.0f;
            //}
            //
            ////Play animation
            
            m_timer = m_spawnDelay;
            m_hasResentlySpawnned = true;
            //m_spawnning = true;
        }

        //
        protected override void Update()
        {
            base.Update();
            m_timer -= Time.deltaTime;
            if(m_hasResentlySpawnned && m_timer <= 0 && !GetComponent<Actor_Brain>().m_animator.enabled)
            {
                transform.position = m_spawn.transform.position;
                GetComponent<Actor_Brain>().m_animator.SetEnabled(true);
                GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", true);
                GetComponent<Actor_Brain>().m_animator.PlayAnimation("Spawn_Loop");
            }
            //if(m_spawnning && m_endLocation.y - transform.position.y < m_height/2.0f)
            //{
            //    GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", false);
            //    m_spawnning = false;
            //    m_hasResentlySpawnned = true;
            //}
            //if(m_hasResentlySpawnned && !m_spawnning)
            //{
            //    m_hasResentlySpawnned = !GetComponent<Actor_Brain>().m_legs.m_agent.isOnNavMesh;
            //}
        }   

        public override void StopSpawning()
        {
            m_hasResentlySpawnned = false;
            m_spawnning = false;
            GetComponent<Actor_Brain>().SetEnabled(true);
            Destroy(m_spawn);
        }

        protected void FixedUpdate()
        {
            //if (m_spawnning)
            //{
            //    m_velocity += m_direction * m_acceleration * Time.fixedDeltaTime;
            //    transform.position += m_velocity * Time.fixedDeltaTime;
            //}
            //else if (m_velocity != Vector3.zero)
            //{
            //    m_velocity -= m_direction * m_deceleration * Time.fixedDeltaTime;
            //    transform.position += m_velocity * Time.fixedDeltaTime;
            //}
            //if(m_hasResentlySpawnned)
            //{
            //    transform.position += m_forwardCast * Time.fixedDeltaTime;
            //}
        }
    }
}
