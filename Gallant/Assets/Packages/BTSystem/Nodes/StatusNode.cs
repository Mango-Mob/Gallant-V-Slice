using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

namespace BTSystem.Nodes
{
    [NodeTint("#e02f85")]
    public abstract class StatusNode : BaseNode
    {
        [Input] public bool entry;
        
        public override void Execute()
        {
            base.Execute();

            if (base.CanExecute())
                OnExecute();
        }

        public abstract void OnExecute();
    }
}
