using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(Renderer))]
    public class Actor_Material : Actor_Component
    {
        public Color m_hitColor;
        public Material m_freezeMaterial;
        public Material m_myMaterial;

        public int m_isDisolving { get; private set; } = 0;

        private Texture m_mainTexture;
        private Renderer m_myMesh;

        private float m_timer = 0.0f;
        private float m_disolveTime = 3.0f;

        private Coroutine m_hit;

        private void Awake()
        {
            m_myMaterial = Instantiate(m_myMaterial);
            m_myMesh = GetComponent<Renderer>();
            SetMaterial(m_myMaterial);
            m_mainTexture = m_myMaterial.GetTexture("TextureAlbedo");
            
            if (m_freezeMaterial != null)
            {
                m_freezeMaterial = Instantiate(m_freezeMaterial);
                m_freezeMaterial.SetTexture("_MainTexture", m_mainTexture);
            }
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
            
            EndFreeze();

            if (m_myMaterial.HasProperty("Fade") || m_hit != null)
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

            EndFreeze();

            if (m_myMaterial.HasProperty("Fade") || m_hit != null)
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
            EndFreeze();
        }

        public void ShowHit()
        {
            if (m_isDisolving != 0)
                return;

            if (m_hit != null)
                StopCoroutine(m_hit);

            m_hit = StartCoroutine(ShowHitRoutine(0.1f));
        }

        private IEnumerator ShowHitRoutine(float time)
        {
            m_myMesh.material.SetColor("_BaseOverrideColor", m_hitColor);

            yield return new WaitForSecondsRealtime(time);

            m_myMesh.material.SetColor("_BaseOverrideColor", new Color(0, 0, 0, 0));
            m_hit = null;
            yield return null;
        }

        public void StartFreeze(float targetFreeze)
        {
            if(m_isDisolving == 0)
            {
                StartCoroutine(SetFreezeStatus(targetFreeze, 1.0f));
            }
        }

        public void EndFreeze()
        {
            if (m_isDisolving == 0)
            {
                StartCoroutine(SetFreezeStatus(0.0f, 1.0f));
            }
        }

        public void SetMaterial(Material _toReplace)
        {
            for (int i = 0; i < m_myMesh.materials.Length; i++)
            {
                m_myMesh.materials[i] = _toReplace;

            }
        }

        private IEnumerator SetFreezeStatus(float target, float time)
        {
            float timer = 0.0f;

            float start = (m_myMesh.material.HasProperty("_IceSlider")) ? m_freezeMaterial.GetFloat("_IceSlider") : 0f;
            SetMaterial(m_freezeMaterial);
            while (timer <= time)
            {
                m_freezeMaterial.SetFloat("_IceSlider", Mathf.Lerp(start, target, timer / time));
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if(target <= 0.0f)
            {
                SetMaterial(m_myMaterial);
            }

            yield return null;
        }
    }
}

