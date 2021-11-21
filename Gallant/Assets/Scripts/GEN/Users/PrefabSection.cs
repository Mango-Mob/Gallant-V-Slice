using GEN.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace GEN.Users
{
    [ExecuteInEditMode]
    public class PrefabSection : MonoBehaviour
    {
        public int depth;

        [SerializeField] public List<ColliderNode> m_levelColliders { get; private set; }
        [SerializeField] public List<Collider> m_colliders { get; private set; }

        [SerializeField] public EntryNode m_entry { get; private set; }
        [SerializeField] public Vector3 m_offset { get; private set; }

        public void Awake()
        {
            m_levelColliders = new List<ColliderNode>(GetComponentsInChildren<ColliderNode>());
            m_colliders = new List<Collider>(GetComponentsInChildren<Collider>());
            m_entry = GetComponentInChildren<EntryNode>();

            if (m_entry != null)
                m_offset = transform.position - m_entry.transform.position;

            foreach (var item in m_levelColliders)
            {
                item.m_owner = this;
            }
        }
    }
}