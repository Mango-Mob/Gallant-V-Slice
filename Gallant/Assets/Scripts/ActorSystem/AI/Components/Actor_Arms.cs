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
            m_myData.Sort(new AttackPrioritySort());
            m_mainComponent = GetComponentInParent<Actor>();
            m_cooldowns = new float[m_myData.Count];
            for (int i = 0; i < m_cooldowns.Length; i++)
            {
                m_cooldowns[i] = 0;
            }
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
                m_myData[m_activeAttack.Value].UpdateActor(m_mainComponent);
            }
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
                return m_myData[m_activeAttack.Value].InvokeAttack(transform, m_attackSource, m_targetMask, id, m_baseDamageMod);
            }
            return false;
        }
        public void End()
        {
            if(m_activeAttack.HasValue)
                m_myData[m_activeAttack.Value].EndActor(m_mainComponent);

            m_activeAttack = null;
        }

        public int GetNextAttack()
        {
            for (int i = 0; i < m_myData.Count; i++)
            {
                if (m_myData[i] == null)
                    continue;

                if(m_cooldowns[i] <= 0 && m_myData[i].HasDetectedCollider(transform, m_targetMask))
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
            if (m_myData[m_activeAttack.Value].postVFXPrefab != null)
            {
                Vector3 hitloc = m_myData[m_activeAttack.Value].GetHitLocation(transform, id);
                RaycastHit hit;
                if(Physics.Raycast(hitloc, Vector3.down, out hit, 15f, 1 << LayerMask.NameToLayer("Environment")))
                {
                    GameObject vfx = Instantiate(m_myData[m_activeAttack.Value].postVFXPrefab, hit.point, Quaternion.identity);
                    vfx.transform.forward = transform.forward;
                }
            }
        }
    }
}
