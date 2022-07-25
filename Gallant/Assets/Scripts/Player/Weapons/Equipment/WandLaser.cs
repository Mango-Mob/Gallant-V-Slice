using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

/****************
 * WandLaser: A wand laser which can be activated by the player.
 * @author : William de Beer
 * @file : WandLaser.cs
 * @year : 2021
 */
public class WandLaser : BasePlayerProjectile
{
    private float m_lifeDuration = 1.0f;
    public bool m_destructOnHit = false;
    public LineRenderer m_lineRenderer;

    private List<Actor> actorHitList = new List<Actor>();

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //m_lineRenderer.SetPosition(0, transform.position);
        Vector3 forward = m_projectileUser.playerController.playerMovement.playerModel.transform.forward;
        //m_lineRenderer.SetPosition(1, transform.position + forward * 10.0f);

        Collider[] colliders = Physics.OverlapCapsule(transform.position, transform.position + forward * 10.0f, 1, m_projectileUser.m_attackTargets);

        hitList.Clear();
        foreach (var collider in colliders)
        {
            if (hitList.Contains(collider.gameObject))
                return;
            //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            LayerMask layerMask = m_projectileUser.m_attackTargets;
            if (layerMask == (layerMask | (1 << collider.gameObject.layer)) || (m_canCollideWithEnvironment && collider.gameObject.layer == LayerMask.NameToLayer("Environment")))
            {
                BeamCollide(collider);
            }
        }
    }

    private bool BeamCollide(Collider other)
    {
        Actor actor = other.GetComponentInParent<Actor>();
        if (actor != null && hitList.Contains(actor.gameObject))
            return false;

        bool isRubble = other.gameObject.layer == LayerMask.NameToLayer("Rubble");

        Debug.Log("Hit " + other.name + " with " + m_weaponData.weaponType + " for " + m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f));

        //if (!isRubble)
        //{
        //    m_projectileUser.playerController.playerAudioAgent.PlayWeaponHit(m_weaponData.weaponType, 2); // Audio

        //    if (m_overrideHitVFXColor && m_weaponData.abilityData && m_overrideHitVFX)
        //        m_projectileUser.CreateVFX(other, transform.position, m_weaponData.abilityData.droppedEnergyColor, m_overrideHitVFX);
        //    else
        //        m_projectileUser.CreateVFX(other, transform.position, m_overrideHitVFX);
        //}
        //if (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        //{
        //    EnvironmentCollision(other);
        //}

        m_projectileUser.DamageTarget(other.gameObject, Time.fixedDeltaTime * m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f), m_weaponData.m_impact * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altImpactMult : 1.0f), 0, CombatSystem.DamageType.Physical, m_weaponData.abilityData != null ? m_weaponData.abilityData.m_tags : null);

        if (actor != null)
        {
            if (actor.m_myBrain.IsDead)
                return false;

            hitList.Add(actor.gameObject);
            //actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_weaponData.m_impact * m_charge);

            if (m_appliedStatusOnHit != EnemyStatus.NONE)
            {
                StatusEffectContainer statusContainer = actor.GetComponent<StatusEffectContainer>();
                if (statusContainer)
                {
                    StatusEffect newStatusEffect = null;
                    switch (m_appliedStatusOnHit)
                    {
                        case EnemyStatus.STUN:
                            newStatusEffect = new StunStatus(m_statusDuration);
                            break;
                        case EnemyStatus.BURN:
                            newStatusEffect = new BurnStatus(m_weaponData.m_damage * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f), m_statusDuration);
                            break;
                        case EnemyStatus.SLOW:
                            newStatusEffect = new SlowStatus(m_statusStrengthMult, m_statusDuration);
                            break;
                    }

                    if (newStatusEffect != null)
                        statusContainer.AddStatusEffect(newStatusEffect);
                }
            }
            return true;
        }

        return false;
    }
    protected override void EnvironmentCollision(Collider _other)
    {
        //Destruct();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (hitList.Contains(other.gameObject))
    //        return;
    //    //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
    //    LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
    //    if (layerMask == (layerMask | (1 << other.gameObject.layer)) || (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment")))
    //    {
    //        ProjectileCollide(other);
    //        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable") && m_destructOnHit)
    //            Destruct();
    //    }
    //}
}
