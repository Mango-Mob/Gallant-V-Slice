using Actor.AI.Components;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/****************
 * Actor : An actor is a collection of game objects in a scene that represents a NPC in the game world.
 * @author : Michael Jordan
 * @file : Actor.cs
 * @year : 2021
 */
namespace Actor.AI
{
    public class Enemy : StateMachine
    {
        [Header("Actor Stats")]
        public EnemyData m_myData;
        public string m_currentStateDisplay;
        public float m_idealDistance = 0.0f;
        public float m_damageModifier = 1.0f;
        public Transform m_selfTargetTransform;

        //Accessables:
        public Actor_Legs m_legs { get; private set; } //The Legs of the actor
        public Actor_Animator m_animator { get; private set; } //The animator of the actor
        public Actor_Tracker m_tracker { get; private set; } //The stat tracker for dummy
        public Actor_ProjectileSource m_projSource { get; private set; } //Projectile Creator
        public Actor_Material m_material { get; private set; }

        public Outline m_myOutline { get; private set; }

        public Actor_UI m_ui { get; private set; }

        public GameObject m_target { get; set; } //The current focus of the actor
        public List<Actor_Attack> m_myAttacks { get; private set; } //A List of all attacks possible by the actor
        public Actor_Attack m_activeAttack { get; set; } = null; //Currently selected attack.

        //Attack Data:
        private List<Collider> m_damagedColliders; //List of all colliders damaged since last clear
        private bool m_IsWeaponLive = false;

        //Status:
        private bool m_isDead = false;
        private float m_currentHealth;
        private UI_Bar m_healthBar;
        private float m_resist;
        private float m_maxHp;

        [SerializeField] private List<Collider> m_myColliders;

        //Called upon the creation of the class.
        protected virtual void Awake()
        {
            if (m_myData != null && !m_myData.invincible)
                EnemyManager.instance.Subscribe(this);

            m_damagedColliders = new List<Collider>();

            //Load information from Scriptable Object
            m_maxHp = m_myData.health + m_myData.deltaHealth * Mathf.FloorToInt(GameManager.currentLevel);
            m_currentHealth = m_maxHp;
            m_legs = GetComponentInChildren<Actor_Legs>();
            if (m_legs != null)
                m_legs.m_baseSpeed = m_myData.baseSpeed + m_myData.deltaSpeed * Mathf.FloorToInt(GameManager.currentLevel);

            m_myColliders = new List<Collider>(GetComponentsInChildren<Collider>());
            m_animator = GetComponentInChildren<Actor_Animator>();
            m_tracker = GetComponentInChildren<Actor_Tracker>();
            m_projSource = GetComponentInChildren<Actor_ProjectileSource>();
            m_material = GetComponentInChildren<Actor_Material>();
            m_ui = GetComponentInChildren<Actor_UI>();

            m_resist = m_myData.phyResist + m_myData.deltaPhyResist * Mathf.FloorToInt(GameManager.currentLevel);
            m_tracker?.RecordResistance(m_resist);

            if (m_myData.enemyName != "")
            {
                m_myAttacks = new List<Actor_Attack>();
                //Search the system for all Actor_Attack classes under the namespace: "name_Attack"
                foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.Namespace != null && type.Namespace.ToString() == $"{m_myData.enemyName}_Attack")
                    {
                        m_myAttacks.Add(Activator.CreateInstance(type) as Actor_Attack);
                    }
                }
            }

            m_damageModifier = m_myData.m_damageModifier + m_myData.deltaDamageMod * Mathf.FloorToInt(GameManager.currentLevel);
            m_myOutline = GetComponentInChildren<Outline>();

            if (m_myOutline != null)
                m_myOutline.enabled = false;
        }

        // Called at the start of the first update call
        protected virtual void Start()
        {
            if (m_myData.m_states.Contains(State.Type.IDLE))
                SetState(new State_Idle(this));

            if (m_ui != null)
            {
                m_healthBar = m_ui.GetElement<UI_Bar>();
            }

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (m_currentState != null && m_legs != null && !m_legs.m_isKnocked)
                m_currentState.Update(); //If state exists, update it.

            if (m_animator != null && m_animator.m_hasVelocity)
                m_animator.SetFloat("VelocityHaste", m_legs.m_speedModifier);

            m_healthBar?.SetValue((float)m_currentHealth / m_maxHp);

            m_tracker?.RecordResistance(m_resist);
        }

        private void OnDestroy()
        {
            if (EnemyManager.HasInstance())
            {
                EnemyManager.instance.UnSubscribe(this);
            }
        }

        /*********************
         * OpenAttackWindow : Starts the vfx to indicate the actor is priming an attack.
         * @author: Michael Jordan
         */
        public void OpenAttackWindow()
        {
            m_IsWeaponLive = true;
        }

        /*******************
        * InvokeAttack : Start checking for collisions based on the current active attack.
        * @author : Michael Jordan
        * @param : (bool) if the active attack should be reset to null (default: false).
        */
        public void InvokeAttack(bool _resetState = false)
        {
            if (m_activeAttack != null)
            {
                List<Collider> hits = new List<Collider>(m_activeAttack.GetOverlap(this, LayerMask.NameToLayer("Player")));
                hits.AddRange(m_activeAttack.GetOverlap(this, LayerMask.NameToLayer("Shadow")));

                foreach (var hit in hits)
                {
                    if (!m_damagedColliders.Contains(hit))
                    {
                        m_damagedColliders.Add(hit);
                        //Damage player
                        m_activeAttack.Invoke(this, hit);
                    }
                }
            }

            if (_resetState)
                CloseAttackWindow();
        }

        /*********************
         * CloseAttackWindow : Resets the actor back to before the attack was made.
         * @author: Michael Jordan
         */
        public void CloseAttackWindow()
        {
            m_IsWeaponLive = false;
            m_damagedColliders.Clear();
            m_activeAttack = null;
        }

        /*******************
        * DealDamage : Reduces the actor's current hp by the damage provided, will ignore any negative damage and will kill the actor when hp = 0.
        * @author : Michael Jordan
        * @param : (float) the damage that will be dealt to the actor.
        * @param : (Vector3?) where the damage was deal from (default = null, for no react).
        */
        public void DealDamage(float _damage, Vector3? fromPos = null)
        {
            if (!m_isDead)
            {
                _damage = EnemyData.CalculateDamage(_damage, m_resist);

                if (_damage <= 0)
                    return;

                m_currentHealth -= _damage;
                m_tracker?.RecordDamage(_damage);

                if (m_tracker != null && m_tracker.m_enableAutoHealing)
                    m_currentHealth = m_myData.health;

                if (!m_myData.invincible)
                {
                    EndScreenMenu.damageDealt += _damage;
                }

                if (GetComponent<MultiAudioAgent>() != null && m_myData.hurtSoundName != "")
                {
                    GetComponent<MultiAudioAgent>().PlayOnce(m_myData.hurtSoundName, false, UnityEngine.Random.Range(0.75f, 1.35f));
                }

                if (m_currentHealth <= 0 && !m_myData.invincible)
                {
                    m_isDead = true;
                    if (GetComponent<MultiAudioAgent>() != null)
                    {
                        GetComponent<MultiAudioAgent>().PlayOnce(m_myData.deathSoundName);
                    }
                    foreach (var collider in m_myColliders)
                    {
                        collider.enabled = false;
                    }

                    float amount = UnityEngine.Random.Range(m_myData.adrenalineGainMin, m_myData.adrenalineGainMax) + m_myData.deltaAdrenaline * Mathf.FloorToInt(GameManager.currentLevel);
                    int orbCount = UnityEngine.Random.Range(2, 6);

                    AdrenalineDrop.CreateAdrenalineDropGroup((uint)orbCount, transform.position, amount / orbCount);

                    if (m_myData.m_states.Contains(State.Type.DEAD))
                    {
                        SetState(new State_Dead(this));
                    }
                }
                else if (fromPos.HasValue && m_animator.m_hasHit)
                {
                    HandleHitLoc(fromPos.Value);
                }

                m_material?.ShowHit();
            }
        }

        public void SetResistance(float m_amount)
        {
            m_resist = Mathf.Clamp(m_amount, -99.0f, m_myData.deltaSpeed * Mathf.FloorToInt(GameManager.currentLevel));
        }

        private void HandleHitLoc(Vector3 fromPos)
        {
            Vector3 direct = (fromPos - transform.position);

            m_animator.SetTrigger("Hit");
            m_animator.SetVector3("HitHorizontal", "", "HitVertical", direct.normalized);
        }

        //Draws guides only in the editor for debuging.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (m_target != null)
            {
                Gizmos.DrawLine(transform.position, m_target.transform.position);
                Gizmos.DrawSphere(m_target.transform.position, 0.5f);
            }

            if (m_myAttacks != null)
            {
                foreach (var attack in m_myAttacks)
                {
                    attack.OnGizmosDraw(this);
                }
            }
        }

        public void KnockbackActor(Vector3 force)
        {
            m_legs?.KnockBack(force);
        }

        /*******************
        * DestroySelf : Destroys the gameObject itself.
        * @author : Michael Jordan
        * @param : (float) the damage that will be dealt to the actor.
        */
        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public bool CheckIsDead()
        {
            return m_isDead;
        }

        public void Kill()
        {
            if (!m_isDead)
            {
                m_currentHealth = 0;

                if (m_tracker != null && m_tracker.m_enableAutoHealing)
                    m_currentHealth = m_myData.health;

                if (m_currentHealth <= 0 && !m_myData.invincible)
                {
                    m_isDead = true;

                    if (m_myData.m_states.Contains(State.Type.DEAD))
                    {
                        SetState(new State_Dead(this));
                    }
                }
            }
        }
    }
}

