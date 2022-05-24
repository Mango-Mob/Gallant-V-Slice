using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.Spawning
{
    public class HeadSpawnner : MonoBehaviour
    {
        public ActorData m_toSpawn;

        public void StartSpawn()
        {
            if(m_toSpawn != null)
            {
                Actor spawn = ActorManager.Instance.GetReservedActor(m_toSpawn.name);
                if(spawn != null && GetComponentInParent<Actor>() != null)
                {
                    spawn.m_lastSpawner = GetComponentInParent<Actor>().m_lastSpawner;
                    spawn.m_lastSpawner?.m_myActors.Add(spawn);
                    spawn.Spawn((uint)Mathf.FloorToInt(GameManager.currentLevel), transform.position, transform.rotation);
                }
            }
        }
    }
}

