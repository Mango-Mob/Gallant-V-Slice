using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewaveProjectile : BaseAbilityProjectile
{
    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
    }
    protected override void DetonateProjectile(bool hitTarget = false)
    {
        BaseDetonateProjectile(hitTarget);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            ProjectileCollide(other);

            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                status.AddStatusEffect(new BurnStatus(m_data.effectiveness, m_data.duration));
            }
        }
    }
}
