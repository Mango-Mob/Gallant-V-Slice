using System.Collections.Generic;
using UnityEngine;

namespace GEN.Nodes
{
    /**
     * A component used to idenify the gameObject as the exit to the section/cap.
     * @author : Michael Jordan
     */
    public class ExitNode : MonoBehaviour
    {
        /** a private variable. 
         * A list of all children objects at start up.
         */
        private List<GameObject> m_children = new List<GameObject>();

        /**
         * Awake function.
         * Called when the component is loaded into the scene (Immediately).
         */
        private void Awake()
        {
            //Collect a list of all children objects, when this component was awake.
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                m_children.Add(transform.GetChild(i).gameObject);
            }
        }

        /**
         * OnDrawGizmos function.
         * Draws when gizmos is enabled.
         */
        private void OnDrawGizmos()
        {
            //Set Colour
            Gizmos.color = Color.red;

            //At base
            Gizmos.DrawSphere(transform.position, 0.25f);

            //At end
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
            Gizmos.DrawSphere(transform.position + transform.forward * 0.5f, 0.05f);
        }

        /**
         * OnDestroy function.
         * Called when this component is destroyed.
         */
        private void OnDestroy()
        {
            //Clear all children under this component, to allow for more level generation. 
            for (int i = m_children.Count - 1; i >= 0; i--)
            {
                //Remove parent from child object
                m_children[i].transform.SetParent(null);

                //Delete child's gameObject
                DestroyImmediate(m_children[i]);
            }
        }
    }
}
