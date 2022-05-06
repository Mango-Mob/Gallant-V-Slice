﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(Renderer))]
    public class Actor_Material : Actor_Component
    {
        public Material m_hitMaterial;
        public bool m_isDisolving { get; private set; } = false;

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
            if (m_isDisolving && m_myMesh != null)
            {
                m_timer += Time.deltaTime;
                float maxTime = m_disolveTime;
                float disolveVal = 1.0f - m_timer / maxTime;

                if (m_hit == null)
                    m_myMaterial.SetFloat("Fade", disolveVal);

                if (m_timer > maxTime)
                {
                    if (m_hit == null)
                        m_myMaterial.SetFloat("Fade", 0.0f);
                    m_isDisolving = false;
                }
                
            } 
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
            if(status)
                m_timer = 0.0f;
        }

        public void StartDisolve(float time = 7.5f)
        {
            m_disolveTime = time;
            if (m_myMesh.material.HasProperty("Fade") || m_hit != null)
            {
                m_isDisolving = true;
            }
            else
            {
                Debug.LogWarning("Disolve called when texture isn't a disolve shader.");
            }
        }

        public void RefreshColor()
        {
            m_isDisolving = false;
            m_myMaterial.SetFloat("Fade", 1.0f);
        }

        public void ShowHit()
        {
            if (m_isDisolving)
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

