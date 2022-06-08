using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class WallTrap : MonoBehaviour
    {
        public float m_damage = 10f;

        public LayerMask m_detectLayers;
        public LayerMask m_damageLayers;
        public bool hasDetected = false;
        public bool hasHitPlayer = false;

        private List<Actor> m_hitActors = new List<Actor>();

        private void OnTriggerStay(Collider other)
        {
            if(!hasDetected && m_detectLayers == (m_detectLayers | (1 << other.gameObject.layer)))
            {
                hasDetected = true;
                GetComponent<Animator>().SetTrigger("Shoot");
            }
        }
        public void LogPlayer(Player_Controller player)
        {
            if(!hasHitPlayer)
            {
                hasHitPlayer = true;
            }
        }

        public void LogActor(Actor actor)
        {
            if (!m_hitActors.Contains(actor))
            {
                m_hitActors.Add(actor);
            }
        }

        public void ResetTrap()
        {
            hasHitPlayer = false;
            hasDetected = false;
            m_hitActors.Clear();
            GetComponent<Animator>().SetTrigger("Reset");
        }
    }
}
