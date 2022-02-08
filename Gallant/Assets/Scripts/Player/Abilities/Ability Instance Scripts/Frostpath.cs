using Actor.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Frostpath : MonoBehaviour
{
    public AbilityData m_data;
    public MeshRenderer meshRenderer { get; private set; }
    [SerializeField] private GameObject m_prefabVFX;
    public List<GameObject> frostParticles { get; private set; } = new List<GameObject>();

    private bool m_beganLife = false;

    private float m_lifeTimer = 0.0f;

    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private float m_startAlpha = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //frostParticles = GetComponentInChildren<ParticleSystem>();

        m_startAlpha = meshRenderer.material.color.a;

        m_startPos = transform.position;
    }

    void FixedUpdate()
    {

        //Color color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b,
        //    Mathf.Clamp((m_lifeTimer < 1.0f) ? m_lifeTimer * 3.0f * m_startAlpha : (m_data.lifetime - m_lifeTimer) * m_startAlpha, 0.0f, m_startAlpha));

        //meshRenderer.material.color = color;

        if (m_beganLife)
        {
            m_lifeTimer += Time.fixedDeltaTime;
            if (m_lifeTimer > m_data.lifetime)
            {
                foreach (var vfx in frostParticles)
                {
                    //vfx.transform.SetParent(null);
                    vfx.GetComponent<VFXTimerScript>().m_startedTimer = true;
                }

                Destroy(gameObject);
                return;
            }
        }
    }

    public void StartLife()
    {
        m_beganLife = true;
        float count = 8.0f;
        for (int i = 0; i <= count; i++)
        {
            SpawnVFX(Vector3.Lerp(m_startPos, m_endPos, i / count));
        }
    }
    public void SetEdgePoint(Vector3 _pos)
    {
        m_endPos = _pos;
        transform.position = (m_startPos + _pos) / 2;

        Vector3 difference = _pos - m_startPos;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan(difference.x / difference.z) * Mathf.Rad2Deg, Vector3.up);
        transform.localScale = new Vector3(0.25f, 0.25f, difference.magnitude / 10.0f);
    }

    private void SpawnVFX(Vector3 _position)
    {
        GameObject gameObject = Instantiate(m_prefabVFX, _position, transform.rotation);
        frostParticles.Add(gameObject);

        VisualEffect vfx = gameObject.GetComponentInChildren<VisualEffect>();
        vfx.SetFloat("life time", m_data.lifetime);

        ParticleSystem particleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            ParticleSystem.MainModule mainModule = particleSystem.main;
            mainModule.duration = m_data.lifetime;
            particleSystem.Play();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Debug.Log("Slowing " + other.name + ".");

            Enemy actor = other.GetComponentInParent<Enemy>();
            if (actor != null)
            {

            }
            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                status.AddStatusEffect(new SlowStatus(m_data.effectiveness, m_data.duration));
            }
        }
    }

}
