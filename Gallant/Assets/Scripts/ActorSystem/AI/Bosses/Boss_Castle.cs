using ActorSystem.AI.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.AI.Bosses
{
    public class Boss_Castle : Boss_Actor
    {
        public enum Phase { WAIT, ATTACK, CHARGE, FLEE, DEAD }

        public enum ChargePhase { IDLE, AIM, RUNNING, RECOVERY}

        public float m_minDistForCharge = 5f;
        public float m_chargeAimDist = 5f;
        public float m_chargeHitDist = 2.5f;
        public float m_chargeKnockback = 20.0f;
        public bool m_chargeControlled = true;
        
        public Phase m_phase;
        public ChargePhase m_cPhase;
        public AnimationCurve m_chargeMSpeedMod;

        public CastleWallController[] m_myWalls;

        public float m_chargeCooldown = 3.0f;
        private float m_chargeCurrCd = 0.0f;
        private float m_recoveryCd = 0.0f;
        private float m_chargeTimer = 0.0f;
        private Vector3 m_chargeTarget;
        protected override void Awake()
        {
            base.Awake();
            m_phase = Phase.WAIT;
        }

        protected override void Start()
        {
            base.Start();
            base.SetTarget(GameManager.Instance.m_player);
        }

        protected override void Update()
        {
            base.Update();

            if (m_chargeCurrCd > 0)
                m_chargeCurrCd -= Time.deltaTime;

            if (m_recoveryCd > 0)
                m_recoveryCd -= Time.deltaTime;

            switch (m_phase)
            {
                case Phase.WAIT:
                    WaitPhaseFunc();
                    break;
                case Phase.ATTACK:
                    AttackPhaseFunc();
                    break;
                case Phase.CHARGE:
                    ChargePhaseFunc();
                    break;
                case Phase.FLEE:
                    break;
                case Phase.DEAD:
                    break;
                default:
                    break;
            }
        }

        protected void TransitionToPhase(Phase phase, bool ignoreCheck = false)
        {
            if (m_phase == phase && !ignoreCheck)
                return;

            if (!ignoreCheck)
            {
                switch (m_phase) //Exit
                {
                    case Phase.WAIT:
                        break;
                    case Phase.ATTACK:
                        break;
                    case Phase.CHARGE:
                        m_chargeCurrCd = m_chargeCooldown;
                        break;
                    case Phase.FLEE:
                        break;
                    case Phase.DEAD:
                        break;
                    default:
                        break;
                }
            }

            m_phase = phase;

            switch (phase)//Enter
            {
                case Phase.WAIT:
                    m_chargeTimer = 0.0f;
                    m_myBrain.m_legs.m_baseSpeed =  m_myData.baseSpeed;
                    break;
                case Phase.ATTACK:
                    m_chargeTimer = 0.0f;
                    m_myBrain.m_legs.m_baseSpeed = m_myData.baseSpeed;
                    break;
                case Phase.CHARGE:
                    m_myBrain.m_legs.m_baseSpeed = m_myData.baseSpeed * m_chargeMSpeedMod.Evaluate(m_chargeTimer);
                    m_chargeControlled = true;
                    m_myBrain.m_animator.PlayAnimation("ChargeStart");
                    m_cPhase = ChargePhase.AIM;
                    foreach (var item in m_myWalls)
                    {
                        item.SetEnabledStatus(false);
                    }
                    break;
                case Phase.FLEE:
                    break;
                case Phase.DEAD:
                    break;
                default:
                    break;
            }
        }

        public void EndChargeAim()
        {
            m_cPhase = ChargePhase.RUNNING;
        }

        private void WaitPhaseFunc()
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(m_target.transform.position, out hit, 1.0f, ~0))
            {
                TransitionToPhase(Phase.ATTACK);
                return;
            }
        }

        private void AttackPhaseFunc()
        {
            float dist = Vector3.Distance(transform.position, m_target.transform.position);
            if (dist > m_myBrain.m_legs.m_idealRange)
                SetTargetLocation(m_target.transform.position, true);
            else
                m_myBrain.m_legs.Halt();

            int index = m_myBrain.GetNextAttack();
            if(index >= 0)
            {
                m_myBrain.BeginAttack(index);
                return;
            }

            if(dist >= m_minDistForCharge && m_chargeCurrCd <= 0)
            {
                TransitionToPhase(Phase.CHARGE);
            }
        }

        private void ChargePhaseFunc()
        {
            switch (m_cPhase)
            {
                case ChargePhase.IDLE:
                    {
                        //Do Nothing
                        break;
                    }
                case ChargePhase.AIM:
                    {
                        SetTargetLocation(transform.position, false);
                        SetTargetOrientaion(m_target.transform.position);
                        m_myBrain.m_animator.SetInteger("ChargeStatus", 0);
                        break;
                    }
                case ChargePhase.RUNNING:
                    {
                        if (m_myBrain.m_legs.enabled)
                        {
                            m_chargeTimer = Mathf.Clamp(m_chargeTimer + Time.deltaTime, 0, m_chargeMSpeedMod.keys[m_chargeMSpeedMod.keys.Count() - 1].time);
                            m_myBrain.m_legs.m_baseSpeed = m_myData.baseSpeed * m_chargeMSpeedMod.Evaluate(m_chargeTimer);

                            if (m_chargeControlled)
                            {
                                SetTargetLocation(m_target.transform.position, true);
                                //If the charge is within distance, lose control.
                                if(Vector3.Distance(transform.position, m_target.transform.position) <= m_chargeAimDist)
                                {
                                    m_chargeControlled = false;
                                    m_chargeTarget = m_target.transform.position;
                                }
                            }
                            else
                            {
                                if(Vector3.Distance(transform.position, m_chargeTarget) > m_chargeAimDist)
                                {
                                    //Lost Target
                                    EnterRecovery(0.5f, false);
                                }
                                else if(Vector3.Distance(transform.position, m_target.transform.position) <= m_chargeHitDist)
                                {
                                    //Hit player
                                    EnterRecovery(1.0f, false);
                                    m_target.GetComponent<Player_Controller>().StunPlayer(0.2f,  (m_target.transform.position - transform.position).normalized * m_chargeKnockback);
                                    m_target.GetComponent<Player_Controller>().ScreenShake(5.0f);
                                }
                                else
                                {
                                    SetTargetLocation(transform.position + transform.forward, true);
                                }
                            }
                        }
                        break;
                    };
                case ChargePhase.RECOVERY:
                    {
                        m_myBrain.m_legs.Halt();
                        if(m_recoveryCd <= 0)
                        {
                            TransitionToPhase(Phase.ATTACK);
                        }
                        break;
                    };
                default:
                    break;
            }

            
        }

        private void EnterRecovery(float delay, bool wasHit = true)
        {
            m_recoveryCd = delay;
            m_myBrain.m_animator.SetInteger("ChargeStatus", (wasHit) ? -1 : 1);
            m_myBrain.m_legs.Halt();
            m_cPhase = ChargePhase.RECOVERY;
            foreach (var item in m_myWalls)
            {
                item.SetEnabledStatus(true);
            }
        }

        public void HitWall()
        {
            if(m_phase == Phase.CHARGE)
            {
                EnterRecovery(1.5f, true);
            }
        }

        public override void DealImpactDamage(float amount, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {

        }

        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            return base.DealDamage(_damage, _type, piercingVal, null);
        }

        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            return base.DealDamageSilent(_damage, _type);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            if(m_phase == Phase.CHARGE && m_target != null)
            {
                if (!m_chargeControlled)
                {
                    Extentions.GizmosDrawCircle(m_chargeTarget, m_chargeAimDist);
                    Gizmos.DrawSphere(m_chargeTarget, 0.5f);
                }
                else
                {
                    Extentions.GizmosDrawCircle(m_target.transform.position, m_chargeAimDist);
                }

                Gizmos.color = Color.red;
                Extentions.GizmosDrawCircle(m_target.transform.position, m_chargeHitDist);
            }
        }

    }
}
