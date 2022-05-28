using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(Renderer))]
    public class Actor_Material : Actor_Component
    {
        public Material m_hitMaterial;
        public int m_isDisolving { get; private set; } = 0;

        private Renderer m_myMesh;
        private Material m_myMaterial;

        private float m_timer = 0.0f;
        private float m_disolveTime = 7.5f;

        private Coroutine m_hit;

        private void Awake()
        {
            m_myMesh = GetComponent<Renderer>();
            m_myMaterial = m_myMesh.material;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isDisolving != 0 && m_myMesh != null)
            {
                m_timer += Time.deltaTime;
                float maxTime = m_disolveTime;
                float disolveVal = (m_isDisolving == 1) ? 1.0f - m_timer / maxTime : m_timer / maxTime;

                if (m_hit == null)
                    m_myMaterial.SetFloat("Fade", disolveVal);

                if (m_timer > maxTime)
                {
                    if (m_hit == null)
                        m_myMaterial.SetFloat("Fade", (m_isDisolving == 1) ? 0.0f : 1.0f);
                    m_isDisolving = 0;
                }
            } 
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
            if(status && m_isDisolving == 0)
                m_timer = 0.0f;

            this.gameObject.layer = (status) ? LayerMask.NameToLayer("Attackable") : LayerMask.NameToLayer("Default");
        }

        public void StartDisolve()
        {
            StartDisolve(ActorManager.Instance.m_actorDeathTime);
        }

        public void StartDisolve(float time)
        {
            m_disolveTime = time;
            m_timer = 0.0f;
            if (m_myMesh == null)
                return;

            if (m_myMesh.material.HasProperty("Fade") || m_hit != null)
            {
                m_isDisolving = 1;
                m_myMaterial.SetFloat("Fade", 1.0f);
            }
            else
            {
                Debug.LogWarning("Disolve called when texture isn't a disolve shader.");
            }
        }

        public void StartResolve(float time)
        {
            m_disolveTime = time;
            m_timer = 0.0f;
            if (m_myMesh.material.HasProperty("Fade") || m_hit != null)
            {
                m_isDisolving = -1;
                m_myMaterial.SetFloat("Fade", 0.0f);
            }
            else
            {
                Debug.LogWarning("Disolve called when texture isn't a disolve shader.");
            }
        }

        public void RefreshColor()
        {
            m_isDisolving = 0;
            m_myMaterial.SetFloat("Fade", 1.0f);
        }

        public void ShowHit()
        {
            if (m_isDisolving != 0)
                return;

            if (m_hit != null)
                StopCoroutine(m_hit);

            m_hit = StartCoroutine(ShowHitRoutine(0.02f));
        }

        private IEnumerator ShowHitRoutine(float time)
        {
            m_myMesh.material = m_hitMaterial;

            yield return new WaitForSecondsRealtime(time);

            m_myMesh.material = m_myMaterial;
            m_hit = null;
            yield return null;
        }
    }
}

