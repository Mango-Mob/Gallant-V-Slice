using ActorSystem.AI;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.Data
{
    public abstract class AttackData : ScriptableObject
    {
        public bool debugShow = false;

        [Header("Attack Variables")]
        public string animID = "";
        public uint priority = 10; //lower the better
        public float cooldown = 5;
        public float brainLag = 2;
        public CombatSystem.DamageType damageType;
        public float baseDamage = 10;
        public GameObject postVFXPrefab;

        public bool canContinueAfterMiss = false;
        public bool canBeCanceled = true;
        public bool canTrackTarget = true;
        public bool canAttackMove = true;
        public bool isIdleAfterAttack = false;
        [SerializeField] protected Hitbox detectCollider;
        [SerializeField] protected Hitbox[] damageColliders;

        public enum Effect
        {
            NULL,
            KNOCK_BACK,
            KNOCK_UP,
            STUN,
        }

        [Header("Effect Data")]
        public Effect onHitEffect;
        public float effectPower;

        public virtual bool CanAttack(Transform parent, GameObject target) { return true; }

        public bool HasDetectedCollider(Transform parent, int filter)
        {
            return GetDetectOverlap(parent, filter).Length > 0;
        }

        public Collider[] GetDetectOverlap(Transform parent, int filter)
        {
            return detectCollider.GetOverlappingObjects(parent, filter);
        }

        protected Collider[] GetDamageOverlap(Transform parent, int filter, uint id = 0)
        {
            if (id >= damageColliders.Length)
                return null;

            return damageColliders[id].GetOverlappingObjects(parent, filter);
        }

        public void DrawGizmos(Transform user)
        {
            Gizmos.color = Color.yellow;
            detectCollider.DrawGizmos(user);

            Gizmos.color = Color.red;
            foreach (var item in damageColliders)
            {
                item.DrawGizmos(user);
            }
        }

        public Vector3 GetHitLocation(Transform parent, uint id)
        {
            if (id >= damageColliders.Length)
                return parent.transform.position;

            return damageColliders[id].GetHitlocation(parent);
        }

        public static void ApplyEffect(Player_Controller target, Transform source, AttackData.Effect effect, float power)
        {
            switch (effect)
            {
                case AttackData.Effect.NULL:
                    break;
                case AttackData.Effect.KNOCK_BACK:
                    Vector3 direction = target.transform.position - source.position;
                    direction.y = 0;
                    target.StunPlayer(0.2f, direction.normalized * power);
                    break;
                case AttackData.Effect.KNOCK_UP:
                    target.StunPlayer(0.2f, Vector3.up * power);
                    break;
                case AttackData.Effect.STUN:
                    target.StunPlayer(power, Vector3.zero);
                    break;
                default:
                    break;
            }
        }

        public virtual void BeginActor(Actor user)
        {
            if (canAttackMove)
            {
                user.SetTargetLocation(user.m_target.transform.position, canTrackTarget);
            }
            else if (canTrackTarget)
            {
                user.SetTargetOrientaion(user.m_target.transform.position);
            }
        }

        public virtual void StartActor(Actor user)
        {
            //Do nothing
        }

        public virtual void UpdateActor(Actor user)
        {
            if (canAttackMove)
            {
                user.SetTargetLocation(user.m_target.transform.position, canTrackTarget);
            }
            else if (canTrackTarget)
            {
                user.SetTargetOrientaion(user.m_target.transform.position);
            }
        }

        public virtual void EndActor(Actor user)
        {
            if(isIdleAfterAttack)
            {
                user.SetState(new State_Idle(user, brainLag));
            }    
            else if (canAttackMove)
            {
                user.SetTargetLocation(user.m_target.transform.position, canTrackTarget);
            }
            else if (canTrackTarget)
            {
                user.SetTargetOrientaion(user.m_target.transform.position);
            }
        }

        public virtual void PostInvoke(Transform user, uint id)
        {
            if (postVFXPrefab != null)
            {
                Vector3 hitloc = GetHitLocation(user, id);
                RaycastHit hit;
                if (Physics.Raycast(hitloc, Vector3.down, out hit, 15f, 1 << LayerMask.NameToLayer("Environment")))
                {
                    GameObject vfx = Instantiate(postVFXPrefab, hit.point, Quaternion.identity);
                    vfx.transform.forward = user.forward;
                }
            }
        }
        public abstract bool InvokeAttack(Transform parent, ref GameObject source, int filter, uint id = 0, float damageMod = 1);
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

