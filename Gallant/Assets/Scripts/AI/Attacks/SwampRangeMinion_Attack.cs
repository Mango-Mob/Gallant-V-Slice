using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwampRangeMinion_Attack
{
    /****************
     * SwampBoss_Attack/SingleSlash : The actor performs a single slash attack.
     * @author : Michael Jordan
     * @file : SwampBoss_Attack.cs
     * @year : 2021
     */
    public class Throw : Actor_Attack
    {
        //Constructor
        public Throw() : base() { m_baseDamage = 10.0f; m_cooldown = 1.5f; m_priority = 8; }

        public override void BeginAttack(Actor user)
        {
            user.m_animator.SetTrigger("RangeAttack");
            base.BeginAttack(user);
        }

        /*******************
        * GetOverlap : Gets all colliders that overlapBox with the attack's collider.
        * @author : Michael Jordan
        * @param : (Actor) the actor who is using this attack.
        * @param : (int) the layer filter for the overlap check.
        * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
        */
        public override Collider[] GetOverlap(Actor user, int targetLayer)
        {
            List<Collider> results = new List<Collider>(Physics.OverlapSphere(user.transform.position, 15f, 1 << targetLayer));

            for (int i = results.Count - 1; i >= 0; i--)
            {
                RaycastHit hit;
                if (Physics.SphereCast(user.transform.position, 1.0f, (results[i].transform.position - user.transform.position).normalized, out hit))
                {
                    if (hit.collider != results[i] || Math.Abs(user.m_legs.GetAngleTowards(hit.collider.gameObject)) > 45)
                    {
                        results.RemoveAt(i);
                    }
                }
                else
                    results.RemoveAt(i);
            }

            return results.ToArray();
        }

        /*******************
         * Invoke : Deals damage to a specific collider based on this attack's stats.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @param : (Collider) the collider which is being damaged.
         */
        public override void Invoke(Actor user, Collider hitCollider)
        {
            if(Math.Abs(user.m_legs.GetAngleTowards(user.m_target)) > 45)
            {
                return;
            }

            GameObject projPrefab = LoadObjectData("RockProjectile");
            user.m_projSource.CreateProjectile(projPrefab, hitCollider, m_baseDamage * user.m_damageModifier, 25f);
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Actor user)
        {
            Gizmos.DrawWireSphere(user.transform.position, 15);
        }
    }
}
