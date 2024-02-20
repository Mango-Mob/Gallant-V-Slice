using BTSystem.Core;
using BTSystem.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BTSystem.Nodes.Flow
{
    [CreateNodeMenu("Status/Timer/ResetTimer", order = 1)]
    public class ResetTimerNode : StatusNode
    {
        [Output] public float Timer;

        public override void OnExecute()
        {
            List<Node> connections = this.GetConnectionsFrom("Timer");
            for (int i = 0; i < connections.Count; i++)
            {
                (connections[i] as ITimer)?.Reset();
            }
        }
    }
}
