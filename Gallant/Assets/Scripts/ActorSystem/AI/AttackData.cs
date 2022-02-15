using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    [System.Serializable]
    public class AttackData : ScriptableObject
    {
        public float baseDamage;
        public float attackRange = 1;
        public string animID;
        public float cooldown;
        public uint priority;

        public GameObject projectile;
        public GameObject vfxSpawn;

        public Vector3 attackOriginOffset = Vector3.forward;
        public float projSpeed;
        public float projLifeTime;

        public uint instancesPerAttack = 1;
        public CombatSystem.DamageType damageType;
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

        public bool IsOverlaping(Transform user, int targetLayer)
        {
            return GetOverlaping(user, targetLayer).Length > 0;
        }

        public Collider[] GetOverlaping(Transform user, int targetLayer)
        {
            Vector3 position = user.position + user.TransformVector(attackOriginOffset);
            return Physics.OverlapSphere(position, attackRange, targetLayer);
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
            int priorityA = (a != null) ? (int)a.priority : int.MaxValue;
            int priorityB = (b != null) ? (int)b.priority : int.MaxValue;

            return (priorityA - priorityB);
        }
    }
}

