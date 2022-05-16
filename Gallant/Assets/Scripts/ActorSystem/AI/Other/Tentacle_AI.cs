using ActorSystem.AI.Bosses;
using ActorSystem.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Other
{
    public class Tentacle_AI : Boss_Actor
    {
        public float m_inkSlamDamage = 40f;
        public float m_rotationDeg = 40f;
        public float m_rotationSpeed = 5f;
        public float m_acceleration = 12f;
        public float m_stoppingDist = 0f;
        public bool isLantern = false;
        public bool isHold = false;
        public bool emergeOnAwake = true;

        public Vector3 m_idealLocation { get; set; }
        public bool isVisible = false;

        public List<AttackData> m_headPhaseAttacks;
        public List<AttackData> m_tentaclePhaseAttacks;
        public List<AttackData> m_oilPhaseAttacks;
        public Hitbox m_inkSlam;

        public bool m_needsToUpdateAttacks = false;

        private bool m_submergStatus = true;
        private Vector3 m_velocity;
        public Boss_Swamp m_octoBrain;
        private bool m_isAttacking = false;
        private bool m_canRotate = true;
        private GameObject m_player;
        private Quaternion m_emergeRotation;

        protected override void Awake()
        {
            base.Awake();

            m_idealLocation = transform.position;
            if(!isHold)
                m_octoBrain = GetComponentInParent<Boss_Swamp>();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            m_player = GameManager.Instance.m_player;
            m_myBrain.m_target = m_player;
            if (emergeOnAwake)
                Emerge();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            if (m_myBrain.IsDead || isHold)
                return;

            if (!m_isAttacking && isVisible && !m_submergStatus)
            {
                int nextAttack = m_myBrain.m_arms.GetNextAttack(m_target);

                if (nextAttack != -1 && m_octoBrain.m_currentlyAttacking.Count < m_octoBrain.m_amountOfAttacks)
                {
                    m_octoBrain.m_currentlyAttacking.Add(this);
                    m_isAttacking = true;
                    m_canRotate = false;
                    m_myBrain.BeginAttack(nextAttack);
                }
            }

            if (m_isAttacking && m_myBrain.m_arms.m_activeAttack == null)
            {
                m_isAttacking = false;
                m_canRotate = true;
                m_octoBrain.m_currentlyAttacking.Remove(this);
            }
            else if(m_myBrain.m_arms.m_activeAttack == null && m_needsToUpdateAttacks)
            {
                UpdateAttacks();
                m_needsToUpdateAttacks = false;
            }
        }

        private void UpdateAttacks()
        {
            m_myBrain.m_arms.SetBrainLag(0.75f);
            for (int i = 0; i < m_myBrain.m_arms.m_myData.Count; i++)
            {
                m_myBrain.m_arms.m_myData[i] = null;
            }
            int start = 0;
            switch (m_octoBrain.m_mode)
            {
                case Boss_Swamp.Phase.HEAD:
                    for (int i = 0; i < m_headPhaseAttacks.Count; i++)
                    {
                        m_myBrain.m_arms.m_myData[i + start] = m_headPhaseAttacks[i];
                    }
                    break;
                case Boss_Swamp.Phase.TENTACLE:
                    start += m_headPhaseAttacks.Count;
                    for (int i = 0; i < m_tentaclePhaseAttacks.Count; i++)
                    {
                        m_myBrain.m_arms.m_myData[i+start] = m_tentaclePhaseAttacks[i];
                    }
                    goto case Boss_Swamp.Phase.INK;
                case Boss_Swamp.Phase.INK:
                    start += m_headPhaseAttacks.Count + m_tentaclePhaseAttacks.Count;
                    for (int i = 0; i < m_oilPhaseAttacks.Count; i++)
                    {
                        m_myBrain.m_arms.m_myData[i + start] = m_oilPhaseAttacks[i];
                    }
                    break;
                default:
                    break;
            }
        }

        public void Emerge()
        {
            if (m_myBrain.IsDead || isHold)
                return;

            m_submergStatus = false;

            m_myBrain.m_arms.SetBrainLag(0.75f);

            m_myBrain.m_animator.SetBool("Visible", !m_submergStatus, Random.Range(0f, 2f));
            m_idealLocation = transform.position;
            m_emergeRotation = transform.rotation;

            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }

            m_velocity = Vector3.zero;
        }

        public void Submerge(bool refresh)
        {
            m_submergStatus = true;
            m_myBrain.m_animator?.SetBool("Visible", !m_submergStatus);

            if (refresh && m_myBrain.IsDead)
            {
                m_myBrain.Refresh();
                m_myBrain.m_animator?.SetFloat("playSpeed", 1.0f);
            }
        }

        public void TeleportToIdeal()
        {
            transform.position = m_idealLocation;
        }

        public void SetVisible(bool status)
        {
            isVisible = status;
        }

        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal, Vector3? _damageLoc = null)
        {
            if (m_mySpawn != null && m_mySpawn.m_spawnning)
            {
                m_mySpawn.StopSpawning();
            }
            if (!m_myBrain.IsDead)
            {
                m_myBrain.ShowHit();
                float before = m_myBrain.m_currHealth;
                float after = 0;
                if (m_myBrain.HandleDamage(_damage, piercingVal, _type, _damageLoc))
                {
                     if (m_HurtVFXPrefab != null)
                        Instantiate(m_HurtVFXPrefab, m_selfTargetTransform.position, Quaternion.identity);

                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.m_animator?.SetFloat("playSpeed", 0.25f);
                    foreach (var material in m_myBrain.m_materials)
                    {
                        material.StartDisolve();
                    }
                    Submerge(false);
                    after = m_myBrain.m_currHealth;
                    m_octoBrain.DealDamage(before - after, _type, piercingVal, _damageLoc);
                    m_octoBrain.m_currentlyAttacking.Remove(this);
                    return true;
                }

                after = m_myBrain.m_currHealth;
                m_octoBrain.DealDamage(before - after, _type, piercingVal, _damageLoc);
            }
            return false;
        }

        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if (m_mySpawn != null && m_mySpawn.m_spawnning)
            {
                m_mySpawn.StopSpawning();
            }
            if (!m_myBrain.IsDead)
            {
                float before = m_myBrain.m_currHealth;
                float after = 0;
                if (m_myBrain.HandleDamage(_damage, 0, _type, transform.position, false, false))
                {
                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.m_animator.SetFloat("playSpeed", 0.25f);
                    foreach (var material in m_myBrain.m_materials)
                    {
                        material.StartDisolve();
                    }
                    Submerge(false);
                    after = m_myBrain.m_currHealth;
                    m_octoBrain.DealDamageSilent(before - after, _type);
                    return true;
                }
                after = m_myBrain.m_currHealth;
                m_octoBrain.DealDamageSilent(before - after, _type);
            }
            return false;
        }

        public override void DealImpactDamage(float amount, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {
            
        }

        public void DamageInSlam()
        {
            Collider[] hits = m_inkSlam.GetOverlappingObjects(transform, m_myBrain.m_arms.m_targetMask);

            foreach (var item in hits)
            {
                if(item.gameObject.GetComponent<Player_Controller>())
                {
                    item.gameObject.GetComponent<Player_Controller>().DamagePlayer(m_inkSlamDamage * m_myBrain.m_arms.m_baseDamageMod, CombatSystem.DamageType.Physical, null, true);
                }else if(item.gameObject.GetComponent<Destructible>())
                {
                    item.gameObject.GetComponent<Destructible>().ExplodeObject(item.transform.position, 10f, 10f, false);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_idealLocation, 0.25f);
            
            Gizmos.DrawLine(transform.position, transform.position + m_emergeRotation * Vector3.forward);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);

            if(m_player != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + (m_player.transform.position - transform.position).normalized);
            }

            m_inkSlam.DrawGizmos(transform);
        }

        public override void Kill()
        {
            base.Kill();
            foreach (var material in m_myBrain.m_materials)
            {
                material.StartDisolve();
            }
            m_myBrain.m_animator.SetFloat("playSpeed", 0.25f);
        }

        private void FixedUpdate()
        {
            if (!isVisible || m_myBrain.IsDead || isHold)
                return;

            if(m_canRotate && m_rotationDeg > 0)
            {
                Vector3 playerPos = new Vector3(m_player.transform.position.x, transform.position.y, m_player.transform.position.z);
                Quaternion look = Quaternion.LookRotation((playerPos - transform.position).normalized, Vector3.up);
                Quaternion rotate = Quaternion.RotateTowards(transform.rotation, look, m_rotationSpeed * Time.fixedDeltaTime);

                if (Mathf.Abs(Quaternion.Angle(m_emergeRotation, rotate)) <= m_rotationDeg)
                {
                    transform.rotation = rotate;
                }
            }

            if(!m_myBrain.m_arms.m_activeAttack.HasValue)
            {
                Vector3 direction = m_idealLocation - transform.position;
                direction.y = 0;

                if (direction == Vector3.zero)
                    return;

                if (direction.magnitude > m_stoppingDist)
                {
                    m_velocity += m_acceleration * direction.normalized * Time.fixedDeltaTime;
                }
                else
                {
                    float decel = -m_velocity.magnitude / (direction.magnitude / (m_velocity.magnitude * 0.5f));
                    m_velocity += decel * direction.normalized * Time.fixedDeltaTime;
                }

                if (direction.magnitude < 0.25f)
                {
                    m_velocity = Vector3.zero;
                    TeleportToIdeal();
                }

                transform.position += m_velocity * Time.fixedDeltaTime;
            }
        }

        public void PlaySlam()
        {
            if(isLantern)
            {
                m_myBrain.m_audioAgent.m_myAgent.Play("TentacleLanternSlam", false, Random.Range(0.90f, 1.1f));
            }
            else
            {
                m_myBrain.m_audioAgent.m_myAgent.Play("TentacleSlam", false, Random.Range(0.90f, 1.1f));
            }
        }

        public void PlaySwipe()
        {
            m_myBrain.m_audioAgent.m_myAgent.Play("TentacleSwipe", false, Random.Range(0.90f, 1.1f));
        }
    }
}
