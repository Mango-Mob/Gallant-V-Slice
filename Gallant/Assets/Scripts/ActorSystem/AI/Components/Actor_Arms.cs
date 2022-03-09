using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_Arms : Actor_Component
    {
        public LayerMask m_targetMask;

        [Header("Preview")]
        public float m_baseDamageMod;
        public int? m_activeAttack = null;
        public string m_lastAttackName = "";
        public List<AttackData> m_myData = new List<AttackData>();

        private float[] m_cooldowns;
        public bool hasCancel { 
            get 
            {
                if (m_activeAttack.HasValue)
                {
                    return m_myData[m_activeAttack.Value].canBeCanceled;
                }
                return false;
            } 
        }
        private void Awake()
        {
            m_myData.Sort(new AttackPrioritySort());

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
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
        }

        public AttackData Begin(int id)
        {
            m_activeAttack = id;
            m_cooldowns[id] = m_myData[id].cooldown;
            m_lastAttackName = m_myData[id].animID;
            return m_myData[id];
        }

        public Collider[] GetOverlapping()
        {
            if(m_activeAttack != null)
            {
                return m_myData[m_activeAttack.Value].GetDamagingOverlaping(transform, m_targetMask).ToArray();
            }
            return null;
        }

        public void Invoke(Collider target, Actor_ProjectileSource _source = null)
        {
            if (m_activeAttack != null)
            {
                for (int i = 0; i < m_myData[m_activeAttack.Value].instancesPerAttack; i++)
                {
                    switch (m_myData[m_activeAttack.Value].attackType)
                    {
                        case AttackData.AttackType.Melee:
                            DealDamage(target, m_myData[m_activeAttack.Value].damageType);
                            AttackData.ApplyEffect(target.GetComponent<Player_Controller>(), transform, m_myData[m_activeAttack.Value].effectAfterwards, m_myData[m_activeAttack.Value].effectPower);
                            break;
                        case AttackData.AttackType.Ranged:
                            _source?.CreateProjectile(m_myData[m_activeAttack.Value], target, m_baseDamageMod);
                            break;
                        case AttackData.AttackType.Instant:
                            _source?.CreateProjectileInstantly(m_myData[m_activeAttack.Value], target, m_baseDamageMod);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void DealDamage(Collider _target, CombatSystem.DamageType _type)
        {
            if(_target.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                _target.GetComponent<Player_Controller>()?.DamagePlayer(m_myData[m_activeAttack.Value].baseDamage * (m_baseDamageMod), gameObject);
            }
        }

        public int GetNextAttack()
        {
            for (int i = 0; i < m_myData.Count; i++)
            {
                if(m_cooldowns[i] <= 0 && m_myData[i].GetAttackOverlaping(transform, m_targetMask).Count > 0)
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
                attack.DrawGizmos(transform);
            }
        }
    }
}
