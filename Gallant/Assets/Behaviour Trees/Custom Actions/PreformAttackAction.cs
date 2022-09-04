using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;
using ActorSystem.Data;

[CreateNodeMenu("Action/PreformAttack", order = 1)]
public class PreformAttackAction : ActionNode 
{
	public AttackData Attack;

    public override bool CanExecute()
    {
        Transform user = BehaviourGraph.Owner.transform;

        return base.CanExecute() && Attack.HasDetectedCollider(user, BehaviourGraph.Owner.TargetMask);
    }
    protected override void Action()
	{
		if(!BehaviourGraph.IsAttacking)
        {
            BehaviourGraph.Owner.Attack.Perform(Attack);
        }
	}
}