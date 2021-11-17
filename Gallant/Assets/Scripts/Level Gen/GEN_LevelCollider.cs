using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GEN_LevelCollider : MonoBehaviour
{
    [SerializeField] public Vector3 m_origin;
    [SerializeField] public Vector3 m_size = new Vector3(1, 1, 1);
    [SerializeField] public Vector3 m_eulerRotation;

    private GEN_PrefabSection m_owner;
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

    public void SetOwner(GEN_PrefabSection _owner)
    {
        m_owner = _owner;
    }

    public List<Collider> IsOverlapping(Transform parent, Quaternion local, LayerMask layer, bool showErrors = false)
    {
        Vector3 center = Vector3.zero;
        if (m_owner != null)
        {
            center = parent.TransformPoint(local * m_origin + transform.position);
            center -= parent.rotation * local * (m_owner.transform.position - m_owner.m_entry.transform.position);
        }
            
        Vector3 halfExtents = (m_size * 0.5f);
        
        halfExtents.x = Mathf.Abs(halfExtents.x);
        halfExtents.y = Mathf.Abs(halfExtents.y);
        halfExtents.z = Mathf.Abs(halfExtents.z);

        List<Collider> others = new List<Collider>(Physics.OverlapBox(center, halfExtents, parent.rotation * local, layer));
        
        for (int i = others.Count - 1; i >= 0; i--)
        {
            if(others[i].gameObject == gameObject)
            {
                others.RemoveAt(i);
                continue;
            }
            if(m_owner != null && m_owner.m_colliders.Contains(others[i]))
            {
                others.RemoveAt(i);
                continue;
            }
        }
        if (showErrors && others.Count >= 1)
        {
            GEN_ErrorNode.CreateErrorAt(center, parent.rotation * local, halfExtents * 2, others);            
        }

        return others;
    }

    private void OnDrawGizmos()
    {
        if (!this.isActiveAndEnabled)
            return;

        Gizmos.color = Color.yellow;
        //if (transform.parent.parent == null)
            //return; 
        //Gizmos.color = (IsOverlapping(transform.parent.parent, Quaternion.Euler(0, 180, 0), ~0)) ? Color.red : Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.Translate(m_origin) * Matrix4x4.Rotate(Quaternion.Euler(m_eulerRotation));
        
        Gizmos.DrawWireCube(Vector3.zero, m_size);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
