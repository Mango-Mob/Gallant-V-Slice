using BTSystem.Core;
using BTSystem.Nodes;
using UnityEngine;
using XNode;

[CreateNodeMenu("Decorator/HasTarget", order = 1)]
public class HasTargetDecorator : DecoratorNode 
{
	protected override bool EnterCondition()
    {
        return (graph as BTGraph).Target != null;
    }
}