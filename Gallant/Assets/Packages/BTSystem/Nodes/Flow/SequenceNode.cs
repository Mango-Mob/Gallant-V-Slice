using System.Collections.Generic;
using BTSystem.Core;
using XNode;

namespace BTSystem.Nodes.Flow
{
	[CreateNodeMenu("Flow/Sequence", order = 1)]
	public class SequenceNode : FlowNode
	{
		public override void Execute()
		{
			base.Execute();
			List<Node> connections = this.GetConnectionsFrom("exits");
			for (int i = 0; i < connections.Count; i++)
			{
				(connections[i] as BaseNode).Execute();
			}
		}
	}
}