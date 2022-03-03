using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.Spawning
{
    public class SwampSpawnData : SpawnDataGenerator
    {
        /*******************
         * GenerateSpawnPoints : (MultiThreaded) Creates points at equal distances around the centre, based upon the amount of sections.
         * A valid spawn has sight of the water layer and has a nearby navmesh point.
         * @author : Michael Jordan
         */
        protected override Task GenerateSpawnPoints()
        {
            m_spawnPoints = new List<SpawnData>();
            float step = Mathf.Deg2Rad * 360f / m_sections;
            float curr = 0;

            for (int i = 0; i < m_sections; i++)
            {
                Vector3 direct = Vector3.zero;

                if (m_isCircle)
                {
                    //Rotate the forward vector to achieve the next equal distant point
                    direct = (Quaternion.Euler(0, Mathf.Rad2Deg * curr, 0) * transform.forward);
                }
                else
                {
                    //Project the normal circle point onto the square.
                    //Formula: 
                    // x = radius/MAX(|cos(deg)|, |sin(deg)|) * cos(deg)
                    // y = 0;
                    // z = radius/MAX(|cos(deg)|, |sin(deg)|) * sin(deg)
                    float m = Mathf.Max(Mathf.Abs(Mathf.Cos(curr)), Mathf.Abs(Mathf.Sin(curr)));
                    direct.x = (1.0f / m) * Mathf.Cos(curr);    
                    direct.z = (1.0f / m) * Mathf.Sin(curr);    
                }

                //Get point at the edge of the circle/square.
                Vector3 point = transform.position + direct * m_spawnSize;

                //Validate this end point:
                SpawnData data;
                if (EvaluatePoint(point, out data))
                {
                    m_spawnPoints.Add(data);
                }
                curr += step;
            }
            return Task.CompletedTask;
        }

        /*******************
         * EvaluatePoint : Evaluates the end point to determine if an enemy could spawn and move to the end point.
         * @author : Michael Jordan
         * @param : (Vector3) The end position of the spawn.
         * @param : (out SpawnData) A returned information to be used by an actor.
         * @return : (bool) status of the evaluation.
         */
        private bool EvaluatePoint(Vector3 point, out SpawnData data)
        {
            data = new SpawnData();
            RaycastHit hit;
            //Overlap check at end point (not in a wall)
            if (Physics.OverlapSphere(point, m_overlapRadius).Length == 0)
            {
                //Raycast downards(check if there is water)
                if (Physics.Raycast(point, Vector3.down, out hit, transform.position.y + 0.5f))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                    {
                        //NavMesh check (if a nearby point is accessable)
                        NavMeshHit navHit;
                        if (NavMesh.SamplePosition(point, out navHit, m_navSampleRadius, NavMesh.AllAreas))
                        {
                            data.startPoint = hit.point;
                            data.endPoint = point;
                            data.navPoint = navHit.position;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //MonoBehaviour
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if(!Application.isPlaying)
            {
                float step = Mathf.Deg2Rad * (360f / m_sections);
                float curr = 0;

                for (int i = 0; i < m_sections; i++)
                {
                    Vector3 direct = Vector3.zero;
                    if (m_isCircle)
                    {
                        direct = (Quaternion.Euler(0, Mathf.Rad2Deg * curr, 0) * transform.forward);
                    }
                    else
                    {
                        float m = Mathf.Max(Mathf.Abs(Mathf.Cos(curr)), Mathf.Abs(Mathf.Sin(curr)));
                        direct.x = (1.0f / m) * Mathf.Cos(curr);    // x = radius/MAX(|cos(deg)|, |sin(deg)|) * cos(deg)
                        direct.z = (1.0f / m) * Mathf.Sin(curr);    // z = radius/MAX(|cos(deg)|, |sin(deg)|) * sin(deg)
                    }

                    Vector3 point = transform.position + direct * m_spawnSize;
                    RaycastHit hit;
                    Gizmos.color = Color.red;
                    if (Physics.OverlapSphere(point, m_overlapRadius).Length == 0)
                    {
                        if (Physics.Raycast(point, Vector3.down, out hit, transform.position.y + 0.5f))
                        {
                            Gizmos.color = Color.yellow;
                            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                            {
                                Extentions.GizmosDrawCircle(point, m_navSampleRadius);
                                NavMeshHit navHit;
                                if (NavMesh.SamplePosition(point, out navHit, m_navSampleRadius, NavMesh.AllAreas))
                                {
                                    Gizmos.color = Color.green;
                                    Gizmos.DrawSphere(navHit.position, m_overlapRadius);
                                }
                            }
                            Gizmos.DrawSphere(hit.point, m_overlapRadius);
                            Gizmos.DrawLine(hit.point, point);
                        }
                    }
                    Gizmos.DrawSphere(point, m_overlapRadius);
                    curr += step;
                }
            }
        }
    }
}
