using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Actor_Animator : An animator accessor to be used by an Actor.
 * @author : Michael Jordan
 * @file : Actor_Animator.cs
 * @year : 2021
 */
public class Actor_Animator : MonoBehaviour
{
    //Accessables:
    protected Animator m_animator;

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    /*******************
    * SetVector2 : Sets two floats in the animator, with the vector2 provided.
    * @author : Michael Jordan
    * @param : (string) name of the x parameter in the animator.
    * @param : (string) name of the y parameter in the animator.
    * @param : (Vector2) float values to store into the animator.
    */
    public void SetVector2(string xName, string yName, Vector2 vector)
    {
        if (xName != "")
            m_animator.SetFloat(xName, vector.x);
        if (yName != "")
            m_animator.SetFloat(yName, vector.y);
    }

    /*******************
    * SetVector3 : Sets three floats in the animator, with the vector3 provided.
    * @author : Michael Jordan
    * @param : (string) name of the x parameter in the animator.
    * @param : (string) name of the y parameter in the animator.
    * @param : (string) name of the z parameter in the animator.
    * @param : (Vector3) float values to store into the animator.
    */
    public void SetVector3(string xName, string yName, string zName, Vector3 vector)
    {
        if(xName != "")
            m_animator.SetFloat(xName, vector.x);
        if (yName != "")
            m_animator.SetFloat(yName, vector.y);
        if (zName != "")
            m_animator.SetFloat(zName, vector.z);
    }

    /*******************
    * SetVector3 : Sets a trigger value in the animator.
    * @author : Michael Jordan
    * @param : (string) name of the trigger parameter in the animator.
    */
    public void SetTrigger(string triggerName)
    {
        m_animator.SetTrigger(triggerName);
    }

    /*******************
    * SetVector3 : Sets a boolean value in the animator.
    * @author : Michael Jordan
    * @param : (string) name of the boolean parameter in the animator.
    * @param : (bool) status of the boolean parameter.
    */
    public void SetBool(string name, bool status)
    {
        m_animator.SetBool(name, status);
    }
}
