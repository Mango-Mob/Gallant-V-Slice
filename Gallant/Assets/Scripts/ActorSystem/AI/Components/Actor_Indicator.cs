using UnityEngine;

namespace ActorSystem.AI.Components
{
    public class Actor_Indicator : Actor_Component
    {
        public float m_speed { get { return GetComponent<Animator>().speed; } set { GetComponent<Animator>().speed = value; } }

        public void Start()
        {
            m_speed = 1.0f;
        }

        public override void SetEnabled(bool status)
        {
            this.gameObject.SetActive(status);
            m_speed = 1.0f;

        }
    }
}
