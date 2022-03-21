using ActorSystem.AI.Bosses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Other
{
    public class Tentacle_AI : Boss_Actor
    {
        public float m_acceleration = 12f;
        public float m_stoppingDist = 0f;
        public bool isLantern = false;
        public bool emergeOnAwake = true;

        public Vector3 m_idealLocation { get; set; }

        public bool isVisible = false;

        private Vector3 m_velocity;
        private Boss_Swamp m_octoBrain;

        protected override void Awake()
        {
            base.Awake();

            m_idealLocation = transform.position;
            m_octoBrain = GetComponentInParent<Boss_Swamp>();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            if (emergeOnAwake)
                Emerge();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public void Emerge()
        {
            if (m_myBrain.IsDead)
                return;

            m_myBrain.m_animator.SetBool("Visible", true);
            m_idealLocation = transform.position;
            m_velocity = Vector3.zero;
        }

        public void Submerge()
        {
            m_myBrain.m_animator.SetBool("Visible", false);
        }

        public void TeleportToIdeal()
        {
            transform.position = m_idealLocation;
        }

        public void SetVisible(bool status)
        {
            isVisible = status;
        }

        public override void DealDamage(float _damage, CombatSystem.DamageType _type, CombatSystem.Faction _from, Vector3? _damageLoc = null)
        {
            if (m_mySpawn != null && m_mySpawn.m_spawnning)
            {
                m_mySpawn.StopSpawning();
            }
            if (!m_myBrain.IsDead)
            {
                m_myBrain.m_material?.ShowHit();
                float before = m_myBrain.m_currHealth;
                if (m_myBrain.HandleDamage(_damage, _type, _damageLoc))
                {
                     if (m_HurtVFXPrefab != null)
                        Instantiate(m_HurtVFXPrefab, m_selfTargetTransform.position, Quaternion.identity);

                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.m_animator.SetFloat("playSpeed", 0.25f);
                    m_myBrain.m_material.StartDisolve(2f);
                    Submerge();
                    return;
                }
                else
                {
                    float after = m_myBrain.m_currHealth;
                    m_octoBrain.DealDamage(before-after, _type, _from, _damageLoc);
                }
            }
        }

        public override void DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if (m_mySpawn != null && m_mySpawn.m_spawnning)
            {
                m_mySpawn.StopSpawning();
            }
            if (!m_myBrain.IsDead)
            {
                float before = m_myBrain.m_currHealth;
                if (m_myBrain.HandleDamage(_damage, _type, transform.position, false, false))
                {
                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.m_animator.SetFloat("playSpeed", 0.25f);
                    m_myBrain.m_material.StartDisolve(2f);
                    Submerge();
                    return;
                }
                else
                {
                    float after = m_myBrain.m_currHealth;
                    m_octoBrain.DealDamageSilent(before - after, _type);
                }
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_idealLocation, 0.25f);
        }

        private void FixedUpdate()
        {
            if (!isVisible)
                return;

            Vector3 direction = m_idealLocation - transform.position;
            direction.y = 0;

            if (direction == Vector3.zero)
                return;

            if(direction.magnitude > m_stoppingDist)
            {
                m_velocity += m_acceleration * direction.normalized * Time.fixedDeltaTime;
            }
            else
            {
                float decel = -m_velocity.magnitude / (direction.magnitude / (m_velocity.magnitude * 0.5f));
                m_velocity += decel * direction.normalized * Time.fixedDeltaTime;
            }

            if(direction.magnitude < 0.25f)
            {
                m_velocity = Vector3.zero;
                TeleportToIdeal();
            }

            transform.position += m_velocity * Time.fixedDeltaTime;
        }
    }
}
