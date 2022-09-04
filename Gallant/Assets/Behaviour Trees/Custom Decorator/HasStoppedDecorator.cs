using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Decorator/HasStopped", order = 1)]
public class HasStopped : DecoratorNode 
{
	protected override bool EnterCondition()
    {
        return BehaviourGraph.Owner.Movement.Velocity == Vector3.zero;
    }
}