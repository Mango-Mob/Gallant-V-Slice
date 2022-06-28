using ActorSystem.AI.Components;
using ActorSystem.AI.Other;
using ActorSystem.Spawning;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Bosses
{
    public class Boss_Swamp : Boss_Actor
    {
        public GameObject m_myModel;
        public int m_amountOfAttacks = 2;
        public float m_tentaclePhaseDuration = 5f;
        public float m_inkAttackDuration = 5f;
        public List<Tentacle_AI> m_currentlyAttacking { get; private set; } = new List<Tentacle_AI>();
        public List<GameObject> m_inkSplatter = new List<GameObject>();
        public GameObject m_inkBallVFX;
        public GameObject m_igniteVFX;

        public bool m_isLava = false;

        [Header("External Tentacles")]
        public Tentacle_AI m_tentacleL;
        public Tentacle_AI m_tentacleR;
        public Tentacle_AI m_tentacleO;
        public Tentacle_AI m_tentacleI;
        public List<Actor_Material> m_holdingTentacles;

        public List<Transform> m_targetLocR;
        public List<Transform> m_targetLocL;
        public List<Transform> m_targetLocO;
        public ActorSpawner m_mySpawnner;
        public WaveData m_waveToSpawn;
        private Transform m_restingTransformL;
        private Transform m_restingTransformR;

        public enum Phase { HEAD, TENTACLE, INK, SUMMON, DEAD}
        public Phase m_mode { get; private set; } = Phase.HEAD;

        public OctopusShipControls m_ship;

        public int m_currentOrient = 0;

        private bool m_visible = false; //Animation controlled variable if the boss is visible.

        private bool isCompletelySubmerged { get { return !m_tentacleL.isVisible && !m_tentacleR.isVisible && !m_tentacleO.isVisible && !m_visible; } }

        private float m_headphaseHealth;
        private Timer m_tentaclePhaseTimer;
        private Timer m_inkPhaseTimer;
        private Timer m_gameOverTimer;

        protected override void Awake()
        {
            base.Awake();
            m_restingTransformL = m_tentacleL.transform.parent;
            m_restingTransformR = m_tentacleR.transform.parent;
            
            foreach (var splatter in m_inkSplatter)
            {
                splatter.SetActive(false);
            }
        }

        protected override void Start()
        {
            base.Start();
            TransitionToPhase(Phase.HEAD, true);
            m_headphaseHealth = m_myBrain.m_startHealth * 0.2f;
            m_tentaclePhaseTimer = new Timer();
            m_tentaclePhaseTimer.onFinish.AddListener(TentacleTimeComplete);
            m_inkPhaseTimer = new Timer();
            m_inkPhaseTimer.onFinish.AddListener(AttackInk);

            m_gameOverTimer = new Timer();
            m_gameOverTimer.onFinish.AddListener(End);
        }

        public void Submerge()
        {
            m_myBrain.m_animator.SetBool("Visible", false);
            m_myBrain.m_myOutline.enabled = false;
        }

        public void Emerge()
        {
            m_myBrain.m_animator.SetBool("Visible", true);
            m_myBrain.m_myOutline.enabled = true;
        }

        public void SetVisible(bool status)
        {
            m_visible = status;
        }

        protected void TransitionToPhase(Phase phase, bool ignoreCheck = false)
        {
            if (m_mode == phase && !ignoreCheck)
                return;

            m_mode = phase;

            m_tentacleL.m_needsToUpdateAttacks = true;
            m_tentacleR.m_needsToUpdateAttacks = true;
            m_tentacleO.m_needsToUpdateAttacks = true;
            m_tentacleI.m_needsToUpdateAttacks = true;

            if (isCompletelySubmerged)
                SelectRandomOrientation();

            int select = m_currentOrient;
            int oppose = (select + 2) % (m_myBrain.m_patrol.m_targetOrientations.Count);
            Transform opposingTransform = m_myBrain.m_patrol.m_targetOrientations[oppose];

            switch (phase)
            {
                case Phase.HEAD:
                    Emerge();
                    m_myModel.transform.position = m_myBrain.m_patrol.m_targetOrientations[m_currentOrient].position;
                    m_myModel.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[m_currentOrient].rotation;
                    m_tentacleL.m_idealLocation = m_restingTransformL.position;
                    m_tentacleR.m_idealLocation = m_restingTransformR.position;
                    m_tentacleL.transform.localRotation = Quaternion.identity;
                    m_tentacleR.transform.localRotation = Quaternion.identity;
                    m_tentacleO.m_idealLocation = opposingTransform.position + opposingTransform.forward * 4.5f;
                    m_tentacleO.transform.rotation = opposingTransform.rotation;
                    m_amountOfAttacks = 2;
                    if (isCompletelySubmerged)
                    {
                        m_tentacleL.transform.localPosition = Vector3.zero;
                        m_tentacleR.transform.localPosition = Vector3.zero;
                        m_tentacleO.TeleportToIdeal();
                        m_tentacleL.Emerge();
                        m_tentacleR.Emerge();
                        m_tentacleO.Emerge();
                    }
                    break;
                case Phase.TENTACLE:
                    //Timer
                    m_tentaclePhaseTimer.Start(m_tentaclePhaseDuration);
                    m_amountOfAttacks = 1;
                    m_myModel.transform.position = m_myBrain.m_patrol.m_targetOrientations[m_currentOrient].position;
                    m_myModel.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[m_currentOrient].rotation;
                    switch (select)
                    {
                        case 0:
                        case 3:
                            m_tentacleL.m_idealLocation = m_targetLocL[0].position;
                            m_tentacleR.m_idealLocation = m_targetLocR[0].position;
                            m_tentacleO.m_idealLocation = m_targetLocO[0].position;
                            break;
                        case 1:
                        case 2:
                            m_tentacleL.m_idealLocation = m_targetLocL[1].position;
                            m_tentacleR.m_idealLocation = m_targetLocR[1].position;
                            m_tentacleO.m_idealLocation = m_targetLocO[1].position;
                            break;
                    }
                    m_tentacleO.transform.rotation = opposingTransform.rotation;

                    if(isCompletelySubmerged)
                    {
                        m_tentacleL.TeleportToIdeal();
                        m_tentacleR.TeleportToIdeal();
                        m_tentacleO.TeleportToIdeal();

                        m_tentacleL.Emerge();
                        m_tentacleR.Emerge();
                        m_tentacleO.Emerge();

                        m_tentacleL.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[(select == 3 || select == 0) ? 0 : 2].rotation;
                        m_tentacleR.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[(select == 3 || select == 0) ? 3 : 1].rotation;
                        m_tentacleO.transform.rotation = opposingTransform.rotation;
                    }
                    Submerge();
                    break;
                case Phase.INK:
                    m_tentacleL.Submerge(false);
                    m_tentacleR.Submerge(false);
                    m_tentacleO.Submerge(false);
                    StartCoroutine(CreateInk(9, 2f, 0.05f));
                    m_inkPhaseTimer.Start(m_inkAttackDuration);
                    break;
                case Phase.SUMMON:
                    m_mySpawnner.SpawnWave(m_waveToSpawn);
                    m_tentacleL.Submerge(false);
                    m_tentacleR.Submerge(false);
                    m_tentacleO.Submerge(false);
                    m_tentacleI.Submerge(false);
                    Submerge();
                    break;
                case Phase.DEAD:
                    Submerge();
                    foreach (var material in m_myBrain.m_materials)
                    {
                        material.StartDisolve();
                    }

                    m_myBrain.m_animator.SetFloat("playSpeed", 0.25f);
                    m_tentacleL.Kill(); m_tentacleL.Submerge(false);
                    m_tentacleR.Kill(); m_tentacleR.Submerge(false);
                    m_tentacleO.Kill(); m_tentacleO.Submerge(false);
                    m_tentacleI.Kill(); m_tentacleI.Submerge(false);

                    m_myBrain.DropOrbs(Random.Range(5, 10), GameManager.Instance.m_player.transform.position, Random.Range(m_myData.adrenalineGainMin, m_myData.adrenalineGainMax));

                    foreach (var hold in m_holdingTentacles)
                    {
                        hold.StartDisolve();
                    }

                    if (m_isLava)
                        GameManager.m_saveInfo.m_completedMagma = 1;
                    else
                        GameManager.m_saveInfo.m_completedSwamp = 1;

                    RewardManager.giveRewardUponLoad = true;
                    
                    GameManager.currentLevel++;
                    m_gameOverTimer.Start(6f);
                    break;
                default:
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();
            if (m_myBrain.IsDead)
            {
                TransitionToPhase(Phase.DEAD);
                m_gameOverTimer.Update();
                return;
            }

            m_tentaclePhaseTimer.Update();
            m_inkPhaseTimer.Update();

            if(this.m_mode == Phase.SUMMON && !m_mySpawnner.isSpawnning && m_mySpawnner.m_myActors.Count == 0)
            {
                TransitionToPhase(Phase.HEAD);
            }
            if (m_tentacleL.m_myBrain.IsDead && m_tentacleR.m_myBrain.IsDead && m_tentacleO.m_myBrain.IsDead)
            {
                if(this.m_mode == Phase.HEAD)
                {
                    StartCoroutine(ChangeOrientation(m_mode, 1f));
                }
                else
                {
                    m_headphaseHealth = m_myBrain.m_startHealth * 0.2f;
                    m_tentaclePhaseTimer.Stop();
                    StartCoroutine(ChangeOrientation(Phase.HEAD, 1f));
                }
                
            }
        }

        private IEnumerator ChangeOrientation(Phase nextPhase, float delay = 0f)
        {
            Submerge();
            m_tentacleL.Submerge(true);
            m_tentacleR.Submerge(true);
            m_tentacleO.Submerge(true);

            while (!isCompletelySubmerged)
            {
                yield return new WaitForEndOfFrame();
            }

            float timer = 0f;
            while(timer < delay)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            TransitionToPhase(nextPhase, true);

            yield return null;
        }

        private void TentacleTimeComplete()
        {
            m_headphaseHealth = m_myBrain.m_startHealth * 0.2f;
            m_tentaclePhaseTimer.Stop();
            TransitionToPhase(Phase.HEAD);
        }
        private void AttackInk()
        {
            m_tentacleI.m_myBrain.m_animator.PlayAnimation("InkAttack");
        }
        public void IgniteInk()
        {
            foreach (var item in m_inkSplatter)
            {
                if(item.activeInHierarchy)
                {
                    Instantiate(m_igniteVFX, item.transform.position, item.transform.rotation);
                    item.SetActive(false);
                }
            }
        }

        private void SelectRandomOrientation()
        {
            m_currentOrient = Random.Range(0, m_myBrain.m_patrol.m_targetOrientations.Count);
        }

        public override void DealImpactDamage(float amount, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {

        }

        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            if (!m_myBrain.IsDead)
            {
                m_myBrain.ShowHit();
                if (m_myBrain.HandleDamage(_damage, piercingVal, _type, _damageLoc))
                {
                    if (m_HurtVFXPrefab != null)
                        Instantiate(m_HurtVFXPrefab, m_selfTargetTransform.position, Quaternion.identity);

                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    return true;
                }
                else
                {
                    if (m_mode == Phase.HEAD)
                    {
                        m_headphaseHealth -= _damage;
                        if(m_headphaseHealth <= 0)
                        {
                            if(m_myBrain.m_currHealth/m_myBrain.m_startHealth < 0.65f && Random.Range(0, 1000) < (1.0f - m_myBrain.m_currHealth / m_myBrain.m_startHealth) * 1000)
                            {
                                if(Random.Range(0, 10000) < 10000 * 0.5)
                                {
                                    TransitionToPhase(Phase.INK);
                                }
                                else
                                {
                                    StartCoroutine(ChangeOrientation(Phase.SUMMON, 1f));
                                }
                            }
                            else
                            {
                                TransitionToPhase(Phase.TENTACLE);
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if (!m_myBrain.IsDead)
            {
                if (m_myBrain.HandleDamage(_damage, 0, _type, transform.position, false, false))
                {
                    foreach (var collider in GetComponentsInChildren<Collider>())
                    {
                        collider.enabled = false;
                    }
                    m_myBrain.m_animator.SetFloat("playSpeed", 0.25f);
                    foreach (var material in m_myBrain.m_materials)
                    {
                        material.StartDisolve(5f);
                    }
                    Submerge();
                    return true;
                }

                if (m_mode == Phase.HEAD)
                {
                    m_headphaseHealth -= _damage;
                    if (m_headphaseHealth <= 0)
                    {
                        TransitionToPhase(Phase.TENTACLE);
                    }
                }
            }
            return false;
        }

        private IEnumerator CreateInk(int subtract, float duration, float delay)
        {
            Submerge();

            while(m_visible)
            {
                yield return new WaitForEndOfFrame();
            }

            List<GameObject> inkList = new List<GameObject>(m_inkSplatter);
            for (int i = 0; i < subtract; i++)
            {
                int select = Random.Range(0, inkList.Count);
                inkList.RemoveAt(select);
            }
            foreach (var item in inkList)
            {
                StartCoroutine(ShootInkBall(item, duration));
                yield return new WaitForSeconds(delay);
            }
            TransitionToPhase(Phase.TENTACLE);
            yield return null;
        }

        private IEnumerator ShootInkBall(GameObject target, float duration)
        {
            float timer = 0;
            float height = 5;
            Vector3 start = m_myModel.transform.position;
            Vector3 end = target.transform.position;
            GameObject ball = Instantiate(m_inkBallVFX, start, Quaternion.identity);

            while(timer < duration)
            {
                ball.transform.position = MathParabola.Parabola(start, end, height, timer/duration);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            Destroy(ball);
            target.SetActive(true);
            yield return null;
        }

        private void End()
        {
            GameManager.Instance.FinishLevel(m_isLava);
        }

        public void Slam(int tentacleId)
        {
            if (tentacleId == 3)
            {
                //Lantern
                m_ship.Slam(true);
            }
            else if (m_currentOrient == 0 || m_currentOrient == 3)
            {
                //Oct at the back
                m_ship.Slam(tentacleId == 2); //id = 2 is the opposite side
            }
            else
            {
                //Oct at the front
                m_ship.Slam(tentacleId != 2);
            }

            GameManager.Instance.m_player.GetComponent<Player_Controller>().ScreenShake(8, 0.3f);
        }
    }
}
