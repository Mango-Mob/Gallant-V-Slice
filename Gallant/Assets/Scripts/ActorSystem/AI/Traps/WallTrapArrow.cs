using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class WallTrapArrow : MonoBehaviour
    {
        private WallTrap m_parentHolder;
        private LayerMask m_damageLayers;
        public void Awake()
        {
            m_parentHolder = GetComponentInParent<WallTrap>();
            m_damageLayers = m_parentHolder.m_damageLayers;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(m_damageLayers == (m_damageLayers | (1 << other.gameObject.layer)))
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    m_parentHolder.LogPlayer(other.gameObject.GetComponent<Player_Controller>());
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
                {
                    m_parentHolder.LogActor(other.gameObject.GetComponent<Actor>());
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Destructible"))
                {
                    other.GetComponent<Destructible>().ExplodeObject(transform.position, 5, 10f);
                }
                else
                {
                    m_parentHolder.ResetTrap();
                }
            }
        }
    }
}
