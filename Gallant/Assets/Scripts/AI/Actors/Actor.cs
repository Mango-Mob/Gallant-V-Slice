using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine;


public class Actor : StateMachine
{
    public EnemyData m_myData;

    public Actor_Legs legs { get; private set; }
    public Actor_Animator animator { get; private set; }

    public string m_currentStateDisplay;

    public GameObject m_target { get; set; } = null;

    //Attack Data
    private List<Collider> m_damagedColliders;
    public List<Actor_Attack> m_myAttacks { get; private set; }
    public Actor_Attack m_activeAttack { get; set; }
    private bool m_IsWeaponLive = false;


    private bool m_isDead = false;
    private float m_currentHealth;

    protected GameObject playerObject;

    private void Awake()
    {
        legs = GetComponentInChildren<Actor_Legs>();
        animator = GetComponentInChildren<Actor_Animator>();
        playerObject = GameManager.instance.m_player;
        m_damagedColliders = new List<Collider>();

        m_currentHealth = m_myData.health;

        if (m_myData.m_states.Contains(State.Type.IDLE))
            SetState(new State_Idle(this));

        m_activeAttack = null;
        if (m_myData.name != "")
        {
            m_myAttacks = new List<Actor_Attack>();

            foreach (System.Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace != null && type.Namespace.ToString() == $"{m_myData.name}_Attack")
                {
                    m_myAttacks.Add(Activator.CreateInstance(type) as Actor_Attack);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentState != null)
            m_currentState.Update();

        if(m_IsWeaponLive && m_activeAttack != null)
        {
            Collider[] hits = m_activeAttack.GetOverlap(this, LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("Shadow"));

            foreach (var hit in hits)
            {
                m_damagedColliders.Add(hit);
                if(hit.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    //Damage player
                }
                else if(hit.gameObject.layer == LayerMask.NameToLayer("Shadow"))
                {
                    //Damage shadow
                }
            }
        }
    }

    public void OpenAttackWindow()
    {
        m_IsWeaponLive = true;
    }

    public void CloseAttackWindow()
    {
        m_IsWeaponLive = false;
        m_damagedColliders.Clear();
        m_activeAttack = null;
    }

    public void DealDamage(float damage)
    {
        if (!m_isDead)
        {
            damage = EnemyData.CalculateDamage(damage, m_myData.resistance);

            m_currentHealth -= damage;
            if (m_currentHealth <= 0)
            {
                m_isDead = true;
            }
        }
    }

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
}
