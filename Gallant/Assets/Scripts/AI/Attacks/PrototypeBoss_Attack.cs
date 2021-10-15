using UnityEngine;

namespace PrototypeBoss_Attack
{
    /****************
     * PrototypeBoss_Attack/SingleSlash : The actor performs a single slash attack.
     * @author : Michael Jordan
     * @file : PrototypeBoss_Attack.cs
     * @year : 2021
     */
    public class SingleSlash : Actor_Attack
    {
        //Constructor
        public SingleSlash() : base() { m_baseDamage = 15.0f; m_cooldown = 0.15f; m_priority = 10; }

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

    /****************
     * PrototypeBoss_Attack/Kick : The actor performs a kick attack.
     * @author : Michael Jordan
     * @file : PrototypeBoss_Attack.cs
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
        public override void BeginAttack(Actor user)
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
        public override Collider[] GetOverlap(Actor user, int targetLayer)
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
        public override void Invoke(Actor user, Collider hitCollider)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hitCollider.GetComponent<Player_Controller>();
                //Add player knockback
            }
            base.Invoke(user, hitCollider);
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Actor user)
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = user.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.forward * 1.8f + Vector3.up, new Vector3(1.5f, 1.4f, 3f));
            Gizmos.matrix = Matrix4x4.identity;
        }
    }

    /****************
     * PrototypeBoss_Attack/AOE_Attack : The actor performs a AOE attack.
     * @author : Michael Jordan
     * @file : PrototypeBoss_Attack.cs
     * @year : 2021
     */
    public class AOE_Attack : Actor_Attack
    {
        //Constructor
        public AOE_Attack() : base() { m_baseDamage = 25.0f; m_cooldown = 15f; m_priority = 15; }

        /*******************
         * BeginAttack : Starts all relevant code when an attack is activated.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void BeginAttack(Actor user)
        {
            user.m_animator.SetTrigger("AOEAttack");
            GameObject vfxPrefab = LoadObjectData("OldBoss_AOE");
            if(vfxPrefab != null)
            {
                GameObject.Instantiate(vfxPrefab, user.transform.position, user.transform.rotation);
            }
            base.BeginAttack(user);
        }

        /*******************
        * GetOverlap : Gets all colliders that overlapSphere with the attack's collider.
        * @author : Michael Jordan
        * @param : (Actor) the actor who is using this attack.
        * @param : (int) the layer filter for the overlap check.
        * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
        */
        public override Collider[] GetOverlap(Actor user, int targetLayer)
        {
            return Physics.OverlapSphere(user.transform.position, 6.8f);
        }

        /*******************
         * Invoke : Deals damage to a specific collider based on this attack's stats.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @param : (Collider) the collider which is being damaged.
         */
        public override void Invoke(Actor user, Collider hitCollider)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hitCollider.GetComponent<Player_Controller>();
                //Knockback player.
            }
            base.Invoke(user, hitCollider);
        }

        /*******************
         * GetPriority : Gets the priority of the AOE, but the priority is decreased if the user's target is behind the actor.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         * @return : (uint) priority of the attack.
         */
        public override uint GetPriority(Actor user)
        {
            float angle = user.m_legs.GetAngleTowards(user.m_target);

            return (Mathf.Abs(angle) >= 90) ? 3 : m_priority;
        }

        /*******************
         * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
         * @author : Michael Jordan
         * @param : (Actor) the actor who is using this attack.
         */
        public override void OnGizmosDraw(Actor user)
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = user.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 6.8f);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
