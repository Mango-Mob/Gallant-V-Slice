using System;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Actor_Attack : An abstract class for an attack class.
 * @author : Michael Jordan
 * @file : Actor_Attack.cs
 * @year : 2021
 */
public abstract class Actor_Attack
{
    //Inherited values
    protected float m_baseDamage; //Base damage of the attack
    protected float m_cooldown; //In seconds
    protected uint m_priority = 0; //Priority of the attack, lowest = first.
    protected DateTime m_timeSinceLastAttack; //Time since last "BeginAttack"
    
    //Constructor
    public Actor_Attack()
    {
        m_timeSinceLastAttack = DateTime.Now;
    }

    /*******************
     * IsAvailable : Gets the current availability of the attack
     * @author : Michael Jordan
     * @return : (bool) true if the time since the last use of this attack exceeds the required cooldown.
     */
    public bool IsAvailable()
    {
        return (DateTime.Now - m_timeSinceLastAttack).TotalSeconds >= m_cooldown;
    }

    /*******************
     * LoadObjectData : Loads an object from the "Resources/EnemyAttackData/" folder.
     * @author : Michael Jordan
     * @param : (string) name of the object in "Resources/EnemyAttackData/"
     * @return : (GameObject) The GameObject that was loaded (may be null if it wasn't found).
     */
    protected GameObject LoadObjectData(string name)
    {
        GameObject result = Resources.Load<GameObject>("EnemyAttackData/" + name);

        if (result == null)
            Debug.LogError($"Failed to load Object Data: EnemyAttackData/{name}");

        return result;
    }

    /*******************
     * IsWithinRange : Is there a single collider overlapping within the attack's collider?
     * @author : Michael Jordan
     * @param : (Actor) the actor who is using this attack.
     * @param : (int) the layer filter for the overlap check.
     * @return : (bool) true if there is one or more colliders within the attack's collider.
     */
    public bool IsWithinRange(Actor user, int targetLayer)
    {
        return GetOverlap(user, targetLayer).Length > 0;
    }

    /*******************
     * GetOverlap : Gets all colliders that overlap with the attack's collider.
     * @author : Michael Jordan
     * @param : (Actor) the actor who is using this attack.
     * @param : (int) the layer filter for the overlap check.
     * @return : (Collider[]) an array of all colliders that overlap with the attack collider.
     */
    public abstract Collider[] GetOverlap(Actor user, int targetLayer);

    /*******************
     * BeginAttack : Starts all relevant code when an attack is activated.
     * @author : Michael Jordan
     * @param : (Actor) the actor who is using this attack.
     */
    public virtual void BeginAttack(Actor user) 
    {
        m_timeSinceLastAttack = DateTime.Now; 
    }

    /*******************
     * Invoke : Deals damage to a specific collider based on this attack's stats.
     * @author : Michael Jordan
     * @param : (Actor) the actor who is using this attack.
     * @param : (Collider) the collider which is being damaged.
     */
    public virtual void Invoke(Actor user, Collider hitCollider)
    {
        if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
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

    /*******************
     * GetPriority : Gets the priority of the attack.
     * @author : Michael Jordan
     * @param : (Actor) the actor who is using this attack.
     * @return : (uint) priority of the attack.
     */
    public virtual uint GetPriority(Actor user)
    {
        return m_priority;
    }

    /*******************
     * OnGizmosDraw : Draws the attack's collider to the screen using Gizmos.
     * @author : Michael Jordan
     * @param : (Actor) the actor who is using this attack.
     */
    public abstract void OnGizmosDraw(Actor user);
}

/****************
 * AttackPrioritySort : A class used to compare Actor_Attacks based on the priority stat.
 * @author : Michael Jordan
 * @file : Actor_Attack.cs
 * @year : 2021
 */
public class AttackPrioritySort : IComparer<Actor_Attack>
{
    private Actor m_user = null;
    //Constructor
    public AttackPrioritySort(Actor user) { m_user = user; }
    public int Compare(Actor_Attack a, Actor_Attack b)
    {
        int priorityA = (a != null) ? (int)a.GetPriority(m_user) : 0;
        int priorityB = (b != null) ? (int)b.GetPriority(m_user) : 0;

        return (priorityA - priorityB);
    }
}