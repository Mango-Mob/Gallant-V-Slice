using System.Collections.Generic;

namespace ActorSystem.AI.Components
{
    public class ActorManager : SingletonPersistent<ActorManager>
    {
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
}
