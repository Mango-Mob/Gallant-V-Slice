using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class BaseAbilityPath : MonoBehaviour
{
    public AbilityData m_data;
    public MeshRenderer meshRenderer { get; private set; }
    [SerializeField] private GameObject m_prefabVFX;
    public List<GameObject> particles { get; private set; } = new List<GameObject>();

    private bool m_beganLife = false;

    private float m_lifeTimer = 0.0f;

    private bool m_hasStartPosition = false;
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
                foreach (var vfx in particles)
                {
                    //vfx.transform.SetParent(null);
                    vfx.GetComponent<VFXTimerScript>().m_startedTimer = true;
                }
                ActorManager.Instance.RemoveObstacle(transform);
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
        ActorManager.Instance?.AddObstacle(transform);
    }
    public void SetEdgePoint(Vector3 _pos)
    {
        if (!m_hasStartPosition)
        {
            m_startPos = _pos;
            m_hasStartPosition = true;
        }

        m_endPos = _pos;
        transform.position = (m_startPos + _pos) / 2;

        Vector3 difference = _pos - m_startPos;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan(difference.x / difference.z) * Mathf.Rad2Deg, Vector3.up);
        transform.localScale = new Vector3(0.25f, 0.25f, difference.magnitude / 10.0f);
    }

    private void SpawnVFX(Vector3 _position)
    {
        GameObject gameObject = Instantiate(m_prefabVFX, _position, transform.rotation);
        particles.Add(gameObject);

        VisualEffect vfx = gameObject.GetComponentInChildren<VisualEffect>();
        vfx.SetFloat("life time", m_data.lifetime);

        ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particleSystems)
        {
            particle.Stop();
            ParticleSystem.MainModule mainModule = particle.main;
            mainModule.duration = m_data.lifetime;
            particle.Play();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {

            }
            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                AddStatusEffect(status);
            }
        }
    }
    protected abstract void AddStatusEffect(StatusEffectContainer _container);
}
