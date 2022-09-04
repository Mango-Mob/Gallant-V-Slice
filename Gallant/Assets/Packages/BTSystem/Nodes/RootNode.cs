using BTSystem.Core;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BTSystem.Nodes
{
	[CreateNodeMenu("Core/Root", order = 1)]
	public class RootNode : Node
	{
		[Output] public bool start;
		// Use this for initialization
		protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return null; // Replace this
		}

		public void Execute()
		{
			(this.graph as BTGraph).currentNodes.Add(this);

			List<Node> connections = this.GetConnectionsFrom("start");
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
