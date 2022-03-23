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

    public AnimationCurve m_fadeInBlend;
    public AnimationCurve m_fadeOutBlend;

    private Renderer m_renderer;
    private DateTime m_beenSet;

    private bool m_isFadingIn;
    private bool m_isFadingOut;
    
    // Start is called before the first frame update
    void Awake()
    {
        m_renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_renderer.material.name == m_fade.name || m_renderer.material.name == m_fade.name + " (Instance)")
        {
            if(m_isFadingIn || m_isFadingOut)
            {
                float value = Mathf.Clamp((float)(DateTime.Now - m_beenSet).TotalSeconds / m_fadeTime, 0.0f, 1.0f);
                Color temp = m_renderer.material.color;
                if (m_isFadingIn)
                {
                    m_renderer.material.color = new Color(temp.r, temp.g, temp.b, m_fadeInBlend.Evaluate(value));
                    if (value >= 1.0f)
                    {
                        m_renderer.material = m_base;
                        m_isFadingIn = false;
                    }
                }
                else if (m_isFadingOut)
                {
                    m_renderer.material.color = new Color(temp.r, temp.g, temp.b, m_fadeOutBlend.Evaluate(value));
                    if (value >= 1.0f)
                    {
                        m_renderer.material.color = new Color(temp.r, temp.g, temp.b, m_fadeOutBlend.Evaluate(1.0f));
                        m_isFadingOut = false;
                    }
                }
            }
        }
            
    }

    public void FadeOut()
    { 
        m_isFadingOut = true;
        m_beenSet = DateTime.Now;
        m_renderer.material = m_fade;
    }

    public void FadeIn()
    {
        m_isFadingIn = true;
        m_beenSet = DateTime.Now;
    }
}
