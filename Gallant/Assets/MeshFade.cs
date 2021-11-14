using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshFade : MonoBehaviour
{
    public Material m_base;
    public Material m_fade;
    public float m_fadeValue;
    public float m_fadeTime;

    public AnimationCurve m_fadeBlend;

    private Renderer m_renderer;
    private DateTime m_beenSet;

    
    // Start is called before the first frame update
    void Awake()
    {
        m_renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_renderer.material == m_fade)
        {
            float value = (float)(DateTime.Now - m_beenSet).TotalSeconds / m_fadeTime;
            Color temp = m_renderer.material.color;
            m_renderer.material.color = new Color(temp.r, temp.g, temp.b, m_fadeBlend.Evaluate(value));

            if((DateTime.Now - m_beenSet).TotalSeconds > m_fadeTime)
            {
                m_renderer.material = m_base;
                m_renderer.material.color = new Color(temp.r, temp.g, temp.b, 1.0f);
            }
        }
            
    }

    public void Fade()
    {
        m_renderer.material = m_fade;
        m_beenSet = DateTime.Now;
    }
}
