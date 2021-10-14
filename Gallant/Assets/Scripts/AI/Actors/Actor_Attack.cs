using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Actor_Attack
{
    //Condition check to figure out if this attack is viable.
    public abstract bool IsWithinRange(Actor user, int targetLayer);

    //Get all the colliders that overlap the user.
    public abstract Collider[] GetOverlap(Actor user, int targetLayer);

    //Start the attack animations and effects.
    public abstract void BeginAttack(Actor user);

    //Invoke the damage related to this attack.
    public abstract void Invoke(Actor user);

    //For debug perposes
    public abstract void OnGizmosDraw(Actor user);
}
