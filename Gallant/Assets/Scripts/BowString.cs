using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowString : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float m_lerp = 0.0f;
    public float m_lineHeight = 0.39f;
    public Transform m_bindTransform;
    public Transform m_startLine;
    public Transform m_endLine;

    private LineRenderer m_renderer;

    // Start is called before the fir
    // st frame update
    void Start()
    {
        m_renderer = GetComponent<LineRenderer>();
        m_renderer.SetPosition(1, (m_startLine.position + m_endLine.position) / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bindTransform != null)
            m_renderer.SetPosition(1, Vector3.Lerp((m_startLine.position + m_endLine.position) / 2, m_bindTransform.position, m_lerp));
        else
            m_renderer.SetPosition(1, (m_startLine.position + m_endLine.position) / 2);

        m_renderer.SetPosition(0, m_startLine.position);
        m_renderer.SetPosition(2, m_endLine.position);
    }
}
