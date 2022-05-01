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
namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(Animator))]
    public class Actor_Animator : Actor_Component
    {
        //Accessables:
        protected Animator m_animator;
        public bool m_hasVelocity { get; private set; }
        public bool m_hasHit { get; private set; }
        public bool m_hasPivot { get; private set; }

        public float m_setDelay = 0.0f;
        // Start is called before the first frame update
        void Awake()
        {
            m_animator = GetComponent<Animator>();

            m_hasVelocity = (HasParameter("VelocityHorizontal") && HasParameter("VelocityVertical") && HasParameter("VelocityHaste"));
            m_hasHit = (HasParameter("Hit"));
            m_hasPivot = (HasParameter("Pivot"));
        }

        public bool IsCurrentStatePlaying(int layer, string name)
        {
            return m_animator.GetCurrentAnimatorStateInfo(layer).IsName(name);
        }

        public bool PlayAnimation(string animID)
        {
            //Play Anim
            if(m_setDelay <= 0f)
            {
                m_animator.Play(animID);
                return true;
            }
            return false;
        }

        public bool IsMutexSet()
        {
            return m_animator.GetBool("Mutex");
        }

        public void Update()
        {
            if (m_setDelay > 0)
                m_setDelay -= Time.deltaTime;
            
        }

        public override void SetEnabled(bool status)
        {
            if(m_animator != null)
                m_animator.enabled = status;

            this.enabled = status;
        }

        public void SetInteger(string name, int value)
        {
            m_animator.SetInteger(name, value);
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
            if (lerpDuration > 0 && m_animator.GetFloat(name) != value)
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

        public int GetInteger(string name)
        {
            return m_animator.GetInteger(name);
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

        public void ResetTrigger(string triggerName)
        {
            m_animator.ResetTrigger(triggerName);
        }

        /*******************
        * SetVector3 : Sets a boolean value in the animator.
        * @author : Michael Jordan
        * @param : (string) name of the boolean parameter in the animator.
        * @param : (bool) status of the boolean parameter.
        */
        public void SetBool(string name, bool status, float delay = 0)
        {
            if (delay <= 0)
                m_animator.SetBool(name, status);
            else
                StartCoroutine(SetBoolDelayed(name, status, delay));
        }

        /*******************
        * SetBoolDelayed : sets a bool variable after a delay.
        * @author : Michael Jordan
        * @param : (string) name of the float value stored in the animator.
        * @param : (bool) final status.
        * @param : (float) time (in seconds) for blending the current values to the new one provided.
        */
        private IEnumerator SetBoolDelayed(string valueName, bool status, float delayInSeconds)
        {
            yield return new WaitForSecondsRealtime(delayInSeconds);

            m_animator.SetBool(valueName, status);

            yield return null;
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

        /*******************
        * BlendFloatValue : Checks the animator params, for if a specific one exists.
        * @author : Michael Jordan
        * @param : (string) name of the parameter to test for, in the animator.
        * @return : (bool) if the parameter exists.
        */
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

        public void SetPause(bool status)
        {
            m_animator.speed = (status) ? 0.0f: 1.0f;
        }

        public void Shake(float intensity)
        {
            Vector2 sides = UnityEngine.Random.insideUnitCircle * intensity;
            Vector3 shakeVector = new Vector3(sides.x, 0, sides.y);
            transform.localPosition = shakeVector;
        }
    }
}

