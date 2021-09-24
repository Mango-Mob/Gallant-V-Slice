using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : StateMachine
{
    public Actor_Legs legs { get; private set; }
    public Actor_Animator animator { get; private set; }

    public string m_currentStateDisplay; 

    protected GameObject playerObject; 


    private void Awake()
    {
        legs = GetComponentInChildren<Actor_Legs>();
        animator = GetComponentInChildren<Actor_Animator>();
        playerObject = GameManager.instance.m_player;

        SetState(new State_Idle(this));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_currentState.Update();
    }
}
