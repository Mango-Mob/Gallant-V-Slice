using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.Spawning
{
    public class HeadSpawnner : MonoBehaviour
    {
        public ActorData m_toSpawn;

        protected Timer m_spawnTimer;

        public void Awake()
        {
            m_spawnTimer = new Timer();
            m_spawnTimer.onFinish.AddListener(DirectSpawn);
        }

        public void Update()
        {
            m_spawnTimer.Update();
        }

        public void StartSpawn(float delay = 1.0f)
        {
            m_spawnTimer.Start(delay);
        }

        public void DirectSpawn()
        {
            if (m_toSpawn != null)
            {
                Actor spawn = ActorManager.Instance.GetReservedActor(m_toSpawn.name);
                if (spawn != null && GetComponentInParent<Actor>() != null)
                {
                    spawn.m_lastSpawner = GetComponentInParent<Actor>().m_lastSpawner;
                    spawn.m_lastSpawner?.m_myActors.Add(spawn);
                    spawn.Spawn((uint)Mathf.FloorToInt(GameManager.currentLevel), GetComponentInChildren<Actor_Head>().transform.position, GetComponentInChildren<Actor_Head>().transform.rotation);

                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}

