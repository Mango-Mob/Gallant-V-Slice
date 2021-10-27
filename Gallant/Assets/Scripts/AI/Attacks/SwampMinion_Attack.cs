using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwampMinion_Attack
{
    /****************
     * SwampBoss_Attack/SingleSlash : The actor performs a single slash attack.
     * @author : Michael Jordan
     * @file : SwampBoss_Attack.cs
     * @year : 2021
     */
    public class SingleSlash : Actor_Attack
    {
        //Constructor
        public SingleSlash() : base() { m_baseDamage = 10.0f; m_cooldown = 0.15f; m_priority = 8; }

        /*******************
         * GetOverlap : Gets all colliders that overlapSphere with the attack's collider.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @param : (int) the layer filter for the overlap check.
         * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
         */
        public override Collider[] GetOverlap(Actor user, int targetLayer)
        {
            return Physics.OverlapSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f, 1 << targetLayer);
        }

        /*******************
         * BeginAttack : Starts all relevant code when an attack is activated.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void BeginAttack(Actor user)
        {
            user.m_animator.SetTrigger("MeleeAttack");
            base.BeginAttack(user);
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Actor user)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f);
        }
    }
}
