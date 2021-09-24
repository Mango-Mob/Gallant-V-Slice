using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Animator : MonoBehaviour
{
    protected Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVector2(string xName, string yName, Vector2 vector)
    {
        m_animator.SetFloat(xName, vector.x);
        m_animator.SetFloat(yName, vector.y);
    }

    public void SetBool(string name, bool status)
    {
        m_animator.SetBool(name, status);
    }
}
