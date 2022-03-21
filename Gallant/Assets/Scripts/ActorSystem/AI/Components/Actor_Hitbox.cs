using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(Collider))]
    public class Actor_Hitbox : Actor_Component
    {
        private Collider m_myHitbox;

        public void Awake()
        {
            m_myHitbox = GetComponent<Collider>();
        }

        public void DealDamage(float _damage, CombatSystem.DamageType _type, CombatSystem.Faction _from, Vector3? _damageLoc = null)
        {
            //if (m_mySpawn != null && m_mySpawn.m_spawnning)
            //{
            //    m_mySpawn.StopSpawning();
            //}
            //if (!m_myBrain.IsDead)
            //{
            //    m_myBrain.m_material?.ShowHit();
            //    if (m_myBrain.HandleDamage(_damage, _type, _damageLoc))
            //    {
            //        if (m_HurtVFXPrefab != null)
            //            Instantiate(m_HurtVFXPrefab, m_selfTargetTransform.position, Quaternion.identity);
            //
            //        foreach (var collider in GetComponentsInChildren<Collider>())
            //        {
            //            collider.enabled = false;
            //        }
            //        m_myBrain.DropOrbs(Random.Range(2, 6));
            //        SetState(new State_Dead(this));
            //        return;
            //    }
            //}
        }

        public void DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            //if (m_mySpawn != null && m_mySpawn.m_spawnning)
            //{
            //    m_mySpawn.StopSpawning();
            //}
            //if (!m_myBrain.IsDead)
            //{
            //    if (m_myBrain.HandleDamage(_damage, _type, transform.position, false, false))
            //    {
            //        foreach (var collider in GetComponentsInChildren<Collider>())
            //        {
            //            collider.enabled = false;
            //        }
            //        m_myBrain.DropOrbs(Random.Range(2, 6));
            //        SetState(new State_Dead(this));
            //        return;
            //    }
            //}
        }

        public override void SetEnabled(bool status)
        {
            m_myHitbox.enabled = status;
        }
    }
}
