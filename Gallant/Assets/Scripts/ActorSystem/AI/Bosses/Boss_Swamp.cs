using ActorSystem.AI.Other;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Bosses
{
    public class Boss_Swamp : Boss_Actor
    {
        public Tentacle_AI m_tentacleL;
        public Tentacle_AI m_tentacleR;

        public List<Tentacle_AI> m_otherTentacles;

        public enum Side { LEFT, RIGHT}
        private Side m_side;

        private bool m_visible = true;
        private List<Transform> m_leftPoints = new List<Transform>();
        private List<Transform> m_rightPoints = new List<Transform>();

        protected override void Awake()
        {
            base.Awake();
            foreach (var item in m_myBrain.m_patrol.m_targetOrientations)
            {
                if (item.rotation.eulerAngles.y == 0)
                {
                    m_leftPoints.Add(item);
                }
                if (item.rotation.eulerAngles.y == 180)
                {
                    m_rightPoints.Add(item);
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            SelectRandomOrientation();
            m_myBrain.m_patrol.transform.SetParent(null);
            m_myBrain.m_animator.SetBool("Visible", true);
            m_tentacleL.Emerge();
            m_tentacleR.Emerge();
        }

        protected override void Update()
        {
            base.Update();

            if (m_visible && InputManager.Instance.IsKeyDown(KeyType.H))
            {
                m_myBrain.m_animator.SetBool("Visible", false);
                m_tentacleL.Emerge();
                m_tentacleR.Emerge();
                m_visible = false;
            }
            if (!m_visible && InputManager.Instance.IsKeyDown(KeyType.Y))
            {
                SelectRandomOrientation();
                m_myBrain.m_animator.SetBool("Visible", true);
                m_tentacleL.Submerge();
                m_tentacleR.Submerge();
                m_visible = true;
            }
        }

        private void SelectRandomOrientation()
        {
            int select = Random.Range(0, m_myBrain.m_patrol.m_targetOrientations.Count);
            transform.position = m_myBrain.m_patrol.m_targetOrientations[select].position;
            transform.rotation = m_myBrain.m_patrol.m_targetOrientations[select].rotation;
        }
    }
}
