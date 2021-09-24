using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
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
}
