using UnityEngine;
using XNode;

namespace BTSystem.Core
{
	[CreateNodeMenu("Comment", order = 1)]
	public class CommentNode : Node
	{
		[SerializeField] public Vector2 Size;
		// Use this for initialization
		protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return base.GetValue(port);
		}
	}
}