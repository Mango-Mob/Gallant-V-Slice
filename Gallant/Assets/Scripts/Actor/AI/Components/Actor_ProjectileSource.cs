using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actor.AI.Components
{
    public class Actor_ProjectileSource : MonoBehaviour
    {
        public void CreateProjectile(GameObject prefab, Collider targetCollider, float damage, float force)
        {
            GameObject prefabInWorld = GameObject.Instantiate(prefab, transform.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            ProjectileObject projInWorld = prefabInWorld.GetComponent<ProjectileObject>();
            projInWorld.m_velocity = ((targetCollider.transform.position + Vector3.up * 0.5f) - transform.position).normalized * force;
            projInWorld.m_damage = damage;
            projInWorld.m_duration = 5.0f;
        }
    }
}