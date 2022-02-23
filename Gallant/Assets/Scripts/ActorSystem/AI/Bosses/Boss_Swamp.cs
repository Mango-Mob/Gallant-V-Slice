using UnityEngine;

namespace ActorSystem.AI.Bosses
{
    public class Boss_Swamp : Boss_Actor
    {
        private bool m_visible = true;
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            m_myBrain.m_patrol.transform.SetParent(null);
            m_myBrain.m_animator.SetBool("Visible", true);
            SelectRandomOrientation();
        }
        protected override void Update()
        {
            base.Update();

            if (m_visible && InputManager.Instance.IsKeyDown(KeyType.H))
            {
                m_myBrain.m_animator.SetBool("Visible", false);
                m_visible = false;
            }
            if (!m_visible && InputManager.Instance.IsKeyDown(KeyType.Y))
            {
                SelectRandomOrientation();
                m_myBrain.m_animator.SetBool("Visible", true);
                m_visible = true;
            }
        }
        private void SelectRandomOrientation()
        {
            int selected = Random.Range(0, m_myBrain.m_patrol.m_targetOrientations.Count);
            Transform select = m_myBrain.m_patrol.m_targetOrientations[selected];
            transform.position = select.position;
            transform.rotation = select.rotation;
            
        }
    }
}
