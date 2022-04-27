﻿using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * ShieldProjectile: A boomerang projectile which can be launched by the player if they are holding a boomerang.
 * @author : William de Beer
 * @file : ShieldProjectile.cs
 * @year : 2021
 */
public class ShieldProjectile : BasePlayerProjectile
{
    public int m_hitsLeft = 3;
    public float m_bounceDetectRange = 8.0f;

    private float m_rotateSpeed = 1000.0f;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        ApplyWeaponModel();

        m_projectileSpeed = m_projectileSpeed * m_weaponData.m_speed * m_projectileUser.playerController.playerStats.m_attackSpeed;
        m_rotateSpeed = 100.0f * m_projectileSpeed;
        m_throwDuration = 10.0f / (m_projectileSpeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Rotate model as it moves
        m_modelTransform.Rotate(new Vector3(0, (m_hand == Hand.LEFT ? 1.0f : -1.0f) * m_rotateSpeed * Time.fixedDeltaTime, 0));

        if (hitList.Count < 1 || m_returning)
        {
            ProjectileReturnUpdate();
        }
        else
        {
            Actor closestTarget = null;
            float closestDistance = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(transform.position, m_bounceDetectRange);
            foreach (var collider in colliders)
            {
                Actor actor = collider.GetComponentInParent<Actor>();
                if (actor == null || hitList.Contains(actor.gameObject) || actor.m_myBrain.IsDead)
                    continue;

                float distance = Vector3.Distance(actor.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestTarget = actor;
                    closestDistance = distance;
                }
            }

            if (closestTarget == null)
            {
                m_returning = true;
                return;
            }

            // Get direction towards target
            Vector3 direction = closestTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            transform.position += m_projectileSpeed * direction * Time.fixedDeltaTime; // Move projectile
        }
    }
    protected override void EnvironmentCollision()
    {
        m_returning = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject))
            return;
        //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)) || (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment")))
        {
            int hitCount = hitList.Count;
            ProjectileCollide(other);
            if (hitCount != hitList.Count)
                m_hitsLeft -= 1;
        }

        if (m_hitsLeft <= 0)
        {
            m_returning = true;
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (m_weaponData != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, m_weaponData.hitSize);
        }
    }
}