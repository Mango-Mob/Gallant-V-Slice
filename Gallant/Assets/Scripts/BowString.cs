using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowString : MonoBehaviour
{
    public float m_lineHeight = 0.39f;

    private Vector3 m_midPoint;
    private LineRenderer m_renderer;
    // Start is called before the fir
    // st frame update
    void Start()
    {
        m_renderer = GetComponent<LineRenderer>();
        m_midPoint = new Vector3(0, 0, m_lineHeight);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.worldToLocalMatrix;
    }
}
