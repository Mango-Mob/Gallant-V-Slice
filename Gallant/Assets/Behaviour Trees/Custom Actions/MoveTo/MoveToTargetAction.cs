using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Action/MoveTo/Target", order = 1)]
public class MoveToTargetAction : ActionNode 
{
	public float IdealStoppingDistance = 2;
	public bool LookAtTarget = true;
	protected override void Action()
	{
		//Add action code here...
		float dist = Vector3.Distance(BehaviourGraph.Owner.transform.position, BehaviourGraph.Owner.Target.transform.position);
		Vector3 forward = (BehaviourGraph.Owner.Target.transform.position - BehaviourGraph.Owner.transform.position).normalized;

		float moveDist = (dist - Mathf.Abs(IdealStoppingDistance));
		if(moveDist > 0.05f)
        {
			BehaviourGraph.Owner.Movement.SetTargetLocation(BehaviourGraph.Owner.transform.position + forward * moveDist, LookAtTarget);
		}
	}
}