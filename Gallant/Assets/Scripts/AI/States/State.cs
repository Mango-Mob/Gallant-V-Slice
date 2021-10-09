using System.Collections;
using System.Collections.Generic;

public abstract class State
{
    public enum Type {IDLE, ROAM};
    protected Actor m_myActor = null;

    public State(Actor _user)
    {
        m_myActor = _user;
    }

    public abstract void Start();

    public abstract void Update();

    public abstract void End();
}
