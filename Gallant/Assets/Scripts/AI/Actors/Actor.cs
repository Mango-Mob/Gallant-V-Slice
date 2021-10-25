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

public class Actor : StateMachine
{
    [Header("Actor Stats")]
    public EnemyData m_myData;
    public string m_currentStateDisplay;

    //Accessables:
    public Actor_Legs m_legs { get; private set; } //The Legs of the actor
    public Actor_Animator m_animator { get; private set; } //The animator of the actor
    public Actor_Tracker m_tracker { get; private set; } //The stat tracker for dummy
    public Actor_ProjectileSource m_projSource { get; private set; } //Projectile Creator

    public Actor_UI m_ui { get; private set; }

    public GameObject m_target { get; set; } = null; //The current focus of the actor
    public List<Actor_Attack> m_myAttacks { get; private set; } //A List of all attacks possible by the actor
    public Actor_Attack m_activeAttack { get; set; } = null; //Currently selected attack.

    //Attack Data:
    private List<Collider> m_damagedColliders; //List of all colliders damaged since last clear
    private bool m_IsWeaponLive = false;

    //Status:
    private bool m_isDead = false;
    private float m_currentHealth;

    //Called upon the creation of the class.
    private void Awake()
    {
        m_damagedColliders = new List<Collider>();

        //Load information from Scriptable Object
        m_currentHealth = m_myData.health;

        m_legs = GetComponentInChildren<Actor_Legs>();
        m_legs.m_baseSpeed = m_myData.baseSpeed;

        m_animator = GetComponentInChildren<Actor_Animator>();
        m_tracker = GetComponentInChildren<Actor_Tracker>();
        m_projSource = GetComponentInChildren<Actor_ProjectileSource>();
        m_ui = GetComponentInChildren<Actor_UI>();

        m_tracker?.RecordResistance(m_myData.resistance);

        if (m_myData.name != "")
        {
            m_myAttacks = new List<Actor_Attack>();
            //Search the system for all Actor_Attack classes under the namespace: "name_Attack"
            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace != null && type.Namespace.ToString() == $"{m_myData.name}_Attack")
                {
                    m_myAttacks.Add(Activator.CreateInstance(type) as Actor_Attack);
                }
            }
        }
    }

    // Called at the start of the first update call
    private void Start()
    {
        if (m_myData.m_states.Contains(State.Type.IDLE))
            SetState(new State_Idle(this));
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentState != null)
            m_currentState.Update(); //If state exists, update it.

        m_animator.SetFloat("VelocityHaste", m_legs.m_speedModifier);

        if(InputManager.instance.IsKeyDown(KeyType.J))
        {
            GetComponent<StatusEffectContainer>().AddStatusEffect(new SlowStatus(1.0f, 5.0f));
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
        if(m_activeAttack != null)
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
    */
    public void DealDamage(float _damage)
    {
        if (!m_isDead)
        {
            _damage = EnemyData.CalculateDamage(_damage, m_myData.resistance);

            if (_damage <= 0)
                return;

            m_currentHealth -= _damage;
            m_tracker?.RecordDamage(_damage);

            if(m_tracker != null && m_tracker.m_enableAutoHealing)
                m_currentHealth = m_myData.health;

            if (m_currentHealth <= 0 && !m_myData.invincible)
            {
                m_isDead = true;
                if(m_myData.m_states.Contains(State.Type.DEAD))
                {
                    SetState(new State_Dead(this));
                }
            }
        }
    }

    //Draws guides only in the editor for debuging.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(m_target != null)
        {
            Gizmos.DrawLine(transform.position, m_target.transform.position);
            Gizmos.DrawSphere(m_target.transform.position, 0.5f);
        }

        if(m_myAttacks != null)
        {
            foreach (var attack in m_myAttacks)
            {
                attack.OnGizmosDraw(this);
            }
        }
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
}
