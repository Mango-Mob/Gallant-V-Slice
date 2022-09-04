using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;

[CreateNodeMenu("Decorator/DistanceTo/Target", order = 1)]
public class DistanceToTargetDecorator : DecoratorNode 
{
    public enum ConditionType : int { MORE, MORE_EQUAL, LESS, LESS_EQUAL }

    [SerializeField] public ConditionType Condition;
    [SerializeField] public float TargetValue;
    protected override bool EnterCondition()
    {
        float dist = Vector3.Distance(BehaviourGraph.Owner.Target.transform.position, BehaviourGraph.Owner.transform.position);

        switch (Condition)
        {
            case ConditionType.MORE:
                return dist > TargetValue;
            case ConditionType.MORE_EQUAL:
                return dist >= TargetValue;
            case ConditionType.LESS:
                return dist < TargetValue;
            case ConditionType.LESS_EQUAL:
                return dist <= TargetValue;
            default:
                return false;
        }
    }
}