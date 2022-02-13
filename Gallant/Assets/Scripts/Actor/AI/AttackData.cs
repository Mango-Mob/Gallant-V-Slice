using Actor.AI.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Actor.AI
{
    [Serializable]
    public class AttackData : ScriptableObject
    {
        public float baseDamage;
        public float attackRange;
        public string animID;
        public float cooldown;
        public uint priority;

        public GameObject projectile;
        public GameObject vfxSpawn;

        public Vector3 attackOriginOffset;
        public float projSpeed;
        public float projLifeTime;

        public bool IsReady { get { return m_timer <= 0.0f; } }
        private float m_timer = 0f;

        public uint instancesPerAttack = 1;

        //TODO:
        private bool canAttackMove = false;
        private bool hasExitTime;
        private bool canBeCanceled;
        //Has effect

        public enum AttackType
        {
            Melee,      //Effect after animation
            Ranged,     //Spawn Proj after animation
            Instant,    //Spawn proj at target after animation
        }

        public AttackType attackType;

        public void Awake()
        {
            attackOriginOffset = Vector3.forward;
            attackRange = 1.0f;
        }

        public void Begin()
        {
            m_timer = cooldown;
        }

        public void Update()
        {
            m_timer = Mathf.Clamp(m_timer - Time.deltaTime, 0.0f, cooldown);
        }

        public void InvokeDamage(GameObject source, float mod, Collider hit)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hit.GetComponent<Player_Controller>();
                player.DamagePlayer(baseDamage * (mod), source);
            }
        }

        public GameObject SpawnProjectileInstantly(GameObject target, float mod)
        {
            GameObject proj = GameObject.Instantiate(projectile, target.transform.position + target.transform.TransformVector(attackOriginOffset), Quaternion.identity);
            proj.GetComponent<KnockUpArea>().StartKnockUp(2.5f, mod * baseDamage, 0.5f);
            return proj;
        }

        public bool IsOverlaping(Transform user, int targetLayer)
        {
            return GetOverlaping(user, targetLayer).Length > 0;
        }

        public Collider[] GetOverlaping(Transform user, int targetLayer)
        {
            Vector3 position = user.position + user.TransformVector(attackOriginOffset);
            return Physics.OverlapSphere(position, attackRange, 1 << targetLayer);
        }

        public void DrawGizmos(Transform user)
        {
            Vector3 position = user.position + user.TransformVector(attackOriginOffset);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(position, attackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.05f);
        }
    }

    /****************
     * AttackPrioritySort : A class used to compare Actor_Attacks based on the priority stat.
     * @author : Michael Jordan
     * @file : Actor_Attack.cs
     * @year : 2021
     */
    public class AttackPrioritySort : IComparer<AttackData>
    {
        public int Compare(AttackData a, AttackData b)
        {
            int priorityA = (a != null && a.IsReady) ? (int)a.priority : int.MaxValue;
            int priorityB = (b != null && b.IsReady) ? (int)b.priority : int.MaxValue;

            return (priorityA - priorityB);
        }
    }
}

