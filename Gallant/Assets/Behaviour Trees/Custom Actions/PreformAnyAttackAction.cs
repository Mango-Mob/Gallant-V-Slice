using UnityEngine;
using XNode;
using BTSystem.Nodes;
using BTSystem.Core;
using ActorSystem.Data;

[CreateNodeMenu("Action/Attacks/PreformAnyAttack", order = 1)]
public class PreformAnyAttackAction : ActionNode 
{
	[SerializeField] private AttackData Attack;

    public override bool CanExecute()
    {
        Transform user = BehaviourGraph.Owner.transform;
        Attack = BehaviourGraph.Owner.Attack.GetFirstAvailable();
        return base.CanExecute() && Attack != null;
    }
    protected override void Action()
	{
		if(!BehaviourGraph.IsAttacking)
        {
            BehaviourGraph.Owner.Attack.Perform(Attack);
            Attack = null;
        }
	}
}