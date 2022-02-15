using ActorSystem.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnActorObject : MonoBehaviour
{
    public GameObject m_ObjectToSpawn;
    public Vector3 m_start;
    public Vector3 m_end;
    public float m_height;
    public float m_time;
    public float m_spawnCost;

    private float m_deltaTime;
    private float m_deltaDeltaTime;
    private bool m_hasReachedPeak;

    private GameObject m_presetTarget = null;
    private Collider m_triggerBox = null;
    private ActorSpawner m_owner = null;

    public AudioClip m_spawnSound;
    // Start is called before the first frame update
    void Start()
    {
        m_deltaTime = 0.020f;
        m_deltaDeltaTime = 0.0005f;
        m_triggerBox = GetComponent<Collider>();
        m_triggerBox.enabled = false;

        AudioManager.instance.PlayAudioTemporary(transform.position, m_spawnSound);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = MathParabola.Parabola(m_start, m_end, m_height, m_time);

        m_triggerBox.enabled = m_time > 0.5;

        if (!m_hasReachedPeak)
        {
            m_time += m_deltaTime;
            m_deltaTime = Mathf.Clamp(m_deltaTime - m_deltaDeltaTime, 0.005f, 1.0f);
            if (transform.position.y > m_height)
            {
                m_hasReachedPeak = true;
            }
        }
        else
        {
            m_time += m_deltaTime;
            m_deltaTime += m_deltaDeltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Default") && m_ObjectToSpawn != null)
        {
            GameObject Actor = GameObject.Instantiate(m_ObjectToSpawn, gameObject.transform.position, gameObject.transform.rotation);
            //Actor.GetComponent<Actor>().m_target = m_presetTarget;
            m_owner.AddActor(Actor.GetComponent<Actor>());
            m_ObjectToSpawn = null;
            Destroy(gameObject);
        }
    }
    public void PresetTarget(GameObject target)
    {
        m_presetTarget = target;
    }

    public void SetOwner(ActorSpawner ActorSpawner)
    {
        m_owner = ActorSpawner;
    }
}
