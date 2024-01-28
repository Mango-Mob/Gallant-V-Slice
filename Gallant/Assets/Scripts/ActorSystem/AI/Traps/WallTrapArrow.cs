using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PlayerSystem;

namespace ActorSystem.AI.Traps
{
    public class WallTrapArrow : MonoBehaviour
    {
        public GameObject m_onHitVFX;
        public AudioClip m_onDestroyClip;

        private WallTrap m_parentHolder;
        private LayerMask m_damageLayers;
        public void Awake()
        {
            m_parentHolder = GetComponentInParent<WallTrap>();
            m_damageLayers = m_parentHolder.m_damageLayers;
        }

        private void OnTriggerEnter(Collider other)
        {
            bool spawnVFX = false;
            if(m_damageLayers == (m_damageLayers | (1 << other.gameObject.layer)))
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    spawnVFX = m_parentHolder.LogPlayer(other.gameObject.GetComponent<Player_Controller>());
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
                {
                    spawnVFX = m_parentHolder.LogActor(other.gameObject.GetComponent<Actor>());
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Destructible"))
                {
                    other.GetComponentInParent<Destructible>().ExplodeObject(transform.position, 500, 1000f);
                    spawnVFX = true;
                }
                else
                {
                    m_parentHolder.ResetTrap();
                    AudioManager.Instance.PlayAudioTemporary(transform.position, m_onDestroyClip, AudioManager.VolumeChannel.SOUND_EFFECT, 15f);
                    Instantiate(m_onHitVFX, transform.position, Quaternion.identity).transform.localScale = Vector3.one * 2f;
                }
            }

            if(spawnVFX)
                Instantiate(m_onHitVFX, transform.position, Quaternion.identity);
        }
    }
}
