using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class Actor : StateMachine
{
    public EnemyData m_myData;

    public Actor_Legs legs { get; private set; }
    public Actor_Animator animator { get; private set; }

    public string m_currentStateDisplay;

    private bool m_isDead = false;
    private float m_currentHealth;

    protected GameObject playerObject; 

    private void Awake()
    {
        legs = GetComponentInChildren<Actor_Legs>();
        animator = GetComponentInChildren<Actor_Animator>();
        playerObject = GameManager.instance.m_player;

        m_currentHealth = m_myData.health;

        if (m_myData.m_states.Contains(State.Type.IDLE))
            SetState(new State_Idle(this));
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
    }

    public void DealDamage(float damage)
    {
        if(!m_isDead)
        {
            damage = EnemyData.CalculateDamage(damage, m_myData.resistance);

            m_currentHealth -= damage;
            if(m_currentHealth <= 0)
            {
                m_isDead = true;
            }
        }
    }
}
