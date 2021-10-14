using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Animator : MonoBehaviour
{
    protected Animator m_animator;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVector2(string xName, string yName, Vector2 vector)
    {
        if (xName != "")
            m_animator.SetFloat(xName, vector.x);
        if (yName != "")
            m_animator.SetFloat(yName, vector.y);
    }
    public void SetVector3(string xName, string yName, string zName, Vector3 vector)
    {
        if(xName != "")
            m_animator.SetFloat(xName, vector.x);
        if (yName != "")
            m_animator.SetFloat(yName, vector.y);
        if (zName != "")
            m_animator.SetFloat(zName, vector.z);
    }

    public void SetTrigger(string triggerName)
    {
        m_animator.SetTrigger(triggerName);
    }

    public void SetBool(string name, bool status)
    {
        m_animator.SetBool(name, status);
    }
}
