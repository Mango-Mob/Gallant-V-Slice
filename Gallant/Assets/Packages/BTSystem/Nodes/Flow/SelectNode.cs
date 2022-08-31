using System.Collections.Generic;
using XNode;
using BTSystem.Core;

namespace BTSystem.Nodes.Flow
{
	[CreateNodeMenu("Flow/Select", order = 1)]
	public class SelectNode : FlowNode
	{
		public override void Execute()
		{
			base.Execute();
			List<Node> connections = this.GetConnectionsFrom("exits");
			for (int i = 0; i < connections.Count; i++)
			{
				if ((connections[i] as BaseNode).CanExecute())
				{
					(connections[i] as BaseNode).Execute();
					return;
				}
			}
		}
	}
}
