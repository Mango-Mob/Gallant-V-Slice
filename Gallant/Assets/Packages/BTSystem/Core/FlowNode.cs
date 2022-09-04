using UnityEngine;
using XNode;

namespace BTSystem.Core
{
	[NodeTint("#03fc7f")]
	public abstract class FlowNode : BaseNode
	{
		[Input] public bool entry;
		[Output] public bool exits;
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
	}
}