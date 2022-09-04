using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Action/HaltAction", order = 1)]
public class HaltAction : ActionNode 
{
	public bool LookAtTarget = true;
	protected override void Action()
	{
		if(LookAtTarget && BehaviourGraph.Owner.Target != null)
        {
			BehaviourGraph.Owner.Movement.SetTargetLocation(BehaviourGraph.Owner.transform.position);
			BehaviourGraph.Owner.Movement.SetTargetRotationTowards(BehaviourGraph.Owner.Target.transform.position);
		}
		else
        {
			BehaviourGraph.Owner.Movement.Halt();
		}
	}
}