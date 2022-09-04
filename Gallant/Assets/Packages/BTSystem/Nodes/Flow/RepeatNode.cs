using System.Collections.Generic;
using BTSystem.Core;
using XNode;

namespace BTSystem.Nodes.Flow
{
    [CreateNodeMenu("Flow/Repeat", order = 1)]
    public class RepeatNode : FlowNode
    {
        public uint count;
        public override void Execute()
        {
            base.Execute();

            List<Node> connections = this.GetConnectionsFrom("exits");
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < connections.Count; j++)
                {
                    if((connections[j] as BaseNode).CanExecute())
                    {
                        (connections[j] as BaseNode).Execute();
                    }
                }
            }
        }
    }
}