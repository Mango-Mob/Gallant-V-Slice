using System;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Attack_Data", menuName = "Game Data/Attack Data", order = 1)]
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
        public float requiredAngle;
        public float projSpeed;
        public float projLifeTime;

        public uint instancesPerAttack = 1;
        public CombatSystem.DamageType damageType;
        public bool canBeCanceled = false;

        //TODO:
        private bool canAttackMove = false;
        private bool hasExitTime;
        public bool isAwaysFacingTarget = false;
        //Has effect

        public enum AttackType
        {
            Melee,      //Effect after animation
            Ranged,     //Spawn Proj after animation
            Instant,    //Spawn proj at target after animation
        }

        public enum Effect
        {
            NULL,
            KNOCK_BACK,
            KNOCK_UP,
        }

        public AttackType attackType;
        public Effect effectAfterwards;
        public float effectPower;

        public bool IsOverlaping(Transform user, int targetLayer)
        {
            return GetOverlaping(user, targetLayer).Count > 0;
        }

        public List<Collider> GetOverlaping(Transform user, int targetLayer)
        {
            Vector3 position = user.position + user.TransformVector(attackOriginOffset);
            List<Collider> colliders = new List<Collider>();
            foreach (var collider in Physics.OverlapSphere(position, attackRange, targetLayer))
            {
                if(attackType == AttackType.Ranged)
                {
                    Quaternion lookAt = Quaternion.LookRotation((collider.transform.position - user.position).normalized, Vector3.up);
                    if (Mathf.Abs(Quaternion.Angle(user.rotation, lookAt)) <= requiredAngle)
                    {
                        colliders.Add(collider);
                    }
                }
                else
                {
                    colliders.Add(collider);
                }
            }
            return colliders;
        }

        public void DrawGizmos(Transform user)
        {
            Vector3 position = user.position + user.TransformVector(attackOriginOffset);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(position, attackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.05f);
        }

        public static void ApplyEffect(Player_Controller target, Transform source, AttackData.Effect effect, float power)
        {
            switch (effect)
            {
                case AttackData.Effect.NULL:
                    break;
                case AttackData.Effect.KNOCK_BACK:
                    target.StunPlayer(0.2f, source.position.DirectionTo(target.transform.position) * power);
                    break;
                case AttackData.Effect.KNOCK_UP:
                    target.StunPlayer(0.2f, Vector3.up * power);
                    break;
                default:
                    break;
            }
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

