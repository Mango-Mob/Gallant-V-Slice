using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GEN_ExitNode : MonoBehaviour
{
    private List<GameObject> m_children = new List<GameObject>();

    private void Awake()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            m_children.Add(transform.GetChild(i).gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Handles.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
        Handles.ConeHandleCap(0, transform.position + transform.forward * 0.5f, Quaternion.LookRotation(transform.forward, Vector3.up), 0.20f, EventType.Repaint);
    }
    public void OnDestroy()
    {
        for (int i = m_children.Count - 1; i >= 0; i--)
        {
            m_children[i].transform.SetParent(null);
            DestroyImmediate(m_children[i]);
        }
    }
}
