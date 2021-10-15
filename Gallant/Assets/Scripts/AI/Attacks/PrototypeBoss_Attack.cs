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
        public SingleSlash() : base() { m_baseDamage = 15.0f; m_cooldown = 0.15f; }

        //Condition check to figure out if this attack is viable.
        public override bool IsWithinRange(Actor user, int targetLayer) 
        {
            return GetOverlap(user, targetLayer).Length > 0; 
        }

        public override Collider[] GetOverlap(Actor user, int targetLayer)
        {
            return Physics.OverlapSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f, 1 << targetLayer);
        }

        //Start the attack animations and effects.
        public override void BeginAttack(Actor user) 
        {
            user.animator.SetTrigger("MeleeAttack");
            m_timeSinceLastAttack = DateTime.Now;
        }

        //Invoke the damage related to this attack.
        public override void Invoke(Actor user, Collider hitCollider) 
        { 
            if(hitCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hitCollider.GetComponent<Player_Controller>();
                player.DamagePlayer(m_baseDamage * user.m_myData.m_damageModifier);
            }
            else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Shadow"))
            {
                //Damage shadow
                AdrenalineProvider provider = hitCollider.GetComponent<AdrenalineProvider>();
                provider.GiveAdrenaline();
            }
        }

        //For debug perposes
        public override void OnGizmosDraw(Actor user) 
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f);
        }

        
    }

    public class Kick : Actor_Attack
    {
        public Kick() : base() { m_baseDamage = 5.0f; m_cooldown = 3.5f; }

        public override void BeginAttack(Actor user)
        {
            user.animator.SetTrigger("KickAttack");
            m_timeSinceLastAttack = DateTime.Now;
        }

        public override Collider[] GetOverlap(Actor user, int targetLayer)
        {
            Vector3 center = user.transform.TransformPoint(Vector3.forward * 1.8f + Vector3.up);
            return Physics.OverlapBox(center, new Vector3(1.5f, 1.4f, 3f) / 2.0f, user.transform.rotation, 1 << targetLayer);
        }

        public override void Invoke(Actor user, Collider hitCollider)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hitCollider.GetComponent<Player_Controller>();
                player.DamagePlayer(m_baseDamage * user.m_myData.m_damageModifier);
                //Add player knockback
            }
            else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Shadow"))
            {
                //Damage shadow
                AdrenalineProvider provider = hitCollider.GetComponent<AdrenalineProvider>();
                provider.GiveAdrenaline();
            }
        }

        public override bool IsWithinRange(Actor user, int targetLayer)
        {
            return GetOverlap(user, targetLayer).Length > 0;
        }

        public override void OnGizmosDraw(Actor user)
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = user.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.forward * 1.8f + Vector3.up, new Vector3(1.5f, 1.4f, 3f));
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
