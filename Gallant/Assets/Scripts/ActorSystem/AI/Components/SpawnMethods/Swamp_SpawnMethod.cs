using UnityEngine;

namespace ActorSystem.AI.Components.SpawnMethods
{
    public class Swamp_SpawnMethod : Actor_SpawnMethod
    {
        public GameObject m_spawnVFX;
        public float m_spawnDelay;

        private bool m_hasResentlySpawnned;
        private float m_timer;
        private GameObject m_spawn;

        public override void StartSpawn(Vector3 spawnLoc)
        {
            m_spawn = GameObject.Instantiate(m_spawnVFX, spawnLoc, Quaternion.identity);

            ////Play animation
            GetComponent<Actor_Brain>().m_ragDoll?.DisableRagdoll();
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
                transform.rotation = Quaternion.LookRotation((GetComponent<Actor_Brain>().m_target.transform.position - transform.position).normalized, Vector3.up);
                GetComponent<Actor_Brain>().m_animator.SetEnabled(true);
                GetComponent<Actor_Brain>().m_animator.SetBool("Spawn", true);
                GetComponent<Actor_Brain>().m_animator.PlayAnimation("Spawn_Loop");
            }
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
