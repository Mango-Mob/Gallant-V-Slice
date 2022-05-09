using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.Spawning
{
    public class PortalSpawnData : SpawnDataGenerator
    {
        public override bool GetASpawnPoint(float actorSize, out Vector3 spawnPos)
        {
            Collider[] overlapCheck = null;
            int safetyCheck = 5;
            NavMeshHit hit;

            do
            {
                //iterate safety
                safetyCheck--;

                //Get Random point within the known bounds, and transform from local to world
                Vector2 randInBounds = (m_isCircle) ? Random.insideUnitCircle * m_spawnSize : new Vector2(Random.Range(-m_spawnSize, m_spawnSize), Random.Range(-m_spawnSize, m_spawnSize));
                Vector3 randPos = transform.TransformPoint(new Vector3(randInBounds.x, 0, randInBounds.y));

                //Project point to navmesh
                if (!NavMesh.SamplePosition(randPos, out hit, m_navSampleRadius, NavMesh.AllAreas))
                    continue; //if failed, do another loop

                //Conduct an overlap check of the area for any other agents/players
                overlapCheck = Physics.OverlapSphere(hit.position, actorSize, spawnOverlapLayer);

                //If WITHIN the safey bounds and another object is WITHIN the overlap, Redo
            } while (safetyCheck > 5 && overlapCheck.Length > 0);

            //isValid point
            if(overlapCheck != null && overlapCheck.Length == 0 && safetyCheck != 0)
            {
                spawnPos = hit.position;
                return true;
            }

            spawnPos = Vector3.zero;
            return false;
        }
    }
}
