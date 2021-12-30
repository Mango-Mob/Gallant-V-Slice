using System.Collections.Generic;

public class ActorManager
{
    #region Application_Singleton

    private static ActorManager _instance = null;
    public static ActorManager instance
    {
        get
        {
            if (_instance == null)
            {
                return new ActorManager();
            }
            return _instance;
        }
    }

    public static bool HasInstance()
    {
        return _instance != null;
    }

    private ActorManager()
    {
        Awake();
    }

    public static void Destroy()
    {
        if (_instance != null)
            _instance = null;
    }

    #endregion 

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public List<Actor> m_subscribed { get; private set; } = new List<Actor>();

    public void Subscribe(Actor user)
    {
        m_subscribed.Add(user);
    }

    public void Kill(Actor user)
    {
        for (int i = m_subscribed.Count - 1; i >= 0; i--)
        {
            if(m_subscribed[i] == user)
            {
                m_subscribed.RemoveAt(i);
                break;
            }
        }
        user.Kill();
    }
    public void KillAll()
    {
        for (int i = m_subscribed.Count - 1; i >= 0; i--)
        {
            m_subscribed[i].Kill();
            m_subscribed.RemoveAt(i);
        }
    }

    public void UnSubscribe(Actor user)
    {
        if(m_subscribed.Contains(user))
            m_subscribed.Remove(user);
    }
}
