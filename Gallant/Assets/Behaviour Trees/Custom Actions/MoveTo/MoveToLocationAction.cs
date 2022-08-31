using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Action/MoveTo/Location", order = 1)]
public class MoveToLocationAction : ActionNode 
{
	[Input] public Vector3 TargetLocation;
	protected override void Action()
	{
		Vector3 moveToLoc = this.GetInputValue<Vector3>("TargetLocation", BehaviourGraph.Owner.transform.position);
		BehaviourGraph.Owner.Movement.SetTargetLocation(moveToLoc, true);
	}
}