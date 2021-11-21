using GEN.Users;
using GEN.Windows;
using System.Collections.Generic;
using UnityEngine;

namespace GEN.Nodes
{
    /****************
     * ColliderNode : A component used to idenify the gameObject as the exit to the section/cap.
     * @author : Michael Jordan
     * @file : ColliderNode.cs
     * @year : 2021
     */
    public class ColliderNode : MonoBehaviour
    {
        [Header("User Settings")]
        [SerializeField] public Vector3 m_origin;
        [SerializeField] public Vector3 m_size = new Vector3(1, 1, 1);
        [SerializeField] public Vector3 m_eulerRotation;

        //Set up a dual link between this node and the owner
        public PrefabSection m_owner { protected get; set; } = null;

        /*******************
         * IsOverlapping : Get all the colliders that overlap with this ColliderNode.
         * @author : Michael Jordan
         * @param : (Transform) Parent of this node.
         * @param : (Quaternion) Local rotation to preview the overlap with.
         * @param : (LayerMask) Layer mask to filter collision check.
         */
        public List<Collider> IsOverlapping(Transform parent, Quaternion local, LayerMask layer)
        {
            Vector3 center = Vector3.zero;

            //If the owner exists, move the center slightly based on the entry node offset.
            if (m_owner != null)
            {
                center = parent.TransformPoint(local * m_origin + transform.position);
                center -= parent.rotation * local * (m_owner.transform.position - m_owner.m_entry.transform.position);
            }

            //Calculate the extents
            Vector3 halfExtents = (m_size * 0.5f);

            //Make sure they are positive
            halfExtents.x = Mathf.Abs(halfExtents.x);
            halfExtents.y = Mathf.Abs(halfExtents.y);
            halfExtents.z = Mathf.Abs(halfExtents.z);

            //Calculate all the physical overlaps
            List<Collider> others = new List<Collider>(Physics.OverlapBox(center, halfExtents, parent.rotation * local, layer));

            //Filter out colliders
            for (int i = others.Count - 1; i >= 0; i--)
            {
                //Filter if the collider is of the same gameObject OR  if the collider is of the same owner
                if (others[i].gameObject == gameObject || (m_owner != null && m_owner.m_colliders.Contains(others[i])))
                {
                    others.RemoveAt(i);
                    continue; //to the next collider.
                }
            }

            //If errors are enabled in the main window AND there was an error
            if (LevelGenMainWindow.showErrors && others.Count >= 1)
            {
                ErrorNode.Instantiate(center, parent.rotation * local, halfExtents * 2, others);
            }

            return others;
        }

        //Draws when gizmos is enabled
        private void OnDrawGizmos()
        {
            //Don't show if this isn't enabled
            if (!this.isActiveAndEnabled)
                return;

            //Set Colour
            Gizmos.color = Color.yellow;

            //Apply transform
            Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Translate(m_origin) * Matrix4x4.Rotate(Quaternion.Euler(m_eulerRotation));

            //Render Cube
            Gizmos.DrawWireCube(Vector3.zero, m_size);

            //Reset transform
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}