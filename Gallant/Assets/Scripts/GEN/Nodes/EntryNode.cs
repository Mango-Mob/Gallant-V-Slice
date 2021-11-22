using UnityEngine;

namespace GEN.Nodes
{
    /**
     * A component used to idenify the gameObject as the enterance to the section/cap.
     * @author : Michael Jordan
     */
    public class EntryNode : MonoBehaviour
    {
        /**
         * OnDrawGizmos function.
         * Draws when gizmos is enabled.
         */
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
