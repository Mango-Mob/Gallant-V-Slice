using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ActorSystem.AI
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Attack_Data", menuName = "Game Data/Attack Data", order = 1)]
    public class AttackData : ScriptableObject
    {
        public float baseDamage;
        public string animID;
        public float cooldown;
        public uint priority;

        public GameObject projectile;
        public GameObject vfxSpawn;

        public float requiredAngle;
        public float projSpeed;
        public float projLifeTime;

        public uint instancesPerAttack = 1;
        public CombatSystem.DamageType damageType;
        public bool canBeCanceled = false;

        public bool canAttackMove = false;
        public bool canTrackTarget = false;
        //Todo
        private bool hasExitTime;
        //Has effect

        public enum AttackType
        {
            Melee,      //Effect after animation
            Ranged,     //Spawn Proj after animation
            Instant,    //Spawn proj at target after animation
        }

        [System.Serializable]
        public class HitBox
        {
            public enum HitType { Box, Sphere, Capsule }
            public Vector3 start;
            public Vector3 end;
            public HitType type;
            public float size;
        }

        public enum Effect
        {
            NULL,
            KNOCK_BACK,
            KNOCK_UP,
        }

        public AttackType attackType;
        public HitBox attackHitbox;
        public HitBox[] damageHitboxes;
        public Effect effectAfterwards;
        public float effectPower;

        public List<Collider> GetAttackOverlaping(Transform user, int targetLayer)
        {
            List<Collider> colliders = GetOverlappingColliders(attackHitbox, user, targetLayer);
            for (int i = colliders.Count - 1; i >= 0; i--)
            {
                if (attackType == AttackType.Ranged)
                {
                    Quaternion lookAt = Quaternion.LookRotation((colliders[i].transform.position - user.position).normalized, Vector3.up);
                    if (Mathf.Abs(Quaternion.Angle(user.rotation, lookAt)) > requiredAngle)
                    {
                        colliders.RemoveAt(i);
                    }
                }
            }
            return colliders;
        }

        public List<Collider> GetDamagingOverlaping(Transform user, int targetLayer, int hitboxID = 0)
        {
            return GetOverlappingColliders(damageHitboxes[hitboxID], user, targetLayer);
        }

        public static List<Collider> GetOverlappingColliders(HitBox box, Transform user, int targetLayer)
        {
            List<Collider> colliders = new List<Collider>();
            Vector3 start = user.position + user.TransformVector(box.start);
            Vector3 end = user.position + user.TransformVector(box.end);
            switch (box.type)
            {
                case HitBox.HitType.Box:
                    break;
                default:
                case HitBox.HitType.Sphere:
                    colliders.AddRange(Physics.OverlapSphere(start, box.size, targetLayer));
                    break;
                case HitBox.HitType.Capsule:
                    colliders.AddRange(Physics.OverlapCapsule(start, end, box.size, targetLayer));
                    break;
            }
            return colliders;
        }

        public void DrawGizmos(Transform user)
        {
            Gizmos.matrix = user.localToWorldMatrix;

            Gizmos.color = Color.yellow;
            DrawHitbox(attackHitbox);

            Gizmos.color = Color.red;
            foreach (var item in damageHitboxes)
            {
                DrawHitbox(item);
            }
            
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void DrawHitbox(HitBox box)
        {
            if (box.size == 0)
                return;

            switch (box.type)
            {
                case HitBox.HitType.Box:
                    break;
                default:
                case HitBox.HitType.Sphere:
                    Gizmos.DrawWireSphere(box.start, box.size);
                    Gizmos.DrawSphere(box.start, 0.05f);
                    break;
                case HitBox.HitType.Capsule:
                    Vector3 forward = (box.end - box.start).normalized;
                    float mag = (box.end - box.start).magnitude;
                    Gizmos.matrix *= Matrix4x4.TRS(box.start, Quaternion.LookRotation(forward, Vector3.up), new Vector3(1, 1, 1));
                    Gizmos.DrawSphere(Vector3.zero, 0.05f);
                    Gizmos.DrawLine(Vector3.zero, Vector3.forward * mag);
                    Gizmos.DrawSphere(Vector3.forward * mag, 0.05f);
                    
                    Gizmos.DrawWireSphere(Vector3.zero, box.size);
                    Gizmos.DrawWireSphere(Vector3.forward * mag, box.size);

                    Gizmos.DrawLine(new Vector3(box.size, 0, 0), Vector3.forward * mag + new Vector3(box.size, 0, 0));
                    Gizmos.DrawLine(new Vector3(-box.size, 0, 0), Vector3.forward * mag + new Vector3(-box.size, 0, 0));
                    Gizmos.DrawLine(new Vector3(0, box.size, 0), Vector3.forward * mag + new Vector3(0, box.size, 0));
                    Gizmos.DrawLine(new Vector3(0, -box.size, 0), Vector3.forward * mag + new Vector3(0, -box.size, 0));
                    Gizmos.matrix *= Matrix4x4.TRS(box.start, Quaternion.LookRotation(forward, Vector3.up), new Vector3(1, 1, 1)).inverse;
                    break;
            }
            
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

