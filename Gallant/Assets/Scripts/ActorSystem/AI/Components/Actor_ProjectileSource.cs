using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_ProjectileSource : Actor_Component
    {
        public void CreateProjectile(AttackData data, Collider targetCollider, float damageMod)
        {
            GameObject prefabInWorld = GameObject.Instantiate(data.projectile, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            ProjectileObject projInWorld = prefabInWorld.GetComponent<ProjectileObject>();
            projInWorld.m_damageDetails = data;
            projInWorld.m_velocity = ((targetCollider.transform.position + Vector3.up * 0.5f) - transform.position).normalized * data.projSpeed;
            projInWorld.m_damage = damageMod * data.baseDamage;
            projInWorld.m_duration = data.projLifeTime;
        }
        public void CreateProjectileInstantly(AttackData data, Collider target, float mod)
        {
            GameObject proj = GameObject.Instantiate(data.projectile, target.transform.position, Quaternion.identity);
            proj.GetComponent<AreaEffect>().m_data = data;
            proj.GetComponent<AreaEffect>().damage = data.baseDamage * mod;
        }
        public override void SetEnabled(bool status)
        {
            this.enabled = status;
        }
    }
}