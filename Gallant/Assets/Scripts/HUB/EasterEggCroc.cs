using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class EasterEggCroc : MonoBehaviour
{
    public Animator m_crocAnimator;
    public Collider[] m_crocColliders;
    private bool m_activated = false;
    public AudioClip m_chompSound;
    public AudioClip m_splashSound;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Controller>() && !m_activated)
        {
            m_crocAnimator.SetTrigger("Activate");
            m_activated = true;

            foreach (var collider in m_crocColliders)
            {
                collider.enabled = false;
            }
        }
    }

    public void PlayChompSound()
    {
        AudioManager.Instance.PlayAudioTemporary(transform.position, m_chompSound, AudioManager.VolumeChannel.SOUND_EFFECT);
    }
    public void PlaySplashSound()
    {
        AudioManager.Instance.PlayAudioTemporary(transform.position, m_splashSound, AudioManager.VolumeChannel.SOUND_EFFECT);
    }
}
