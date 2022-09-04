using BTSystem.Core;
using UnityEngine;
using XNode;

namespace BTSystem.Nodes
{
	public abstract class DecoratorNode : Node
	{
		protected BTGraph BehaviourGraph { get { return this.graph as BTGraph; } }
		// Use this for initialization
		protected override void Init()
		{
			base.Init();
		}

		[Input(connectionType = ConnectionType.Override)] public int entry;

		public bool InverseInput = false;
		public bool CanEnter()
		{
			return (InverseInput) ? !EnterCondition() : EnterCondition();
		}

		protected abstract bool EnterCondition();
	}
}
