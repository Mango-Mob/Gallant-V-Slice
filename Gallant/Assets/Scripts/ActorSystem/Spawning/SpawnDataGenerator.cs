using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.Spawning
{
    public struct SpawnData
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public Vector3 navPoint;
    }

    /****************
     * ActorSystem.Spawning/SpawnDataGenerator : A parent script used to define the spawn node generation in a given level.
     * @author : Michael Jordan
     * @file : SpawnDataGeneration.cs
     * @year : 2022
     */

    public abstract class SpawnDataGenerator : MonoBehaviour
    {
        public List<SpawnData> m_spawnPoints { get; protected set; }

        [SerializeField] protected bool m_isCircle = false;
        [Min(3)]
        [SerializeField] protected uint m_sections = 3;
        [Min(1.0f)]
        [SerializeField] protected float m_spawnSize;
        [Min(1.0f)]
        [SerializeField] protected float m_navSampleRadius = 1.5f;
        [Min(0.05f)]
        [SerializeField] protected float m_overlapRadius = 0.35f;

        /*******************
         * GenerateSpawnPoints : (MultiThreaded) Creates and generates spawn data for a given point on the map.
         * @author : Michael Jordan
         */
        protected abstract Task GenerateSpawnPoints();

        //MonoBehaviour
        public virtual void OnDrawGizmos()
        {
            if (m_isCircle)
                Extentions.GizmosDrawCircle(transform.position, m_spawnSize);
            else
                Extentions.GizmosDrawSquare(transform.position, transform.rotation, new Vector2(m_spawnSize, m_spawnSize));

            if (m_spawnPoints != null)
            {
                foreach (var data in m_spawnPoints)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(data.startPoint, m_overlapRadius);
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(data.endPoint, m_overlapRadius);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(data.startPoint, data.endPoint);
                    Gizmos.DrawSphere(data.navPoint, m_overlapRadius);
                }
            }
        }

        //MonoBehaviour
        protected async void Awake()
        {
            await GenerateSpawnPoints();
        }
    }
}
