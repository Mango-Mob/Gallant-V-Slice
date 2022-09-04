using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Decorator/Conditions/IsAttacking", order = 1)]
public class IsAttackingDecorator : DecoratorNode 
{
	protected override bool EnterCondition()
    {
		return BehaviourGraph.IsAttacking;
    }
}