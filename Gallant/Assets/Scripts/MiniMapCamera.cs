using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private GameObject m_objectTracking;
    public float m_lerpVal = 0.2f;
    public float m_height = 20f;

    public void Start()
    {
        m_objectTracking = GameManager.Instance.m_player;
    }
    public void Update()
    {
        Vector3 targetPos = m_objectTracking.transform.position + new Vector3(0, m_height, 0);

        transform.position = Vector3.Lerp(transform.position, targetPos, m_lerpVal);
    }
}
