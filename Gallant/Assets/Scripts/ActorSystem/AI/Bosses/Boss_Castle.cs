using ActorSystem.AI.Other;
using ActorSystem.AI.Traps;
using System;
using System.Collections;
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

        [Header("Charge variables")]
        public float m_minDistForCharge = 5f;
        public float m_chargeAimDist = 5f;
        public float m_chargeHitDist = 2.5f;
        public float m_chargeKnockback = 20.0f;
        public float m_chargeDamage = 10f;
        public float m_wallStun = 3.5f;
        public bool m_chargeControlled = true;
        public AnimationCurve m_chargeMSpeedMod;
        public GameObject m_wallStunVFX;
        private float m_speedMod = 1.0f;
        public float m_chargeCooldown = 3.0f;

        [Header("Flee variables")]
        public float m_slamDamage = 10f;
        public float m_fleeHealthReq = 0.5f;
        public Transform m_fleeLocation;
        public float m_fleeTime = 45f;
        public bool m_hasFled = false;
        public bool m_isJumping = false;
        private bool m_spikesStarted = false;
        private bool m_hasJumpBack = false;
        private float m_cameraDelay = 0.0f;
        private bool m_cameraReturned = false;
        public GameObject m_buttSlamIndicator;
        public float m_fleeHeal = 1f;

        [Header("System")]
        public Phase m_phase;
        public ChargePhase m_cPhase;
        public CastleWallController[] m_myWalls;
        public SpikeTrapGroup m_spikeGroup;

        
        private float m_chargeCurrCd = 0.0f;
        private float m_recoveryCd = 0.0f;
        private float m_chargeTimer = 0.0f;
        private Vector3 m_chargeTarget;
        private Vector3 m_jumpStartLoc;
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
                    FleePhaseFunc();
                    break;
                case Phase.DEAD:
                    DeadPhaseFunc();
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
                        m_speedMod = 1.5f;
                        m_chargeCooldown *= 0.8f;
                        m_wallStun *= 0.8f;
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
                    m_hasFled = true;
                    m_isJumping = false;
                    m_spikesStarted = false;
                    m_hasJumpBack = false;
                    m_target.GetComponent<Player_Controller>().ChangeCameraFocus(transform, 1f, true);
                    StartCoroutine(JumpTo(m_fleeLocation.position, 1.0f, false));
                    m_cameraReturned = false;
                    m_cameraDelay = 1.0f;
                    break;
                case Phase.DEAD:
                    foreach (var item in m_myBrain.m_materials)
                    {
                        item.StartDisolve(2.5f);
                    }
                    RewardManager.giveRewardUponLoad = true;
                    GameManager.currentLevel++;
                    GameManager.m_saveInfo.m_completedCastle = 1;
                    break;
                default:
                    break;
            }
        }

        public void EndChargeAim()
        {
            m_cPhase = ChargePhase.RUNNING;
        }

        private void DeadPhaseFunc()
        {
            bool hasEnded = true;
            foreach (var item in m_myBrain.m_materials)
            {
                if(item.m_isDisolving != 0)
                {
                    hasEnded = false;
                }
            }

            if(hasEnded)
            {
                GameManager.Instance.FinishLevel();
            }
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

            if(!m_myBrain.m_arms.m_activeAttack.HasValue)
            {
                if (!m_hasFled && m_myBrain.m_currHealth <= m_myBrain.m_startHealth * m_fleeHealthReq)
                {
                    TransitionToPhase(Phase.FLEE);
                    return;
                }

                if (!m_myBrain.m_arms.m_activeAttack.HasValue && m_myBrain.m_arms.m_brainLag <= 0f && dist >= m_minDistForCharge && m_chargeCurrCd <= 0)
                {
                    TransitionToPhase(Phase.CHARGE);
                }
            }
        }

        private void FleePhaseFunc()
        {
            //Initial Phase
            if(Vector3.Distance(m_fleeLocation.position, transform.position) < 0.25f && !m_hasJumpBack)
            {
                if(!m_cameraReturned)
                {
                    m_cameraDelay -= Time.deltaTime;
                    if(m_cameraDelay <= 0)
                    {
                        m_cameraReturned = true;
                        m_target.GetComponent<Player_Controller>().ResetCameraFocus(0.5f);
                    } 
                }
                else if(m_cameraDelay <= 0)
                {
                    //In position
                    Vector3 forward = m_target.transform.position - transform.position;
                    forward.y = 0;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward.normalized, Vector3.up), 120);
                    m_myBrain.m_currHealth += m_fleeHeal * Time.deltaTime;
                    if (!m_spikesStarted)
                    {
                        m_spikesStarted = true;
                        m_spikeGroup.m_playingInSeconds = m_fleeTime;
                    }
                    else if (m_spikesStarted && m_spikeGroup.m_playingInSeconds <= 0 && m_spikeGroup.IsReady())
                    {
                        m_hasJumpBack = true;
                        NavMeshHit hit;

                        Vector3 randOnUnitSphere = UnityEngine.Random.onUnitSphere;
                        randOnUnitSphere.y = 0;

                        if (NavMesh.SamplePosition(m_target.transform.position + randOnUnitSphere.normalized * 3f, out hit, 3, ~0))
                        {
                            Instantiate(m_buttSlamIndicator, hit.position, Quaternion.identity).SetActive(true);
                            StartCoroutine(JumpTo(hit.position, 1.5f, true));
                        }
                    }
                }
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
                            m_myBrain.m_legs.m_baseSpeed = m_myData.baseSpeed * m_chargeMSpeedMod.Evaluate(m_chargeTimer) * m_speedMod;

                            if (m_chargeControlled)
                            {
                                Vector3 targetForward = (m_target.transform.position - transform.position).normalized;
                                Vector3 averageForward = (targetForward * 0.3f + transform.forward * 0.7f).normalized;
                                SetTargetLocation(transform.position + averageForward, true);
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
                                    m_myBrain.m_audioAgent.PlayAttack(1);
                                    m_target.GetComponent<Player_Controller>().DamagePlayer(m_chargeDamage, CombatSystem.DamageType.Physical, null, true);
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

                        if (m_wallStunVFX.activeInHierarchy)
                            m_myBrain.m_animator.Shake(0.025f * m_recoveryCd / m_wallStun);

                        if (m_recoveryCd <= 0)
                        {
                            m_wallStunVFX.SetActive(false);
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

            if(wasHit)
            {
                m_wallStunVFX.SetActive(true);
                m_myBrain.m_audioAgent.PlayAttack(2);
            }

            m_myBrain.m_animator.SetInteger("ChargeStatus", (wasHit) ? -1 : 1);
            m_myBrain.m_legs.Halt();
            m_cPhase = ChargePhase.RECOVERY;
            foreach (var item in m_myWalls)
            {
                item.SetEnabledStatus(true);
            }
        }

        public IEnumerator JumpTo(Vector3 location, float jumpDuration, bool enableLegsAfter = false)
        {
            //Lift off
            SetTargetOrientaion(m_fleeLocation.position);
            m_myBrain.m_animator.PlayAnimation("JumpStart");
            do
            {
                yield return new WaitForEndOfFrame();
            } while (!m_isJumping);

            float jumpTimer = 0;
            float timePerFrame = 1.0f/Mathf.Max(jumpDuration, 0.01f);
            m_myBrain.m_animator.SetBool("Landed", false);
            Vector3 initialPos = transform.position;

            m_myBrain.m_legs.SetEnabled(false);

            do
            {
                jumpTimer = Mathf.Clamp(jumpTimer + timePerFrame * Time.deltaTime, 0, 1);
                transform.position = MathParabola.Parabola(initialPos, location, 5f, jumpTimer);

                m_myBrain.m_animator.SetFloat("Landed", (enableLegsAfter) ? jumpTimer : -jumpTimer);

                yield return new WaitForEndOfFrame();
            } while (jumpTimer < 1.0f);

            m_myBrain.m_legs.SetEnabled(enableLegsAfter);
            m_myBrain.m_legs.m_brainDecay = 2f;
            m_myBrain.m_arms.SetBrainLag(2f, false);

            if (enableLegsAfter)
            {
                TransitionToPhase(Phase.ATTACK);
                m_myBrain.m_audioAgent.PlayAttack(2);
            }

            yield return null;
        }

        public void HitWall()
        {
            if(m_phase == Phase.CHARGE)
            {
                EnterRecovery(m_wallStun, true);
                float dist = Vector3.Distance(m_target.transform.position, transform.position);
                GameManager.Instance.m_player.GetComponent<Player_Controller>().ScreenShake(10 * (1.0f - dist / 15f), 0.3f);
            }
        }

        public override void DealImpactDamage(float amount, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {

        }

        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            if(base.DealDamage(_damage, _type, piercingVal, null))
            {
                TransitionToPhase(Phase.DEAD);
                return true;
            }
            return false;
        }

        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if(base.DealDamageSilent(_damage, _type))
            {
                TransitionToPhase(Phase.DEAD);
                return true;
            }

            return false;
        }

        public void StartJump()
        {
            m_isJumping = true;
            m_myBrain.m_legs.m_rotationDirection = 0.0f;
        }

        public void ButtSlam()
        {
            float dist = Vector3.Distance(m_target.transform.position, transform.position);
            if (dist <= 15.0f)
            {
                GameManager.Instance.m_player.GetComponent<Player_Controller>().ScreenShake(10 * (1.0f - dist/15f), 0.3f);
                if (dist <= 5.0f)
                {
                    m_target.GetComponent<Player_Controller>().DamagePlayer(m_slamDamage, CombatSystem.DamageType.Physical, this.gameObject, true);
                }
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            
            if(m_fleeLocation != null)
                Gizmos.DrawSphere(m_fleeLocation.position, 0.5f);
            
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
