using EPOOutline;
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
        public Actor_Material[] m_materials { get; private set; } //Core texture of the actor (the renderer)
        public Actor_UI m_ui { get; private set; } //UI display for this actor
        public Actor_MiniMapIcon m_icon {get; private set;}
        public Actor_PatrolData m_patrol {get; private set;}
        public Actor_AudioAgent m_audioAgent { get; private set; }
        public Actor_Ragdoll m_ragDoll { get; private set; }
        public Outlinable m_myOutline { get; private set; }
        #endregion

        public bool IsDead { get{ return m_currHealth <= 0 && !m_isInvincible; } }
        public bool IsStunned { get; set; } = false;
        public bool m_canBeTarget = true;
        public bool m_forceShowUI = false;
        
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
                    if (material.m_isDisolving)
                        return true;
                }
                return false;
            } 
        }

        protected virtual void Awake()
        {
            m_arms = GetComponentInChildren<Actor_Arms>();
            m_legs = GetComponentInChildren<Actor_Legs>();
            m_animator = GetComponentInChildren<Actor_Animator>();
            m_materials = GetComponentsInChildren<Actor_Material>();
            m_ui = GetComponentInChildren<Actor_UI>();
            m_audioAgent = GetComponent<Actor_AudioAgent>();
            m_patrol = GetComponentInChildren<Actor_PatrolData>();
            m_ragDoll = GetComponentInChildren<Actor_Ragdoll>();
            m_icon = GetComponentInChildren<Actor_MiniMapIcon>();
            m_myOutline = GetComponent<Outlinable>();

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
            if (IsStunned || IsDead)
            {
                m_legs?.Halt();
                return;
            }

            //Externals
            UpdateExternals();
            m_refreshTimer?.Update();
            if(m_canBeTarget && m_ui != null && m_myOutline != null)
            {
                m_ui.SetEnabled(m_forceShowUI || m_myOutline.enabled);
            }
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
                m_animator?.SetFloat("VelocityHaste", (m_legs != null) ? m_legs.m_speedModifier : 1.0f);
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
            m_ui?.SetBar("Health", (float) m_currHealth / m_startHealth);

            if(m_staminaBar != null)
                m_ui?.SetBar("Stamina", (float)m_currStamina / m_startStamina);
            
            m_staminaBar?.gameObject.SetActive(m_canStagger);
        }

        public void LoadData(ActorData _data, uint _level = 0)
        {
            //Brain
            m_startHealth = _data.health + _data.deltaHealth * _level;
            m_startStamina = _data.stamina + _data.deltaStamina * _level;
            m_staminaRegen = _data.staminaReg + _data.deltaStaminaReg * _level;
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

            m_animator.ResetTrigger("Cancel");
            if (m_animator.PlayAnimation(m_arms.m_myData[id].animID))
            {
                if(m_arms.Begin(id) != null)
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

            //External
            m_refreshTimer?.Start(5.0f);

            if(m_arms != null && m_arms.hasCancel)
            {
                EndAttack();
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

            if (playAudio && IsDead)
            {
                m_audioAgent?.PlayDeath();
                m_legs?.Halt();
            }
            else if (playAudio)
            {
                m_audioAgent?.PlayHurt();
            }

            return IsDead;
        }

        public bool HandleImpactDamage(float damage, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {
            if (IsDead)
                return false;

            if(!m_canStagger)
            {
                m_legs.KnockBack(direction * damage);
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

            if (m_currStamina == 0)
            {
                m_legs.KnockBack(direction * damage);
                return true;
            }
            else
            {
                m_legs.KnockBack(direction * damage * 0.5f);
                return false;
            }
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

        public void DropOrbs(int amount, Vector3 position)
        {
            CurrencyDrop.CreateCurrencyDropGroup((uint) amount, position, m_adrenalineGain.GetRandom() / amount);
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
