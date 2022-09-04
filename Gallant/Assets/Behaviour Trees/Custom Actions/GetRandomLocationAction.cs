using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Action/GetRandomLocation", order = 1)]
public class GetRandomLocationAction : ActionNode 
{
	[Output] public Vector3 Location;

	public float Range;

	protected override void Action()
	{
		Vector3 randomDirection = Random.insideUnitSphere * Mathf.Abs(Range);
		randomDirection += (this.graph as BTGraph).Owner.transform.position;

		UnityEngine.AI.NavMeshHit hit;
		UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, Range, 1);

		Location = hit.position;

	}

	public override object GetValue(NodePort port)
	{
		if (port.fieldName == "Location")
			return Location;

		return null;
	}
}