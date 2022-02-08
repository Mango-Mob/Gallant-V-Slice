using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actor.AI.Components
{
    public class Actor_ProjectileSource : MonoBehaviour
    {
        public void CreateProjectile(AttackData data, Collider targetCollider, float damageMod)
        {
            GameObject prefabInWorld = GameObject.Instantiate(data.projectile, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            ProjectileObject projInWorld = prefabInWorld.GetComponent<ProjectileObject>();
            projInWorld.m_velocity = ((targetCollider.transform.position + Vector3.up * 0.5f) - transform.position).normalized * data.projSpeed;
            projInWorld.m_damage = damageMod * data.baseDamage;
            projInWorld.m_duration = data.projLifeTime;
        }
    }
}