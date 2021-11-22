using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierProjectile : MonoBehaviour
{
    public float m_barrierValue;
    public float m_speed = 10.0f;
    private float m_lifeTimer = 0.0f;
    public AbilityData m_data;
    private float m_rotateRate = 180.0f;

    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject[] cubes;

    // Start is called before the first frame update
    void Start()
    {
        model.transform.localScale = new Vector3(1, 1, 1) * (0.5f + (m_barrierValue / 40.0f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cubes[0].transform.Rotate(new Vector3(m_rotateRate * Time.deltaTime, m_rotateRate * Time.deltaTime, m_rotateRate * Time.deltaTime));
        cubes[1].transform.Rotate(new Vector3(-m_rotateRate * Time.deltaTime, -m_rotateRate * Time.deltaTime, -m_rotateRate * Time.deltaTime));

        transform.position += transform.forward * m_speed * Time.fixedDeltaTime;
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            if (particles != null)
            {
                ParticleSystem[] particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
                foreach (var particles in particleSystems)
                {
                    particles.Stop();
                }

                particles.transform.SetParent(null);
                particles.GetComponent<VFXTimerScript>().m_startedTimer = true;
            }
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                actor.DealDamage(m_data.damage * m_barrierValue / 50.0f);
                Destroy(gameObject);
            }
        }
    }
}
