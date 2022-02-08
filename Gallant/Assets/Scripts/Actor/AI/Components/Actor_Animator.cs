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
namespace Actor.AI.Components
{
    public class Actor_Animator : MonoBehaviour
    {
        //Accessables:
        protected Animator m_animator;
        public bool m_hasVelocity { get; private set; }
        public bool m_hasHit { get; private set; }


        // Start is called before the first frame update
        void Awake()
        {
            m_animator = GetComponent<Animator>();

            m_hasVelocity = (HasParameter("VelocityHorizontal") && HasParameter("VelocityVertical") && HasParameter("VelocityHaste"));
            m_hasHit = (HasParameter("Hit") && HasParameter("HitVertical") && HasParameter("HitHorizontal"));
        }

        /*******************
         * SetFloat : Sets a single float value in the animator, with the float provided.
         * @author : Michael Jordan
         * @param : (string) name of the x parameter in the animator.
         * @param : (float) value to store into the animator.
         * @param : (float) time (in seconds) for blending the current values to the new one provided (Default = 0.0f).
         */
        public void SetFloat(string name, float value, float lerpDuration = 0)
        {
            if (lerpDuration > 0)
            {
                if (name != "")
                    StartCoroutine(BlendFloatValue(name, value, lerpDuration));
                return;
            }

            if (name != "")
                m_animator.SetFloat(name, value);
        }

        /*******************
        * SetVector2 : Sets two floats in the animator, with the vector2 provided.
        * @author : Michael Jordan
        * @param : (string) name of the x parameter in the animator.
        * @param : (string) name of the y parameter in the animator.
        * @param : (Vector2) float values to store into the animator.
        * @param : (float) time (in seconds) for blending the current values to the new one provided (Default = 0.0f).
        */
        public void SetVector2(string xName, string yName, Vector2 vector, float lerpDuration = 0)
        {
            if (lerpDuration > 0)
            {
                if (xName != "")
                    StartCoroutine(BlendFloatValue(xName, vector.x, lerpDuration));
                if (yName != "")
                    StartCoroutine(BlendFloatValue(yName, vector.y, lerpDuration));
                return;
            }

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
        * @param : (float) time (in seconds) for blending the current values to the new one provided (Default = 0.0f).
        */
        public void SetVector3(string xName, string yName, string zName, Vector3 vector, float lerpDuration = 0)
        {
            if (lerpDuration > 0)
            {
                if (xName != "")
                    StartCoroutine(BlendFloatValue(xName, vector.x, lerpDuration));
                if (yName != "")
                    StartCoroutine(BlendFloatValue(yName, vector.y, lerpDuration));
                if (zName != "")
                    StartCoroutine(BlendFloatValue(zName, vector.z, lerpDuration));

                return;
            }

            if (xName != "")
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

        /*******************
        * BlendFloatValue : Blends the existing float value to another based on a durtation.
        * @author : Michael Jordan
        * @param : (string) name of the float value stored in the animator.
        * @param : (float) target float value.
        * @param : (float) time (in seconds) for blending the current values to the new one provided.
        */
        private IEnumerator BlendFloatValue(string valueName, float end, float duration)
        {
            float start = m_animator.GetFloat(valueName);

            float value = start;
            DateTime startTime = DateTime.Now;

            while (value != end)
            {
                float lerp = (float)(DateTime.Now - startTime).TotalSeconds / duration;
                value = Mathf.Lerp(start, end, lerp);
                m_animator.SetFloat(valueName, value);
                yield return new WaitForEndOfFrame();
            }

            value = end;
            m_animator.SetFloat(valueName, value);
            yield return null;
        }

        public bool HasParameter(string _name)
        {
            if (m_animator == null)
                return false;

            foreach (var param in m_animator.parameters)
            {
                if (param.name == _name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

