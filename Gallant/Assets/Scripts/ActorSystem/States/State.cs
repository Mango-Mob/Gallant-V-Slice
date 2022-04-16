using System.Collections;
using System.Collections.Generic;

public abstract class State
{
    public enum Type {IDLE, ROAM, MOVE_TO_TARGET, ATTACK, STAGGER, DEAD, FLEE_FROM_TARGET, STRAFE };

    protected StateMachine m_myUser = null;
    protected ActorSystem.AI.Actor m_myActor = null;

    public State(StateMachine _user)
    {
        m_myUser = _user;
        m_myActor = _user as ActorSystem.AI.Actor;
    }

    public abstract void Start();

    public abstract void Update();

    public abstract void End();
}
