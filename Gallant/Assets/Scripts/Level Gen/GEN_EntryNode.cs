using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GEN_EntryNode : MonoBehaviour
{
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
        Gizmos.color = Color.green;
        //Handles.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.25f);
        //Gizmos.DrawRay(transform.position, transform.forward * 0.5f);
        //Handles.ConeHandleCap(0, transform.position + transform.forward * 0.5f, Quaternion.LookRotation(transform.forward, Vector3.up), 0.20f, EventType.Repaint);
    }
}
