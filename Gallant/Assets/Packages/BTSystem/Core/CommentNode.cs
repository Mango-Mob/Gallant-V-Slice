using UnityEngine;
using XNode;

namespace BTSystem.Core
{
	[CreateNodeMenu("System/Comment", order = 1)]
	public class CommentNode : Node
	{
        public int width = 400;
        public int height = 400;
        public Color color = new Color(1f, 1f, 1f, 0.1f);
		public string comment;

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