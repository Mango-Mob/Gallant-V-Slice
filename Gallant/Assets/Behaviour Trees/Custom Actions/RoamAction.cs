using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;
using EntitySystem.Core.AI;

[CreateNodeMenu("Action/RoamAction", order = 1)]
public class RoamAction : ActionNode 
{
	public float WalkRange;

	private bool _hasTargetLocation;

	private Vector3 _currentTargetLocation;

	protected override void Action()
	{
		if(!_hasTargetLocation)
        {
			_hasTargetLocation = true;
			_currentTargetLocation = GetNewRoamPoint();
		}
		else
        {
			float dist = Vector3.Distance(_currentTargetLocation, BehaviourGraph.Owner.transform.position);
			if (dist < 0.05f)
            {
				_hasTargetLocation = false;
			}

			BehaviourGraph.Owner.Movement.SetTargetLocation(_currentTargetLocation, true);
		}
	}

	private Vector3 GetNewRoamPoint()
	{
		Vector3 randomDirection = Random.insideUnitSphere * Mathf.Abs(WalkRange);
		randomDirection += (this.graph as BTGraph).Owner.transform.position;

		UnityEngine.AI.NavMeshHit hit;
		UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, WalkRange, 1);
		return hit.position;
	}
}