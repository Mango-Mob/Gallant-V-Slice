using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Actor_Attack
{
    protected float m_baseDamage;
    protected float m_cooldown; //In seconds

    protected DateTime m_timeSinceLastAttack;
    
    public Actor_Attack()
    {
        m_timeSinceLastAttack = DateTime.Now;
    }

    public bool IsAvailable()
    {
        return (DateTime.Now - m_timeSinceLastAttack).TotalSeconds >= m_cooldown;
    }

    //Condition check to figure out if this attack is viable.
    public abstract bool IsWithinRange(Actor user, int targetLayer);

    //Get all the colliders that overlap the user.
    public abstract Collider[] GetOverlap(Actor user, int targetLayer);

    //Start the attack animations and effects.
    public abstract void BeginAttack(Actor user);

    //Invoke the damage related to this attack.
    public abstract void Invoke(Actor user, Collider hitCollider);

    //For debug perposes
    public abstract void OnGizmosDraw(Actor user);
}
