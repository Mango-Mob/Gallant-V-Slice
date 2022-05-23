using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewaveProjectile : BaseAbilityProjectile
{
    public float m_expansionRate = 1.0f;
    public float m_startingSize = 0.3f;

    private void Awake()
    {
        transform.localScale = Vector3.one * m_startingSize;
    }


    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        
    }

    private void Update()
    {
        transform.localScale += Vector3.one * m_expansionRate * Time.deltaTime;
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
                status.AddStatusEffect(new BurnStatus(m_data.effectiveness * playerController.playerStats.m_abilityDamage, m_data.duration));
            }
        }
    }
}
