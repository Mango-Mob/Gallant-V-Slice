using BTSystem.Core;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BTSystem.Nodes
{
	public abstract class ActionNode : BaseNode
	{
		// Use this for initialization
		[Input] public bool entry;
		[Output(connectionType = ConnectionType.Override)] public bool exit;
		protected override void Init()
		{
			base.Init();
		}
		public override object GetValue(NodePort port)
		{
			return null;
		}
		public override void Execute()
        {
			base.Execute();

			Action();

			List<Node> connections = this.GetConnectionsFrom("exit");
			for (int j = 0; j < connections.Count; j++)
			{
				if ((connections[j] as BaseNode).CanExecute())
				{
					(connections[j] as BaseNode).Execute();
				}
			}
		}

		protected abstract void Action();
	}
}