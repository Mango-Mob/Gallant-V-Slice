using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.Spawning
{
    public class PortalSpawnData : SpawnDataGenerator
    {
        protected override Task GenerateSpawnPoints()
        {
            m_spawnPoints = new List<SpawnData>();

            for (int i = 0; i < m_sections; i++)
            {
                Vector2 loc;
                if(m_isCircle)
                {
                    loc = Random.insideUnitCircle;
                }
                else
                {
                    loc = new Vector2(Random.Range(-m_spawnSize, m_spawnSize), Random.Range(-m_spawnSize, m_spawnSize));
                }
                SpawnData data = new SpawnData();
                data.endPoint = transform.position + new Vector3(loc.x, 0, loc.y);
                data.startPoint = data.endPoint;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(data.endPoint, out navHit, m_navSampleRadius, NavMesh.AllAreas))
                {
                    data.navPoint = navHit.position;
                }

                m_spawnPoints.Add(data);
            }

            return Task.CompletedTask;
        }

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
        }
    }
}
