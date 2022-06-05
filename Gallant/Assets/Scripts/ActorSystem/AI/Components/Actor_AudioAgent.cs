using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    [RequireComponent(typeof(MultiAudioAgent))]    
    public class Actor_AudioAgent : Actor_Component
    {
        public enum SoundEffectType { Hurt, Death, Other}

        public MultiAudioAgent m_myAgent { get; protected set; }

        public List<AudioClip> m_attackClips;
        protected List<AudioClip> m_hurtClips;
        protected List<AudioClip> m_deathClips;
        
        public void Awake()
        {
            m_myAgent = GetComponent<MultiAudioAgent>();
        }

        public void Load(SoundEffectType sType,  AudioClip[] clips)
        {
            switch (sType)
            {
                case SoundEffectType.Hurt:
                    m_hurtClips = new List<AudioClip>(clips);
                    break;
                case SoundEffectType.Death:
                    m_deathClips = new List<AudioClip>(clips);
                    break;
                default:
                case SoundEffectType.Other:
                    return;
            }

        }

        public void Finalise()
        {
            foreach (var item in m_hurtClips)
            {
                if(!m_myAgent.audioClips.Contains(item))
                {
                    m_myAgent.audioClips.Add(item);
                }
            }
            foreach (var item in m_deathClips)
            {
                if (!m_myAgent.audioClips.Contains(item))
                {
                    m_myAgent.audioClips.Add(item);
                }
            }
            m_myAgent.audioClips.AddRange(m_attackClips);
            m_myAgent.UpdateList();
        }

        public void PlayAttack(int index)
        {
            m_myAgent.Play(m_attackClips[index].name, false, Random.Range(0.85f, 1.25f));
        }
        public void PlayAttackTemporarily(int index)
        {
            AudioManager.Instance.PlayAudioTemporary(transform.position, m_attackClips[index]);
        }

        public void PlayHurt()
        {
            if(m_hurtClips != null && m_hurtClips.Count > 0)
                m_myAgent.Play(m_hurtClips[Random.Range(0, m_hurtClips.Count)].name, false, Random.Range(0.85f, 1.25f));
        }

        public void PlayDeath()
        {
            if (m_deathClips != null && m_deathClips.Count > 0)
                m_myAgent.Play(m_deathClips[Random.Range(0, m_deathClips.Count)].name, false, Random.Range(0.85f, 1.25f));
        }

        public override void SetEnabled(bool status)
        {
            m_myAgent?.SetEnabled(status);
        }
    }
}
