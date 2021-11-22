using GEN.Nodes;
using System.Collections.Generic;
using UnityEngine;

namespace GEN.Users
{
    /**
     * A component used to idenify the root node of the prefab to be used as a section or a cap.
     * @author : Michael Jordan
     */
    [ExecuteInEditMode]
    public class PrefabSection : MonoBehaviour
    {
        /** a public variable. 
         * How deap into the tree this section is 
         */
        public int depth;

        /** a public variable. 
         * A list of all ColliderNodes owned by this section.
         */
        [SerializeField] 
        public List<ColliderNode> m_levelColliders { get; private set; }

        /** a public variable. 
         * A list of all physical colliders owned by this section.
         */
        [SerializeField] 
        public List<Collider> m_colliders { get; private set; }

        /** a public variable. 
         * A defined entry node of this prefab.
         */
        [SerializeField] 
        public EntryNode m_entry { get; private set; }

        /** a public variable. 
         * A localPosition offset to place the entry node at Vector3.zero.
         */
        [SerializeField] 
        public Vector3 m_offset { get; private set; }

        /**
         * Awake function.
         * Called when the component is loaded into the scene (Immediately).
         */
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