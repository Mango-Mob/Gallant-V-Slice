using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_PatrolData : MonoBehaviour
    {
        public List<Transform> m_targetOrientations = new List<Transform>();
        public void Awake()
        {
            foreach (var item in m_targetOrientations)
            {
                item.SetParent(null);
            }
        }
        
        public void DrawGizmos()
        {
            for (int i = 0; i < m_targetOrientations.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(m_targetOrientations[i].position, 0.5f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(m_targetOrientations[i].position, m_targetOrientations[i].position + m_targetOrientations[i].forward);

                Gizmos.color = Color.cyan;
                if (i == m_targetOrientations.Count - 1)
                    Gizmos.DrawLine(m_targetOrientations[i].position, m_targetOrientations[0].position);
                else if(m_targetOrientations.Count > 1)
                    Gizmos.DrawLine(m_targetOrientations[i].position, m_targetOrientations[i+1].position);
            }
        }
    }
}
