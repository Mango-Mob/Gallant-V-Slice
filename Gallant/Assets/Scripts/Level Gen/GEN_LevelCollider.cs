using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GEN_LevelCollider : MonoBehaviour
{
    [SerializeField] public Vector3 m_origin;
    [SerializeField] public Vector3 m_size = new Vector3(1, 1, 1);
    [SerializeField] public Vector3 m_eulerRotation;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider currentCollider = GetComponent<BoxCollider>();
        if (currentCollider != null && m_size == Vector3.zero)
        {
            m_origin = currentCollider.center;
            m_size = currentCollider.size;
        }
    }

    public List<Collider> IsOverlapping(Transform parent, Quaternion local, LayerMask layer, bool showErrors = false)
    {      
        Vector3 center = parent.TransformPoint(local * m_origin);
        Vector3 halfExtents = (m_size * 0.495f);
        
        halfExtents.x = Mathf.Abs(halfExtents.x);
        halfExtents.y = Mathf.Abs(halfExtents.y);
        halfExtents.z = Mathf.Abs(halfExtents.z);

        List<Collider> others = new List<Collider>(Physics.OverlapBox(center, halfExtents, parent.rotation * local, layer));
        
        for (int i = others.Count - 1; i >= 0; i--)
        {
            if(others[i].gameObject == gameObject)
            {
                others.RemoveAt(i);
            }
            
        }
        if (showErrors && others.Count > 1)
        {
            GEN_ErrorNode.CreateErrorAt(center, parent.rotation * local, halfExtents * 2, others);
        }
        //return false;
        return others;
    }

    private void OnDrawGizmos()
    {
        if (!this.isActiveAndEnabled)
            return;

        Gizmos.color = (IsOverlapping(transform.parent, Quaternion.identity, ~0).Count > 0) ? Color.red : Color.yellow;
        //if (transform.parent.parent == null)
            //return; 
        //Gizmos.color = (IsOverlapping(transform.parent.parent, Quaternion.Euler(0, 180, 0), ~0)) ? Color.red : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Translate(m_origin) * Matrix4x4.Rotate(Quaternion.Euler(m_eulerRotation));
        
        Gizmos.DrawWireCube(Vector3.zero, m_size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
