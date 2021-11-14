using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GEN_ErrorNode : MonoBehaviour
{
    public List<Collider> m_hits;
    public Vector3 m_boxSize;
    public Color m_displayColor = Color.red;
    public void OnDrawGizmos()
    {
        Gizmos.color = m_displayColor;
        Gizmos.DrawSphere(transform.position, 1.0f);
        if(m_hits != null)
        {
            foreach (var hit in m_hits)
            {
                Gizmos.DrawLine(transform.position, hit.bounds.center);
                Gizmos.DrawCube(hit.bounds.center, m_boxSize);                
            }
        }
    }

    public static void CreateErrorAt(Vector3 position, Quaternion rotation, Vector3 boxSize, List<Collider> hits)
    {
        GameObject temp = new GameObject();
        temp.name = "GEN ERROR NODE";
        temp.transform.position = position;
        temp.transform.rotation = rotation;
        temp.transform.position += temp.transform.forward;
        GEN_ErrorNode node = temp.AddComponent<GEN_ErrorNode>();
        node.m_boxSize = boxSize;
        node.m_hits = hits;
    }
}