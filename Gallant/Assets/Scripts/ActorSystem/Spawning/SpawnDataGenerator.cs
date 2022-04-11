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
        [SerializeField] protected bool m_isCircle = false;
        [Min(3)]
        [SerializeField] protected uint m_sections = 3;
        [Min(1.0f)]
        [SerializeField] protected float m_spawnSize;
        [Min(1.0f)]
        [SerializeField] protected float m_navSampleRadius = 1.5f;
        [Min(0.05f)]
        [SerializeField] protected float m_overlapRadius = 0.35f;

        public LayerMask spawnOverlapLayer;

        public abstract bool GetASpawnPoint(float actorSize, out Vector3 spawnPos);
        //MonoBehaviour
        public virtual void OnDrawGizmos()
        {
            if (m_isCircle)
                Extentions.GizmosDrawCircle(transform.position, m_spawnSize);
            else
                Extentions.GizmosDrawSquare(transform.position, transform.rotation, new Vector2(m_spawnSize, m_spawnSize) * 2);
        }
    }
}
