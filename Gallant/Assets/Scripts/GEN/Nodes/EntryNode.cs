using UnityEngine;

namespace GEN.Nodes
{
    /****************
     * EntryNode : A component used to idenify the gameObject as the enterance to the section/cap.
     * @author : Michael Jordan
     * @file : EntryNode.cs
     * @year : 2021
     */

    public class EntryNode : MonoBehaviour
    {
        //Draws when gizmos is enabled
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
