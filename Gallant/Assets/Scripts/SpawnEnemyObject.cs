using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyObject : MonoBehaviour
{
    public GameObject m_ObjectToSpawn;
    public Vector3 m_start;
    public Vector3 m_end;
    public float m_height;
    public float m_time;

    private float m_deltaTime;
    private float m_deltaDeltaTime;
    private bool m_hasReachedPeak;

    private GameObject m_presetTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        m_deltaTime = 0.020f;
        m_deltaDeltaTime = 0.0005f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = MathParabola.Parabola(m_start, m_end, m_height, m_time);
        

        if(!m_hasReachedPeak)
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
        if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            GameObject enemy = GameObject.Instantiate(m_ObjectToSpawn, gameObject.transform.position, gameObject.transform.rotation);
            enemy.GetComponent<Actor>().m_target = m_presetTarget;
            Destroy(gameObject);
        }
    }
    public void PresetTarget(GameObject target)
    {
        m_presetTarget = target;
    }
}
