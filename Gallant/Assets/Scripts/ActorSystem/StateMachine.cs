using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public string m_activeStateText = "";
    protected State m_currentState = null;

    public void SetState(State _newState)
    {
        if (_newState == null) return;

        if(m_currentState != null)
        {
            m_currentState.End();
        }

        m_currentState = _newState;
        m_currentState.Start();
    }

    public void SetState(State.Type _type)
    {
        switch (_type)
        {
            case State.Type.IDLE:
                SetState(new State_Idle(this));
                break;
            case State.Type.ROAM:
                SetState(new State_Roam(this));
                break;
            case State.Type.MOVE_TO_TARGET:
                SetState(new State_MoveToTarget(this));
                break;
            case State.Type.ATTACK:
                SetState(new State_Attack(this));
                break;
            case State.Type.STAGGER:
                SetState(new State_Staggered(this));
                break;
            case State.Type.DEAD:
                SetState(new State_Dead(this));
                break;
            case State.Type.FLEE_FROM_TARGET:
                SetState(new State_FleeFromTarget(this));
                break;
            default:
                break;
        }
    }
}
