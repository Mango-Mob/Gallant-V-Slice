using System.Collections.Generic;
using UnityEngine;

namespace GEN.Nodes
{
    /**
     * A component used to help debug the random generation.
     * @author : Michael Jordan
     */
    public class ErrorNode : MonoBehaviour
    {
        [Header("User Settings")]

        /** a public variable. 
         * A display colour for the error.
         */
        public Color m_displayColor = Color.red;

        /** a public variable. 
         * List of all collisions that caused this error.
         */
        public List<Collider> m_hits { get; private set; }

        /** a private variable. 
         * Display size to use for rendering..
         */
        private Vector3 m_bounds;

        /**
         * OnDrawGizmos function.
         * Draws when gizmos is enabled.
         */
        public void OnDrawGizmos()
        {
            //Set Colour
            Gizmos.color = m_displayColor;

            //If there is a list of hits
            if (m_hits != null)
            {
                //For each hit
                foreach (var hit in m_hits)
                {
                    
                    if (hit != null)
                    {
                        //Draw a line from this transform to the hit's transform.
                        Gizmos.DrawLine(transform.position, hit.bounds.center);
                    }
                }
            }

            //Apply this gameObject's transform.
            Gizmos.matrix = transform.localToWorldMatrix;

            //Render the level collider's bounds
            Gizmos.DrawCube(Vector3.zero, m_bounds);

            //Reset transform
            Gizmos.matrix = Matrix4x4.identity;
        }

        /**
         * A static function that creates an error node in the game world.
         * @param _position Position of the error.
         * @param _rotation Rotation of the error.
         * @param _boundSize Size of the error.
         * @param _hits A list of all the hit's that caused this error (default = null).
         */
        public static void Instantiate(Vector3 _position, Quaternion _rotation, Vector3 _boundSize, List<Collider> _hits = null)
        {
            //Create a new gameObject
            GameObject temp = new GameObject();
            temp.name = "GEN ERROR NODE";

            //Set the position of the error
            temp.transform.position = _position;
            temp.transform.rotation = _rotation;

            //Create an ErrorNode Component
            ErrorNode node = temp.AddComponent<ErrorNode>();

            //Set the variables for this node
            node.m_bounds = _boundSize;
            node.m_hits = _hits;
        }

        /**
         * CleanAll : Removes all instances of an error node from the game world.
         */
        public static void CleanAll()
        {
            ErrorNode[] node = FindObjectsOfType<ErrorNode>();
            for (int i = 0; i < node.Length; i++)
            {
                DestroyImmediate(node[i].gameObject);
            }
        }
    }
}
