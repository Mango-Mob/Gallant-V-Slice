using ActorSystem.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_Arms : Actor_Component
    {
        public LayerMask m_targetMask;
        public GameObject m_attackSource;

        [Header("Preview")]
        public float m_baseDamageMod;
        public int? m_activeAttack = null;
        public string m_lastAttackName = "";
        public List<AttackData> m_myData = new List<AttackData>();

        public bool m_canUpdateAttack { get; set; } = true;
        private Actor m_mainComponent;
        private float[] m_cooldowns;
        public float m_brainLag { get; protected set; } = 0f;
        public AnimationCurve m_brainDecay;
        public float m_timeSinceLastHit;
        public float m_maxTimeSinceLastHit = 2.0f;

        public Animator m_indicatorAnim;

        public bool hasCancel { 
            get 
            {
                if (m_activeAttack.HasValue && m_myData[m_activeAttack.Value] != null)
                {
                    return m_myData[m_activeAttack.Value].canBeCanceled;
                }
                return false;
            } 
        }
        private void Awake()
        {
            List<AttackData> dupes = new List<AttackData>();
            foreach (var attack in m_myData)
            {
                if(attack != null)
                    dupes.Add(Instantiate(attack));
            }
            m_myData = dupes;
            m_myData.Sort(new AttackPrioritySort());
            m_mainComponent = GetComponent<Actor>();
            m_cooldowns = new float[m_myData.Count];
            for (int i = 0; i < m_cooldowns.Length; i++)
            {
                m_cooldowns[i] = 0;
            }
            m_timeSinceLastHit = 0;
        }

        public void Update()
        {
            for (int i = 0; i < m_cooldowns.Length; i++)
            {
                if (m_cooldowns[i] > 0)
                    m_cooldowns[i] -= Time.deltaTime;
            }

            if(m_canUpdateAttack && m_activeAttack.HasValue)
            {
                m_myData[m_activeAttack.Value]?.UpdateActor(m_mainComponent);
            }

            if(m_brainLag > 0)
                m_brainLag = Mathf.Clamp(m_brainLag - Time.deltaTime, 0f, float.MaxValue);

            m_timeSinceLastHit = Mathf.Clamp(m_timeSinceLastHit + Time.deltaTime, 0, m_maxTimeSinceLastHit);
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
        }

        public AttackData Begin(int id)
        {
            if(m_myData[id] != null)
            {
                m_activeAttack = id;
                m_cooldowns[id] = m_myData[id].cooldown;
                m_lastAttackName = m_myData[id].animID;
                m_myData[id].BeginActor(m_mainComponent);
                m_canUpdateAttack = true;
                return m_myData[id];
            }
            return null;
        }

        public bool Invoke(uint id = 0)
        {
            if (m_activeAttack != null)
            {
                return m_myData[m_activeAttack.Value].InvokeAttack(transform, ref m_attackSource, m_targetMask, id, m_baseDamageMod);
            }
            return false;
        }
        public void End()
        {
            if(m_activeAttack.HasValue && m_myData[m_activeAttack.Value] != null)
            {
                m_myData[m_activeAttack.Value].EndActor(m_mainComponent);
                SetBrainLag(m_myData[m_activeAttack.Value].brainLag);
            }

            m_activeAttack = null;
        }

        public int GetNextAttack(GameObject target)
        {
            if (m_brainLag > 0)
                return -1;

            for (int i = 0; i < m_myData.Count; i++)
            {
                if (m_myData[i] == null)
                    continue;

                if(m_cooldowns[i] <= 0 && m_myData[i].HasDetectedCollider(transform, m_targetMask) && m_myData[i].CanAttack(transform, target))
                {
                    return i;
                }
            }
            return -1;
        }

        public void DrawGizmos()
        {
            foreach (var attack in m_myData)
            {
                if(attack != null && attack.debugShow)
                    attack.DrawGizmos(transform);
            }
        }

        public void PostInvoke(uint id)
        {
            if(m_activeAttack.HasValue)
            {
                m_myData[m_activeAttack.Value]?.PostInvoke(transform, id);
            }
        }

        public void SetBrainLag(float decay, bool fromPlayer = false)
        {
            m_brainLag = Mathf.Max(m_brainLag, decay * m_brainDecay.Evaluate(m_timeSinceLastHit/m_maxTimeSinceLastHit));

            if (fromPlayer)
                m_timeSinceLastHit = 0;
        }
    }
}
