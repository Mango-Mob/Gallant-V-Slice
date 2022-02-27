using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utility;

namespace ActorSystem.AI.Components
{
    /****************
     * Actor_Brain : An central class that coordinates with the other actor components to enact states/actions created by the actor.
     * @author : Michael Jordan
     * @file : Actor.cs
     * @year : 2021
     */
    public class Actor_Brain : Actor_Component
    {
        #region Actor sub-sections
        public Actor_Arms m_arms { get; private set; } //The arms of the actor
        public Actor_Legs m_legs { get; private set; } //The Legs of the actor (the navmesh)
        public Actor_Animator m_animator { get; private set; } //The animator of the actor (the animator)
        public Actor_ProjectileSource m_projSource { get; private set; } //Projectile Creator
        public Actor_Material m_material { get; private set; } //Core texture of the actor (the renderer)
        public Actor_UI m_ui { get; private set; } //UI display for this actor
        public Actor_MiniMapIcon m_icon {get; private set;}
        public Actor_PatrolData m_patrol {get; private set;}

        public MultiAudioAgent m_audioAgent { get; private set; }
        public Outline m_myOutline { get; private set; }
        #endregion

        public bool IsDead { get{ return m_currHealth <= 0 && !m_isInvincible; } }

        [Header("Preview")]
        public float m_currHealth;
        public float m_currPhyResist;
        public float m_currAbilResist;
        public GameObject m_target;

        public float m_idealDistance = 1.5f;

        private bool m_isInvincible;
        private float m_startHealth;
        private float m_basePhyResist;
        private float m_baseAbilResist;

        private FloatRange m_adrenalineGain;
        private Timer m_refreshTimer;
        protected virtual void Awake()
        {
            m_arms = GetComponentInChildren<Actor_Arms>();
            m_legs = GetComponentInChildren<Actor_Legs>();
            m_animator = GetComponentInChildren<Actor_Animator>();
            m_projSource = GetComponentInChildren<Actor_ProjectileSource>();
            m_material = GetComponentInChildren<Actor_Material>();
            m_ui = GetComponentInChildren<Actor_UI>();
            m_audioAgent = GetComponentInChildren<MultiAudioAgent>();
            m_myOutline = GetComponentInChildren<Outline>();
            m_patrol = GetComponentInChildren<Actor_PatrolData>();
            m_icon = GetComponentInChildren<Actor_MiniMapIcon>();
            m_myOutline.enabled = false;
            SetOutlineEnabled(false);
        }

        public void StartSpawnAnimation()
        {
            m_animator.SetEnabled(true);
            m_animator.PlayAnimation("Spawn");
        }

        public void Update()
        {
            //Externals
            UpdateExternals();
            m_refreshTimer?.Update();
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
            foreach (var item in GetComponentsInChildren<Actor_Component>())
            {
                if (item == this)
                    continue;

                item.SetEnabled(status);
            }
        }

        private void UpdateExternals()
        {
            if(m_animator != null && m_animator.m_hasVelocity)
            {
                m_animator?.SetFloat("VelocityHaste", (m_legs != null) ? m_legs.m_speedModifier : 1.0f);
                m_animator?.SetVector3("VelocityHorizontal", "", "VelocityVertical", (m_legs != null) ? m_legs.localVelocity.normalized : Vector3.zero);
            }

            m_ui?.SetBar("Health", (float) m_currHealth / m_startHealth);
        }

        public void LoadData(ActorData _data, uint _level = 0)
        {
            //Brain
            m_startHealth = _data.health + _data.deltaHealth * _level;
            m_basePhyResist = _data.phyResist + _data.deltaPhyResist * _level;
            m_baseAbilResist = _data.abilResist + _data.deltaAbilResist * _level;
            m_adrenalineGain = new FloatRange(_data.adrenalineGainMin + _data.deltaAdrenaline * _level, _data.adrenalineGainMax + _data.deltaAdrenaline * _level);
            m_isInvincible = _data.invincible;
            
            //Arms
            if(m_arms != null)
            {
                m_arms.m_baseDamageMod = _data.m_damageModifier + _data.deltaDamageMod * _level;
            }


            //Legs
            if (m_legs != null)
            {
                m_legs.m_baseSpeed = _data.baseSpeed + _data.deltaSpeed * _level;
            }

            //Animator

            //Tracker

            //projScource

            //Material
            m_material?.RefreshColor();
            
            //UI

            //Colliders
            foreach (var item in GetComponentsInChildren<Collider>())
            {
                item.enabled = true;
            }

            //Start
            if(_data.invincible && m_refreshTimer == null)
            {
                m_refreshTimer = new Timer();
                m_refreshTimer.onFinish.AddListener(Refresh);
            }
            else if(!_data.invincible)
            {
                m_refreshTimer = null;
            }
                
            m_currHealth = m_startHealth;
            m_currPhyResist = m_basePhyResist;
            m_currAbilResist = m_baseAbilResist;
        }

        public void SetOutlineEnabled(bool status)
        {
            if (m_myOutline != null)
                m_myOutline.enabled = status;
        }

        public int GetNextAttack()
        {
            if (m_arms == null)
                return -1;

            return m_arms.GetNextAttack();
        }
        
        public void BeginAttack(int id)
        {
            if (m_arms.m_activeAttack != null)
                return;

            if(m_animator.PlayAnimation(m_arms.m_myData[id].animID))
            {
                m_arms.Begin(id);
            }
        }

        public void InvokeAttack()
        {
            Collider[] hits = m_arms.GetOverlapping();
            foreach (var hit in hits)
            {
                m_arms.Invoke(hit, m_projSource);
            }
        }

        public void EndAttack()
        {
            m_arms.m_activeAttack = null;
        }

        public bool HandleDamage(float damage, CombatSystem.DamageType _type, Vector3? _damageLoc = null)
        {
            switch (_type)
            {
                default:
                case CombatSystem.DamageType.Physical:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, m_currPhyResist)); 
                    break;
                case CombatSystem.DamageType.Ability:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, m_currAbilResist)); 
                    break;
                case CombatSystem.DamageType.True:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, 0)); 
                    break;
            }

            //External
            m_material?.ShowHit();
            m_refreshTimer?.Start(5.0f);
            //Hit animation
            if (_damageLoc.HasValue && m_animator != null && m_animator.m_hasHit)
            {
                m_animator.SetTrigger("Hit");
                m_animator.SetVector3("HitHorizontal", "", "HitVertical", (transform.position.DirectionTo(_damageLoc.Value)).normalized);
            }

            //Internal
            m_currHealth -= damage;

            m_ui.enabled = !IsDead;
            return IsDead;
        }

        public void Refresh()
        {
            m_currHealth = m_startHealth;
        }

        public void DropOrbs(int amount)
        {
            CurrencyDrop.CreateCurrencyDropGroup((uint) amount, transform.position, m_adrenalineGain.GetRandom() / amount);
        }

        public bool PlaySoundEffect(string soundName)
        {
            if (m_audioAgent == null)
                return false;
            return m_audioAgent.Play(soundName, false, UnityEngine.Random.Range(0.75f, 1.25f));
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (m_target != null)
            {
                Gizmos.DrawLine(transform.position, m_target.transform.position);
                Gizmos.DrawSphere(m_target.transform.position, 0.25f);
            }

            GetComponentInChildren<Actor_Arms>()?.DrawGizmos();
            GetComponentInChildren<Actor_Legs>()?.DrawGizmos();
            GetComponentInChildren<Actor_PatrolData>()?.DrawGizmos();
        }
    }
}
