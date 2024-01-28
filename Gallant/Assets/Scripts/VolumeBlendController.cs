using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class VolumeBlendController : MonoBehaviour
{
    private Volume m_volume;
    public float m_fadeSpeed = 1.0f;
    public bool m_volumeEnabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_volume = GetComponent<Volume>();
        m_volume.weight = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_volume.weight = Mathf.Clamp01(m_volume.weight + m_fadeSpeed * Time.deltaTime * (m_volumeEnabled ? 1.0f : -1.0f));
    }
}
