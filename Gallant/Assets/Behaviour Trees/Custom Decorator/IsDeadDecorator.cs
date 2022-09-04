using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Decorator/Conditions/IsDead", order = 1)]
public class IsDeadDecorator : DecoratorNode 
{
	protected override bool EnterCondition()
    {
        return BehaviourGraph.IsDead;
    }
}