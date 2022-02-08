using Actor.AI;
using Actor.AI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwampBoss_Attack
{
    /****************
     * SwampBoss_Attack/SpitAttack : The actor performs a spit attack.
     * @author : Michael Jordan
     * @file : SwampBoss_Attack.cs
     * @year : 2021
     */
    class SpitAttack : Actor_Attack
    {
        public SpitAttack() : base() { m_baseDamage = 15.0f; m_priority = 15; m_cooldown = 7.5f; }

        /*******************
         * BeginAttack : Starts all relevant code when an attack is activated.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void BeginAttack(Enemy user)
        {
            user.m_animator.SetTrigger("SpitWater");
            base.BeginAttack(user);
        }

        /*******************
        * GetOverlap : Gets all colliders that overlapBox with the attack's collider.
        * @author : Michael Jordan
        * @param : (Actor) the actor who is using this attack.
        * @param : (int) the layer filter for the overlap check.
        * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
        */
        public override Collider[] GetOverlap(Enemy user, int targetLayer)
        {
            List<Collider> results = new List<Collider>(Physics.OverlapSphere(user.transform.position, 15f, 1 << targetLayer));

            for (int i = results.Count - 1; i >= 0; i--)
            {
                RaycastHit hit;
                if(Physics.SphereCast(user.transform.position, 1.0f, (results[i].transform.position - user.transform.position).normalized, out hit))
                {
                    if (hit.collider != results[i] && Math.Abs(user.m_legs.GetAngleTowards(hit.collider.gameObject)) > 45)
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
        public override void Invoke(Enemy user, Collider hitCollider)
        {
            if (Math.Abs(user.m_legs.GetAngleTowards(user.m_target)) > 45)
            {
                return;
            }

            GameObject projPrefab = LoadObjectData("SpitProjectile");
            user.GetComponent<MultiAudioAgent>().Play("BossSpit");
            user.m_projSource.CreateProjectile(projPrefab, hitCollider, m_baseDamage * user.m_damageModifier, 50f);
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Enemy user)
        {
            Gizmos.DrawWireSphere(user.transform.position, 15);
        }
    }
    /****************
     * SwampBoss_Attack/RaiseAttack : The actor performs a raise attack, knocking up the player.
     * @author : Michael Jordan
     * @file : SwampBoss_Attack.cs
     * @year : 2021
     */
    class RaiseAttack : Actor_Attack
    {
        public RaiseAttack() : base() { m_baseDamage = 10.0f; m_priority = 13; m_cooldown = 8.0f; }

        /*******************
         * BeginAttack : Starts all relevant code when an attack is activated.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void BeginAttack(Enemy user)
        {
            user.m_animator.SetTrigger("RaiseWater");
            base.BeginAttack(user);
        }

        /*******************
        * GetOverlap : Gets all colliders that overlapBox with the attack's collider.
        * @author : Michael Jordan
        * @param : (Actor) the actor who is using this attack.
        * @param : (int) the layer filter for the overlap check.
        * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
        */
        public override Collider[] GetOverlap(Enemy user, int targetLayer)
        {
            return Physics.OverlapSphere(user.transform.position, 15f, 1 << targetLayer);
        }

        /*******************
         * Invoke : Deals damage to a specific collider based on this attack's stats.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @param : (Collider) the collider which is being damaged.
         */
        public override void Invoke(Enemy user, Collider hitCollider)
        {
            if(hitCollider.tag == "Player")
            {
                GameObject projPrefab = LoadObjectData("RaiseWaterProjectile");

                GameObject projInWorld = GameObject.Instantiate(projPrefab, hitCollider.transform.position + hitCollider.transform.forward * -1.25f, Quaternion.identity);
                projInWorld.transform.forward = Vector3.up;
                KnockUpArea knockUp = projInWorld.GetComponent<KnockUpArea>();
                knockUp.StartKnockUp(2.5f, m_baseDamage * user.m_damageModifier, 0.5f);
            }
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Enemy user)
        {
            Gizmos.DrawWireSphere(user.transform.position, 15);
        }
    }

    /****************
     * SwampBoss_Attack/SingleSlash : The actor performs a single slash attack.
     * @author : Michael Jordan
     * @file : SwampBoss_Attack.cs
     * @year : 2021
     */
    public class SingleSlash : Actor_Attack
    {
        //Constructor
        public SingleSlash() : base() { m_baseDamage = 15.0f; m_cooldown = 0.15f; m_priority = 8; }

        /*******************
         * GetOverlap : Gets all colliders that overlapSphere with the attack's collider.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @param : (int) the layer filter for the overlap check.
         * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
         */
        public override Collider[] GetOverlap(Enemy user, int targetLayer)
        {
            return Physics.OverlapSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f, 1 << targetLayer);
        }

        /*******************
         * BeginAttack : Starts all relevant code when an attack is activated.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void BeginAttack(Enemy user)
        {
            user.m_animator.SetTrigger("MeleeAttack");
            base.BeginAttack(user);
        }

        public override void Invoke(Enemy user, Collider hitCollider)
        {
            base.Invoke(user, hitCollider);
            user.GetComponent<MultiAudioAgent>().Play("BossAttack");
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Enemy user)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(user.transform.position + (user.transform.forward * 1.8f) + user.transform.up, 1.0f);
        }
    }

    /****************
     * SwampBoss_Attack/Kick : The actor performs a kick attack.
     * @author : Michael Jordan
     * @file : SwampBoss_Attack.cs
     * @year : 2021
     */
    public class Kick : Actor_Attack
    {
        //Constructor
        public Kick() : base() { m_baseDamage = 5.0f; m_cooldown = 3.5f; m_priority = 5; }

        /*******************
         * BeginAttack : Starts all relevant code when an attack is activated.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void BeginAttack(Enemy user)
        {
            user.m_animator.SetTrigger("KickAttack");
            
            base.BeginAttack(user);
        }

        /*******************
        * GetOverlap : Gets all colliders that overlapBox with the attack's collider.
        * @author : Michael Jordan
        * @param : (Actor) the actor who is using this attack.
        * @param : (int) the layer filter for the overlap check.
        * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
        */
        public override Collider[] GetOverlap(Enemy user, int targetLayer)
        {
            Vector3 center = user.transform.TransformPoint(Vector3.forward * 1.8f + Vector3.up);
            return Physics.OverlapBox(center, new Vector3(1.5f, 1.4f, 3f) / 2.0f, user.transform.rotation, 1 << targetLayer);
        }

        /*******************
         * Invoke : Deals damage to a specific collider based on this attack's stats.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @param : (Collider) the collider which is being damaged.
         */
        public override void Invoke(Enemy user, Collider hitCollider)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hitCollider.GetComponent<Player_Controller>();
                player.StunPlayer(0.5f, user.transform.forward * 25f);
            }
            user.GetComponent<MultiAudioAgent>().Play("KickSE");
            base.Invoke(user, hitCollider);
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Enemy user)
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = user.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.forward * 1.8f + Vector3.up, new Vector3(1.5f, 1.4f, 3f));
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

}
