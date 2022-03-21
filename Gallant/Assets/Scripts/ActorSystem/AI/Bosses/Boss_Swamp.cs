using ActorSystem.AI.Other;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Bosses
{
    public class Boss_Swamp : Boss_Actor
    {
        public GameObject m_myModel;

        [Header("External Tentacles")]
        public Tentacle_AI m_tentacleL;
        public Tentacle_AI m_tentacleR;
        public Tentacle_AI m_tentacleO;
        public Tentacle_AI m_tentacleI;
        public List<Tentacle_AI> m_holdingTentacles;

        private Transform m_restingTransformL;
        private Transform m_restingTransformR;

        public enum Phase { HEAD, TENTACLE, INK}
        private Phase m_mode = Phase.HEAD;

        private int m_currentOrient = 0;

        private bool m_visible = false; //Animation controlled variable if the boss is visible.

        private bool isCompletelySubmerged { get { return !m_tentacleL.isVisible && !m_tentacleR.isVisible && !m_tentacleO.isVisible && !m_visible; } }

        protected override void Awake()
        {
            base.Awake();
            m_restingTransformL = m_tentacleL.transform.parent;
            m_restingTransformR = m_tentacleR.transform.parent;
        }

        protected override void Start()
        {
            base.Start();
            TransitionToPhase(Phase.HEAD, true);
        }

        public void Submerge()
        {
            m_myBrain.m_animator.SetBool("Visible", false);
        }

        public void Emerge()
        {
            m_myBrain.m_animator.SetBool("Visible", true);
            m_myModel.transform.position = m_myBrain.m_patrol.m_targetOrientations[m_currentOrient].position;
            m_myModel.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[m_currentOrient].rotation;
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

            if (isCompletelySubmerged)
                SelectRandomOrientation();

            int select = m_currentOrient;
            int oppose = (select + 2) % (m_myBrain.m_patrol.m_targetOrientations.Count);
            Transform opposingTransform = m_myBrain.m_patrol.m_targetOrientations[oppose];

            switch (phase)
            {
                case Phase.HEAD:
                    Emerge();

                    m_tentacleL.m_idealLocation = m_restingTransformL.position;
                    m_tentacleR.m_idealLocation = m_restingTransformR.position;

                    m_tentacleO.m_idealLocation = opposingTransform.position + opposingTransform.forward * 4.5f;
                    m_tentacleO.transform.rotation = opposingTransform.rotation;

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
                    switch (select)
                    {
                        case 0:
                            m_tentacleL.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[0].position + m_myModel.transform.forward * 4.5f;
                            m_tentacleL.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[0].rotation;
                            m_tentacleR.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[3].position + m_myModel.transform.forward * 4.5f;
                            m_tentacleR.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[3].rotation;
                            break;
                        case 1:
                            m_tentacleL.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[2].position - m_myModel.transform.forward * 4.5f;
                            m_tentacleL.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[2].rotation;
                            m_tentacleR.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[1].position - m_myModel.transform.forward * 4.5f;
                            m_tentacleR.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[1].rotation;
                            break;
                        case 2:
                            m_tentacleL.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[2].position - m_myModel.transform.forward * 4.5f;
                            m_tentacleL.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[2].rotation;
                            m_tentacleR.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[1].position - m_myModel.transform.forward * 4.5f;
                            m_tentacleR.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[1].rotation;
                            break;
                        case 3:
                            m_tentacleL.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[0].position + m_myModel.transform.forward * 4.5f;
                            m_tentacleL.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[0].rotation;
                            m_tentacleR.m_idealLocation = m_myBrain.m_patrol.m_targetOrientations[3].position + m_myModel.transform.forward * 4.5f;
                            m_tentacleR.transform.rotation = m_myBrain.m_patrol.m_targetOrientations[3].rotation;
                            break;
                    }
                    m_tentacleO.transform.position = opposingTransform.position + opposingTransform.forward * 4.5f;
                    m_tentacleO.transform.rotation = opposingTransform.rotation;

                    Vector3 midPoint = Extentions.MidPoint(m_tentacleR.m_idealLocation, m_tentacleL.m_idealLocation);
                    m_tentacleO.m_idealLocation = new Vector3(midPoint.x, m_tentacleO.transform.position.y, m_tentacleO.transform.position.z);

                    if(isCompletelySubmerged)
                    {
                        m_tentacleL.TeleportToIdeal();
                        m_tentacleR.TeleportToIdeal();
                        m_tentacleO.TeleportToIdeal();

                        m_tentacleL.Emerge();
                        m_tentacleR.Emerge();
                        m_tentacleO.Emerge();
                    }
                    Submerge();
                    break;
                case Phase.INK:
                    break;
                default:
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (m_mode == Phase.HEAD && InputManager.Instance.IsKeyDown(KeyType.H))
            {
                TransitionToPhase(Phase.TENTACLE);
            }
            else if (m_mode == Phase.TENTACLE && InputManager.Instance.IsKeyDown(KeyType.H))
            {
                TransitionToPhase(Phase.HEAD);
            }
            if (InputManager.Instance.IsKeyDown(KeyType.Y))
            {
                StartCoroutine(ChangeOrientation(m_mode));
            }
        }

        private IEnumerator ChangeOrientation(Phase nextPhase, float delay = 0f)
        {
            Submerge();
            m_tentacleL.Submerge();
            m_tentacleR.Submerge();
            m_tentacleO.Submerge();

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

        private void SelectRandomOrientation()
        {
            m_currentOrient = Random.Range(0, m_myBrain.m_patrol.m_targetOrientations.Count);
        }
    }
}
