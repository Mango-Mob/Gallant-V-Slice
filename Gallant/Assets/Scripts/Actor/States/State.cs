using System.Collections;
using System.Collections.Generic;

public abstract class State
{
    public enum Type {IDLE, ROAM, MOVE_TO_TARGET, ATTACK, KEEP_AWAY_FROM_TARGET, DEAD };
    protected StateMachine m_myUser = null;

    public State(StateMachine _user)
    {
        m_myUser = _user;
    }

    public abstract void Start();

    public abstract void Update();

    public abstract void End();
}
