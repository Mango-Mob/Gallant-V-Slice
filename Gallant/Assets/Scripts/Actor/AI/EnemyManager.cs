using System.Collections.Generic;

namespace Actor.AI.Components
{
    public class EnemyManager
    {
        #region Application_Singleton

        private static EnemyManager _instance = null;
        public static EnemyManager instance
        {
            get
            {
                if (_instance == null)
                {
                    return new EnemyManager();
                }
                return _instance;
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        private EnemyManager()
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

        public List<Enemy> m_subscribed { get; private set; } = new List<Enemy>();

        public void Subscribe(Enemy user)
        {
            m_subscribed.Add(user);
        }

        public void Kill(Enemy user)
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

        public void UnSubscribe(Enemy user)
        {
            if(m_subscribed.Contains(user))
                m_subscribed.Remove(user);
        }
    }
}
