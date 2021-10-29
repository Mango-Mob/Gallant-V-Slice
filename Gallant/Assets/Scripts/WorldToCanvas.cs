﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToCanvas : MonoBehaviour
{
    public Transform m_anchorTransform;
    private Camera m_camera;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = m_camera.WorldToScreenPoint(m_anchorTransform.transform.position);
        (transform as RectTransform).position = pos;
    }
}