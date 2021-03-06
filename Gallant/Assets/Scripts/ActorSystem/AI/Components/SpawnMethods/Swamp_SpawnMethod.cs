using UnityEngine;

namespace ActorSystem.AI.Components.SpawnMethods
{
    public class Swamp_SpawnMethod : Actor_SpawnMethod
    {
        public GameObject m_spawnVFX;
        public float m_spawnDelayMin;
        public float m_spawnDelayMax;
        public float m_spawnTime = 1.0f;

        private bool m_hasResentlySpawnned;
        private float m_timer;
        private GameObject m_spawn;
        private Actor m_myActor;
        private Vector3 m_lastSpawnLoc;

        public void Awake()
        {
            m_myActor = GetComponent<Actor>();
        }
        public override void StartSpawn(Vector3 spawnLoc, Quaternion rotation)
        {
            m_lastSpawnLoc = spawnLoc;
            m_spawn = GameObject.Instantiate(m_spawnVFX, spawnLoc, Quaternion.identity);
            m_spawn.transform.localScale = Vector3.one * m_myActor.m_myData.radius;

            //Play animation
            m_myActor.m_myBrain.SetEnabled(false);
            m_myActor.m_myBrain.m_animator.SetEnabled(false);
            m_myActor.m_myBrain.m_ragDoll?.DisableRagdoll();
            
            m_myActor.GetComponent<Rigidbody>().isKinematic = true;
            m_timer = Random.Range(m_spawnDelayMin, m_spawnDelayMax);
            m_hasResentlySpawnned = true;

            foreach (var item in m_myActor.m_myBrain.m_materials)
            {
                item.SetEnabled(true);
                item.StartDisolve(m_timer);
            }
        }

        protected override void Update()
        {
            base.Update();
            m_timer -= Time.deltaTime;
            if(m_hasResentlySpawnned && m_timer <= 0 && !m_myActor.m_myBrain.m_animator.enabled)
            {
                transform.position = m_spawn.transform.position;
                if(m_myActor.m_target != null)
                {
                    transform.rotation = Quaternion.LookRotation((m_myActor.m_target.transform.position - transform.position).normalized, Vector3.up);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                }

                m_myActor.m_myBrain.m_animator.SetEnabled(true);
                m_myActor.m_myBrain.m_animator.SetBool("Spawn", true);
                m_myActor.m_myBrain.m_animator.PlayAnimation("Spawn_Loop");
                foreach (var item in m_myActor.m_myBrain.m_materials)
                {
                    item.SetEnabled(true);
                    item.StartResolve(m_spawnTime);
                }
            }
        }   

        public override void StopSpawning()
        {
            m_hasResentlySpawnned = false;
            m_spawnning = false;
            m_myActor.m_myBrain.SetEnabled(true);
            m_myActor.m_onSpawnEvent?.Invoke();
            m_myActor.m_myBrain.m_legs.SetTargetLocation(transform.position);
            m_myActor.m_myBrain.m_legs.SetTargetRotation(transform.rotation);
            Destroy(m_spawn);
        }

        public override void Respawn()
        {
            StartSpawn(m_lastSpawnLoc, Quaternion.identity);
        }
    }
}
