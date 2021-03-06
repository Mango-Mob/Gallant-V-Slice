using ActorSystem.Data;
using EPOOutline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
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
        public Actor_Head m_head { get; private set; } //Controls sight/hearing based behaviours and look direction
        public Actor_Arms m_arms { get; private set; } //The arms of the actor
        public Actor_Legs m_legs { get; private set; } //The Legs of the actor (the navmesh)
        public Actor_Animator m_animator { get; private set; } //The animator of the actor (the animator)
        public Actor_Material[] m_materials { get; private set; } //Core texture of the actor (the renderer)
        public Actor_UI m_ui { get; private set; } //UI display for this actor
        public Actor_MiniMapIcon m_icon {get; private set;}
        public Actor_PatrolData m_patrol {get; private set;}
        public Actor_AudioAgent m_audioAgent { get; private set; }
        public Actor_Ragdoll m_ragDoll { get; private set; }
        public Actor_Indicator[] m_indicators { get; private set; }
        public Outlinable m_myOutline { get; private set; }
        #endregion
        public bool IsDead { get{ return m_currHealth <= 0 && !m_isInvincible; } }
        public bool IsStunned { get; set; } = false;
        public bool m_canBeTarget = true;
        public bool m_forceShowUI = false;
        public bool m_lookAtHit { get; set; } = true;

        [Header("Preview")]
        public float m_currHealth;
        public float m_currStamina;
        public float m_currPhyResist;
        public float m_currAbilResist;
        public GameObject m_target;

        private bool m_isInvincible;
        public float m_startHealth { get; private set; }
        public float m_startStamina { get; private set; }
        public float m_basePhyResist { get; private set; }
        public float m_baseAbilResist   { get; private set; }

        public bool m_canStagger { get; set; }

        private float m_staminaRegen; 
        private FloatRange m_adrenalineGain;
        private Timer m_refreshTimer;

        private UI_Bar m_staminaBar;

        public bool m_isDisolving { 
            get {
                foreach (var material in m_materials)
                {
                    if (material.m_isDisolving != 0)
                        return true;
                }
                return false;
            } 
        }

        protected virtual void Awake()
        {
            m_head = GetComponentInChildren<Actor_Head>();
            m_arms = GetComponentInChildren<Actor_Arms>();
            m_legs = GetComponentInChildren<Actor_Legs>();
            m_animator = GetComponentInChildren<Actor_Animator>();
            m_materials = GetComponentsInChildren<Actor_Material>();
            m_ui = GetComponentInChildren<Actor_UI>();
            m_audioAgent = GetComponent<Actor_AudioAgent>();
            m_patrol = GetComponentInChildren<Actor_PatrolData>();
            m_ragDoll = GetComponentInChildren<Actor_Ragdoll>();
            m_icon = GetComponentInChildren<Actor_MiniMapIcon>();
            m_myOutline = GetComponentInChildren<Outlinable>();
            m_indicators = GetComponentsInChildren<Actor_Indicator>();
            SetOutlineEnabled(false);
        }
        public void Start()
        {
            m_staminaBar = m_ui?.GetElement<UI_Bar>("Stamina");
        }
        public void StartSpawnAnimation()
        {
            m_animator.SetEnabled(true);
            m_animator.PlayAnimation("Spawn");
        }

        public void Update()
        {
            m_ui?.SetBar("Health", (float)m_currHealth / m_startHealth);
            if (IsStunned || IsDead)
            {
                foreach (var item in m_indicators)
                {
                    item.m_speed = 0.0f;
                }
                m_legs?.Halt();
                return;
            }

            //Externals
            UpdateExternals();
            m_refreshTimer?.Update();
        }

        public override void SetEnabled(bool status)
        {
            this.enabled = status;
            foreach (var item in GetComponentsInChildren<Actor_Component>())
            {
                if (item == this && item != m_ragDoll)
                    continue;

                item.SetEnabled(status);
            }
        }

        private void UpdateExternals()
        {
            if(m_animator != null && m_animator.m_hasVelocity)
            {
                m_animator.m_speed = (m_legs != null) ? m_legs.m_speedModifier : 1.0f;
                m_animator.SetFloat("VelocityHaste", 1.0f);
                m_animator?.SetVector3("VelocityHorizontal", "", "VelocityVertical", (m_legs != null) ? m_legs.scaledVelocity : Vector3.zero);
            }
            if (m_animator != null && m_animator.HasParameter("RotationVelocity"))
            {
                m_animator.SetFloat("RotationVelocity", (m_legs != null) ? m_legs.m_rotationDirection : 0f, 0.25f);
            }
            if(m_animator != null && m_animator.m_hasPivot)
            {
                m_animator.SetBool("Pivot", (m_legs != null) ? m_legs.enabled && m_legs.ShouldPivot() : false);
            }
            
            if(m_staminaBar != null)
                m_ui?.SetBar("Stamina", (float)m_currStamina / m_startStamina);
            
            m_staminaBar?.gameObject.SetActive(m_canStagger);

            if(m_target != null)
            {
                m_head?.SetLookDirection((m_target.transform.position - transform.position).normalized);
            }
        }

        public void LoadData(ActorData _data, uint _level = 0)
        {
            //Brain
            m_startHealth = _data.health;
            m_startStamina = _data.stamina;
            m_staminaRegen = _data.staminaReg;
            m_basePhyResist = _data.phyResist;
            m_baseAbilResist = _data.abilResist;
            m_adrenalineGain = new FloatRange(_data.adrenalineGainMin, _data.adrenalineGainMax);
            m_isInvincible = _data.invincible;
            
            //Arms
            if(m_arms != null)
            {
                m_arms.m_baseDamageMod = _data.m_damageModifier;
            }

            //Legs
            if (m_legs != null)
            {
                m_legs.m_baseSpeed = _data.baseSpeed;
            }

            //Animator

            //Tracker

            //projScource

            //Material
            foreach (var material in m_materials)
            {
                material.RefreshColor();
            }

            //UI

            //Audio
            m_audioAgent?.Load(Actor_AudioAgent.SoundEffectType.Hurt, _data.hurtSounds);
            m_audioAgent?.Load(Actor_AudioAgent.SoundEffectType.Death, _data.deathSounds);
            m_audioAgent?.Finalise();

            m_ragDoll?.DisableRagdoll();

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
            m_currStamina = m_startStamina;
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

            return m_arms.GetNextAttack(m_target);
        }
        
        public void BeginAttack(int id)
        {
            if (m_arms.m_activeAttack != null)
                return;

            m_animator.ResetTrigger("Cancel");
            if (m_animator.PlayAnimation(m_arms.m_myData[id].animID))
            {
                AttackData attack = m_arms.Begin(id);
                if (attack != null)
                {

                }
            }
        }

        public void RegenStamina(float mod)
        {
            m_currStamina = Mathf.Clamp(m_currStamina + m_staminaRegen * Time.deltaTime * mod, 0, m_startStamina);
        }

        public void InvokeAttack(int id = 0)
        {
            if (m_arms == null)
                return;

            if(m_arms.Invoke((uint)id))
            {
                m_arms.PostInvoke((uint)id);
            }
            else
            {
                EndAttack();
            }
        }

        public void EndAttack()
        {
            m_arms.End(); 
        }

        public bool HandleDamage(float damage, float piercingVal, CombatSystem.DamageType _type, Vector3? _damageLoc = null, bool playAudio = true, bool canCancel = true, bool hitIndicator = true)
        {
            if (IsDead)
                return true;
            
            if(_damageLoc != null && m_legs != null && m_lookAtHit)
            {
                Vector3 direction = (_damageLoc.Value - transform.position).normalized;
                direction.y = 0;
                transform.forward = direction.normalized;
                
                if(m_arms != null)
                    this.m_arms.SetBrainLag(0.35f, true);

                if(m_target == null)
                {
                    m_target = GameManager.Instance.m_player;
                }
            }

            switch (_type)
            {
                default:
                case CombatSystem.DamageType.Physical:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, m_currPhyResist, piercingVal)); 
                    break;
                case CombatSystem.DamageType.Ability:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, m_currAbilResist, piercingVal)); 
                    break;
                case CombatSystem.DamageType.True:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, 0, 0)); 
                    break;
            }

            if (playAudio)
                HUDManager.Instance.GetDamageDisplay().DisplayDamage(transform, _type, damage);

            m_ui?.Show();
            //External
            m_refreshTimer?.Start(5.0f);

            if(m_arms != null && m_arms.hasCancel)
            {
                m_arms.End();
                foreach (var indicator in m_indicators)
                {
                    indicator.GetComponent<Animator>().SetTrigger("Cancel");
                }
                m_animator.SetTrigger("Cancel");
            }

            //Hit animation
            if (hitIndicator && _damageLoc.HasValue && m_animator != null && m_animator.m_hasHit)
            {
                m_animator.SetTrigger("Hit");
                m_animator.SetVector3("HitHorizontal", "", "HitVertical", transform.TransformVector(transform.position.DirectionTo(_damageLoc.Value)).normalized);
            }

            //Internal
            m_currHealth -= damage;
            EndScreenMenu.damageDealt += damage;
            GameManager.m_damageDealt += damage;

            if(IsDead)
            {
                GameManager.m_killCount++;
                m_ui?.SetBar("Health", 0f);
                m_legs?.Halt();
                m_arms?.End();
                foreach (var indicator in m_indicators)
                {
                    indicator.SetEnabled(false);
                }
                m_audioAgent?.PlayDeath();
            }
            else if (playAudio)
            {
                m_audioAgent?.PlayHurt();
            }

            return IsDead;
        }

        public bool HandleImpactDamage(float damage, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {
            float impactToKnockbackConst = 1/5f;
            if (IsDead && m_ragDoll != null)
            {
                m_ragDoll.m_mainCollider.GetComponent<Rigidbody>().velocity = transform.TransformVector(direction * damage * impactToKnockbackConst);
                return false;
            }

            if(!m_canStagger)
            {
                m_legs?.KnockBack(direction * damage * impactToKnockbackConst);
                return false;
            }

            switch (_type)
            {
                default:
                case CombatSystem.DamageType.Physical:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, m_currPhyResist, piercingVal));
                    break;
                case CombatSystem.DamageType.Ability:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, m_currAbilResist, piercingVal));
                    break;
                case CombatSystem.DamageType.True:
                    damage *= (1.0f - CombatSystem.CalculateDamageNegated(_type, 0, 0));
                    break;
            }
            m_currStamina = Mathf.Clamp(m_currStamina - damage, 0, m_startStamina);
            m_legs?.KnockBack(direction * (damage * impactToKnockbackConst) * 0.4f);

            return m_currStamina == 0;
        }

        public void Refresh()
        {
            m_currHealth = m_startHealth;
            foreach (var material in m_materials)
            {
                material.RefreshColor();
            }

            m_ragDoll?.DisableRagdoll();
        }

        public void DropOrbs(int amount, Vector3 position, int value)
        {
            if (value % 2 == 0)
            {
                amount = value / 2;
                value = 2;
            }
            else
            {
                amount = value;
                value = 1;
            }
                

            CurrencyDrop.CreateCurrencyDropGroup((uint)amount, position, value);
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.white;

            GetComponentInChildren<Actor_Arms>()?.DrawGizmos();
            GetComponentInChildren<Actor_Legs>()?.DrawGizmos();
            GetComponentInChildren<Actor_PatrolData>()?.DrawGizmos();
        }

        public void ShowHit()
        {
            foreach (var material in m_materials)
            {
                material.ShowHit();
            }
        }
    }
}
