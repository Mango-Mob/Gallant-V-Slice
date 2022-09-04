using BTSystem.Nodes;
using System.Collections.Generic;
using XNode;

namespace BTSystem.Core
{
    [NodeWidth(240)]
    public abstract class BaseNode : Node
    {
        [Output] public int decoration;

        protected BTGraph BehaviourGraph { get { return this.graph as BTGraph; } }
        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        public virtual void Execute()
        {
            (this.graph as BTGraph).currentNodes.Add(this);
        }

        public virtual bool CanExecute()
        {
            List<Node> connections = this.GetConnectionsFrom("decoration");
            bool status = true;
            foreach (var item in connections)
            {
                status = status && (item as DecoratorNode).CanEnter();
            }
            return status;
        }
    }
}