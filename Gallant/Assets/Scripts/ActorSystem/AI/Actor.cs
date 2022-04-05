﻿using ActorSystem.AI.Components;
using ActorSystem.Data;
using ActorSystem.Spawning;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI
{
    [RequireComponent(typeof(Actor_Brain))]
    public class Actor : StateMachine
    {
        public bool m_toReserveOnLoad = false;
        public Actor_Brain m_myBrain { get; protected set; }
        public Actor_SpawnMethod m_mySpawn { get; protected set; }
        public ActorSpawner m_lastSpawner { get; set; }

        public uint m_myLevel = 0;
        public string m_name = "";
        public Transform m_selfTargetTransform;
        public GameObject m_HurtVFXPrefab;

        [SerializeField] protected ActorData m_myData;

        public GameObject m_target { get { return m_myBrain.m_target; }}

        public List<State.Type> m_states { get; private set; }

        protected virtual void Awake()
        {
            m_myBrain = GetComponent<Actor_Brain>();
            m_mySpawn = GetComponent<Actor_SpawnMethod>();

            if (m_myData == null)
                Debug.LogWarning("Actor does not contain data.");
            else
                m_name = m_myData.ActorName;

            if (m_toReserveOnLoad)
            {
                ActorManager.Instance.ReserveMe(this);
            }
            else
            {
                ActorManager.Instance.Subscribe(this);
            }
                
            m_states = new List<State.Type>(m_myData.m_states);
        }

        protected virtual void Start()
        {
            //Start Statemachine
            SetState(m_myData.m_initialState);

            if (m_toReserveOnLoad)
            {
                m_myBrain.SetEnabled(false);
            }
            else
            {
                m_myBrain.LoadData(m_myData, (uint)Mathf.FloorToInt(GameManager.currentLevel));
                m_myBrain.SetEnabled(true);
            }
            m_myBrain.m_canStagger = m_states.Contains(State.Type.STAGGER);
        }

        public void Spawn(uint level, Vector3 start, Vector3 end, Vector3 forward)
        {
            m_myLevel = level;
            m_myBrain.LoadData(m_myData, level);
            m_mySpawn.SetEnabled(true);
            m_mySpawn.StartSpawn(start, end, forward);
        }

        public void SetLevel(uint level)
        {
            m_myLevel = level;
            m_myBrain.LoadData(m_myData, level);
        }

        protected virtual void Update()
        {
            m_myBrain.Update();

            if(m_myBrain.enabled)
            {
                if (!(m_currentState is State_Staggered))
                    m_myBrain.RegenStamina(1.0f);

                m_currentState.Update();
            }
        }

        public void DisableFunction()
        {
            m_myBrain.SetEnabled(false);
        }

        public void OnDestroy()
        {
            if (ActorManager.HasInstance())
                ActorManager.Instance.UnSubscribe(this);
        }

        private void OnDrawGizmosSelected()
        { 
            GetComponent<Actor_Brain>().DrawGizmos();
        }

        public void KnockbackActor(Vector3 force)
        {
            if(m_myBrain.m_legs != null && m_myBrain.m_legs.enabled)
                m_myBrain.m_legs.KnockBack(force);
        }   

        public void SetTarget(GameObject _target)
        {
            m_myBrain.m_target = _target;
        }

        public void SetTargetLocation(Vector3 locVector, bool lookAtTarget = false)
        {
            m_myBrain.m_legs.SetTargetLocation(locVector, lookAtTarget);
        }

        public void SetTargetVelocity(Vector3 moveVector){m_myBrain.m_legs?.SetTargetVelocity(moveVector);}
        public void SetTargetVelocity(Quaternion rotatVector) { m_myBrain.m_legs?.SetTargetRotation(rotatVector); }
        public void SetTargetOrientaion(Vector3 targetLook) { SetTargetVelocity(Quaternion.LookRotation((targetLook - transform.position).normalized, Vector3.up)); }
        public void SetTargetVelocity(Vector3 moveVector, Quaternion rotatVector)
        {
            m_myBrain.m_legs?.SetTargetVelocity(moveVector);
            m_myBrain.m_legs?.SetTargetRotation(rotatVector);
        }

        public virtual void DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            if (m_mySpawn != null && m_mySpawn.m_spawnning)
            {
                m_mySpawn.StopSpawning();
            }
            if (!m_myBrain.IsDead)
            {
                m_myBrain.ShowHit();
                if (m_myBrain.HandleDamage(_damage, piercingVal, _type, _damageLoc))
                {
                    if(m_HurtVFXPrefab != null)
                        Instantiate(m_HurtVFXPrefab, m_selfTargetTransform.position, Quaternion.identity);

                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.DropOrbs(Random.Range(2, 6), transform.position);
                    SetState(new State_Dead(this));
                    return;
                }
            }
        }

        public virtual void DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if (m_mySpawn != null && m_mySpawn.m_spawnning)
            {
                m_mySpawn.StopSpawning();
            }
            if (!m_myBrain.IsDead)
            {
                if (m_myBrain.HandleDamage(_damage, 0, _type, transform.position, false, false, false))
                {
                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.DropOrbs(Random.Range(2, 6), transform.position);
                    SetState(new State_Dead(this));
                    return;
                }
            }
        }

        public virtual void DealImpactDamage(float amount, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {
            if (m_currentState is State_Staggered)
                return;

            if(m_myBrain.HandleImpactDamage(amount, piercingVal, direction, _type))
            {
                SetState(new State_Staggered(this));
                return;
            }
        }

        public void SetActorResistance(float val, CombatSystem.DamageType _type)
        {
            val = Mathf.Clamp(val, -99.0f, float.MaxValue);

            switch (_type)
            {
                case CombatSystem.DamageType.Physical:
                    m_myBrain.m_currPhyResist = val;
                    break;
                case CombatSystem.DamageType.Ability:
                    m_myBrain.m_currAbilResist = val;
                    break;
                default:
                case CombatSystem.DamageType.True:
                    break;
            }
        }

        public AttackData GetAttack(int id)
        {
            if(id >= 0 && m_myBrain.m_arms != null)
            {
                if(id < m_myBrain.m_arms.m_myData.Count)
                {
                    return m_myBrain.m_arms.m_myData[id];
                }
            }
            return null;
        }

        public virtual void Kill()
        {
            if(!m_myBrain.IsDead)
                m_myBrain.HandleDamage(float.MaxValue, 0, CombatSystem.DamageType.True);
        }

        public void DestroySelf()
        {
            ActorManager.Instance.ReserveMe(this);
            if(m_lastSpawner != null)
            {
                m_lastSpawner.m_myActors.Remove(this);
            }
            //Restart Statemachine
            SetState(m_myData.m_initialState);
            m_myBrain.SetEnabled(false);
            foreach (var material in m_myBrain.m_materials)
            {
                material.RefreshColor();
            }
            m_myBrain.m_ragDoll?.DisableRagdoll();
        }

        public void Respawn(bool fullRefresh = false)
        {
            if(m_lastSpawner != null)
            {
                m_myBrain.SetEnabled(false);
                SpawnData data;
                if(m_lastSpawner.GetClosestSpawn(transform.position, out data))
                {
                    SetState(m_myData.m_initialState);
                    m_mySpawn.StartSpawn(data.startPoint, data.endPoint, data.navPoint);
                }
            }
            else if (m_myBrain.m_legs != null)
            {
                Debug.LogError("TODO");
            }
        }
        public virtual void Slam()
        {
            GameManager.Instance.m_player.GetComponent<Player_Controller>().ScreenShake(6, 0.2f);
        }
    }
}
