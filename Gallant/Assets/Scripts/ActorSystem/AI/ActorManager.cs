using ActorSystem.Spawning;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI
{
    public class ActorManager : SingletonPersistent<ActorManager>
    {
        public List<Actor> m_subscribed { get; private set; } = new List<Actor>();
        public List<ActorSpawner> m_activeSpawnners = new List<ActorSpawner>();

        public Dictionary<string, List<Actor>> m_reserved = new Dictionary<string, List<Actor>>();

        private List<NavMeshSurface> m_surfaces = new List<NavMeshSurface>();
        protected override void Awake()
        {
            base.Awake();
            m_surfaces.AddRange(FindObjectsOfType<NavMeshSurface>());
            foreach (var item in m_surfaces)
            {
                item.BuildNavMesh();
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            m_surfaces.Clear();
            m_surfaces.AddRange(FindObjectsOfType<NavMeshSurface>());
            m_activeSpawnners.Clear();
            foreach (var item in m_surfaces)
            {
                item.BuildNavMesh();
            }
        }

        public void AddObstacle(Transform obstacle)
        {
            float dist = float.MaxValue;
            NavMeshSurface surface = null;
            foreach (var item in m_surfaces)
            {
                float testDist = Vector3.Distance(item.transform.position, obstacle.position);
                if(testDist < dist)
                {
                    surface = item;
                    dist = testDist;
                }
            }

            if(surface != null)
            {
                obstacle.SetParent(surface.transform);
                surface.BuildNavMesh();
            }
        }

        public void RemoveObstacle(Transform obstacle)
        {
            NavMeshSurface surface = obstacle.GetComponentInParent<NavMeshSurface>();
            if (surface != null)
            {
                obstacle.SetParent(null);
                surface.BuildNavMesh();
            }
        }

        public void ClearActors()
        {
            foreach (var item in m_reserved.Values)
            {
                for (int i = item.Count - 1; i >= 0; i--)
                {
                    Destroy(item[i].gameObject);
                    item.RemoveAt(i);
                }
            }
            for (int i = m_subscribed.Count - 1; i >= 0; i--)
            {
                Destroy(m_subscribed[i].gameObject);
                m_subscribed.RemoveAt(i);
            }
            m_subscribed.Clear();
            m_reserved.Clear();
        }

        public void Subscribe(Actor user)
        {
            if(!m_subscribed.Contains(user))
                m_subscribed.Add(user);

            if (!m_reserved.ContainsKey(user.m_myData.ActorName))
                m_reserved.Add(user.m_myData.ActorName, new List<Actor>());
        }

        public void ReserveMe(Actor user)
        {
            if (m_subscribed.Contains(user))
                m_subscribed.Remove(user);

            List<Actor> list;
            int x = 0;
            if(!m_reserved.TryGetValue(user.m_name, out list))
            { 
                list = new List<Actor>();
                m_reserved.Add(user.m_name, list);
            }

            List<string> keys = new List<string>(m_reserved.Keys.ToArray());
            for (int i = 0; i < keys.Count; i++)
            {
                if(keys[i] == user.m_name)
                {
                    x = i;
                }
            }

            list.Add(user);
            user.transform.position = transform.position + new Vector3(list.Count * 2f, 0f, x * 1.5f);
            user.transform.localScale = new Vector3(1, 1, 1);
            user.transform.SetParent(transform);
        }

        public Actor GetReservedActor(string name)
        {
            List<Actor> list;
            Actor used = null;
            if (m_reserved.TryGetValue(name, out list))
            {
                bool found = false;
                if (list.Count == 0)
                {
                    for (int i = 0; i < m_subscribed.Count; i++)
                    {
                        if(m_subscribed[i].m_name == name)
                        {
                            used = Instantiate(m_subscribed[i].gameObject, transform.position, Quaternion.identity).GetComponent<Actor>();
                            used.transform.localScale = new Vector3(1, 1, 1);
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                        return null;
                }

                if(!found)
                {
                    used = list[list.Count - 1];
                }
                
                list.Remove(used);
                used.transform.SetParent(null);
                Subscribe(used);

                return used;
            }
            return null;
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
