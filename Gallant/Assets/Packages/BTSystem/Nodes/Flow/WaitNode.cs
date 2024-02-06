using BTSystem.Core;
using BTSystem.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using XNode;

namespace BTSystem.Nodes.Flow
{
    [CreateNodeMenu("Flow/Wait", order = 1)]
    public class WaitNode : FlowNode, ITimer
    {
        public float WaitTime;
        public bool ExecuteOnce;

        [Range(0f, 1f)]
        public float TimerDisplay;
        [Input] public float timer;
        private float m_timer;
        private float m_waitTime;
        private void Awake()
        {
            if (Application.isPlaying)
            {
                m_waitTime = WaitTime;
                m_timer = m_waitTime;
            }
        }

        public override bool CanExecute()
        {
            return base.CanExecute() && (m_timer > 0 || !ExecuteOnce);
        }

        public override void Execute()
        {
            base.Execute();

            if (m_timer > 0)
            {
                UpdateTimer();
                if(m_timer <= 0 && ExecuteOnce)
                {
                    List<Node> connections = this.GetConnectionsFrom("exits");
                    for (int i = 0; i < connections.Count; i++)
                    {
                        (connections[i] as BaseNode).Execute();
                    }
                }
                return;
            }
            else if(!ExecuteOnce)
            {
                List<Node> connections = this.GetConnectionsFrom("exits");
                for (int i = 0; i < connections.Count; i++)
                {
                    (connections[i] as BaseNode).Execute();
                }
            }
        }

        public void UpdateTimer()
        {
            m_timer -= Time.deltaTime;
        }

        public void SetTime( float time )
        {
            WaitTime = time;
        }

        public void Reset()
        {
            m_waitTime = WaitTime;
            m_timer = m_waitTime;
        }

        public float GetRemainder()
        {
            return (m_waitTime > 0) ? m_timer / m_waitTime : 0;
        }
    }
}