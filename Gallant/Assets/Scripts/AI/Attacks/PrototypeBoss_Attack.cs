using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrototypeBoss_Attack
{
    public class SingleSlash : Actor_Attack
    {
        //Condition check to figure out if this attack is viable.
        public override bool IsWithinRange(Actor user, int targetLayer) 
        {
            return GetOverlap(user, 1 << targetLayer).Length > 0; 
        }

        public override Collider[] GetOverlap(Actor user, int targetLayer)
        {
            return Physics.OverlapSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f, targetLayer);
        }

        //Start the attack animations and effects.
        public override void BeginAttack(Actor user) 
        {
            user.animator.SetTrigger("MeleeAttack");
        }

        //Invoke the damage related to this attack.
        public override void Invoke(Actor user) 
        { 

        }

        //For debug perposes
        public override void OnGizmosDraw(Actor user) 
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f);
        }

        
    }

}
