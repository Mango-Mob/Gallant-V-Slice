using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Destructible : MonoBehaviour
{
    public GameObject crackedObject;
    public AudioClip m_soundClip;
    [Range(0.0f, 1.0f)] public float m_soundLocalVolume = 1.0f;
    public bool m_letPlayerDestroy = true;
    public bool m_letRollDestroy = true;

    public void CrackObject()
    {
        GameObject newObject = Instantiate(crackedObject, transform.position, transform.rotation);
        newObject.transform.localScale = transform.localScale;

        if (m_soundClip != null)
            AudioManager.Instance.PlayAudioTemporary(transform.position, m_soundClip);

        Destroy(gameObject);
    }
    public void ExplodeObject(Vector3 forceLoc, float forceVal, float maxDist, bool isPlayer = true)
    {
        if (isPlayer && !m_letPlayerDestroy)
            return;

        Destruction destructObject = Instantiate(crackedObject, transform.position, transform.rotation).GetComponent<Destruction>();
        destructObject.transform.localScale = transform.localScale;
        destructObject.ApplyExplosionForce(forceLoc, forceVal, maxDist);

        if (m_soundClip != null)
            AudioManager.Instance.PlayAudioTemporary(transform.position, m_soundClip, AudioManager.VolumeChannel.SOUND_EFFECT, -1.0f, m_soundLocalVolume);

        Destroy(gameObject);
    }

}
