using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public GameObject m_objectTracking;
    public float m_lerpVal = 0.2f;

    public void Update()
    {
        Vector3 targetPos = m_objectTracking.transform.position + new Vector3(0, 20, 0);

        transform.position = Vector3.Lerp(transform.position, targetPos, m_lerpVal);
    }
}
