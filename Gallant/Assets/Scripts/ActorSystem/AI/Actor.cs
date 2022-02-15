using ActorSystem.AI.Components;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI
{
    [RequireComponent(typeof(Actor_Brain))]
    public class Actor : StateMachine
    {
        public CombatSystem.Faction m_myFaction;

        public Actor_Brain m_myBrain { get; private set; }
        
        public uint m_myLevel = 0;
        public Transform m_selfTargetTransform;

        [SerializeField] protected ActorData m_myData;

        public GameObject m_target { get { return m_myBrain.m_target; }}

        public List<State.Type> m_states { get; private set; }

        public void Awake()
        {
            m_myBrain = GetComponent<Actor_Brain>();
            

            if (m_myData == null)
                Debug.LogWarning("Actor does not contain data.");

            ActorManager.Instance.Subscribe(this);
            m_states = new List<State.Type>(m_myData.m_states);
        }

        public void Start()
        {
            if (m_myData != null)
                m_myBrain.LoadData(m_myData, m_myLevel);

            //Start Statemachine
            SetState(m_myData.m_initialState);
        }

        public void Update()
        {
            m_myBrain.Update();
            m_currentState.Update();
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
            SetTargetVelocity(force);
        }

        public void SetTarget(GameObject _target)
        {
            m_myBrain.m_target = _target;
        }

        public void SetTargetLocation(Vector3 locVector, bool lookAtTarget = false)
        {
            m_myBrain.m_legs.SetTargetLocation(locVector, lookAtTarget);
        }

        public void SetTargetVelocity(Vector3 moveVector){m_myBrain.m_legs.SetTargetVelocity(moveVector);}
        public void SetTargetVelocity(Quaternion rotatVector) { m_myBrain.m_legs.SetTargetRotation(rotatVector); }
        public void SetTargetVelocity(Vector3 moveVector, Quaternion rotatVector)
        {
            m_myBrain.m_legs.SetTargetVelocity(moveVector);
            m_myBrain.m_legs.SetTargetRotation(rotatVector);
        }

        public void DealDamage(float _damage, CombatSystem.DamageType _type, CombatSystem.Faction _from, Vector3? _damageLoc = null)
        {
            if(!m_myBrain.IsDead)
            {
                if(m_myBrain.HandleDamage(_damage, _type))
                {
                    //Fatal Damage
                    m_myBrain.PlaySoundEffect(m_myData.deathSoundName);

                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.DropOrbs(Random.Range(2, 6));
                    SetState(new State_Dead(this));
                    return;
                }

                m_myBrain.PlaySoundEffect(m_myData.hurtSoundName);
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

        public void Kill()
        {
            if(!m_myBrain.IsDead)
                m_myBrain.HandleDamage(float.MaxValue, CombatSystem.DamageType.True);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
