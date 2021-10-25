using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostpath : MonoBehaviour
{

    public AbilityData m_data;
    public MeshRenderer meshRenderer { get; private set; }
    public ParticleSystem frostParticles { get; private set; }

    private bool m_beganLife = false;

    public float m_lifeTime = 1.0f;
    private float m_lifeTimer = 0.0f;

    private Vector3 m_startPos;
    private float m_startAlpha = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        frostParticles = GetComponentInChildren<ParticleSystem>();

        m_startAlpha = meshRenderer.material.color.a;

        m_startPos = transform.position;
    }

    void FixedUpdate()
    {
        if (m_beganLife)
        {
            m_lifeTimer += Time.fixedDeltaTime;
            if (m_lifeTimer > m_lifeTime)
            {
                frostParticles.Stop();
                frostParticles.transform.SetParent(null);
                frostParticles.GetComponent<VFXTimerScript>().m_startedTimer = true;
                Destroy(gameObject);
            }
        }

        Color color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 
            Mathf.Clamp((m_lifeTimer < 1.0f) ? m_lifeTimer * 3.0f * m_startAlpha : (m_lifeTime - m_lifeTimer) * m_startAlpha, 0.0f, m_startAlpha));

        meshRenderer.material.color = color;
    }

    public void StartLife()
    {
        m_beganLife = true;
    }
    public void SetEdgePoint(Vector3 _pos)
    {
        transform.position = (m_startPos + _pos) / 2;

        Vector3 difference = _pos - m_startPos;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan(difference.x / difference.z) * Mathf.Rad2Deg, Vector3.up);
        transform.localScale = new Vector3(0.25f, 0.25f, difference.magnitude / 10.0f);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Debug.Log("Slowing " + other.name + ".");

            Actor actor = other.GetComponent<Actor>();
            if (actor != null)
            {
            }
        }
    }

}
