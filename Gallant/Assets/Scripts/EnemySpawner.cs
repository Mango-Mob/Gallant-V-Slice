using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public int m_spawnCount;
    public float m_spawnWidth = 0.25f;
    public float m_spawnArcHeight = 5.0f;
    public float m_distOffEdge = 1.0f;
    public float m_spawnDelay = 1.0f;

    public GameObject m_EnemyToSpawn;
    public AtmosphereScript m_music;

    private float m_timer;
    private Player_Controller m_player = null;
    private BoxCollider m_roomBCollider;
    public GameObject[] m_gates;

    public bool m_isSphere = true;
    public float m_radius = 10.0f;
    public Vector3 m_size = new Vector3(1f, 1f, 1f);

    public RewardWindow m_reward;

    private struct SpawnLocation
    {
        public Vector3 m_start;
        public Vector3 m_end;

        public Vector3 m_forward;
    }


    private List<SpawnLocation> m_spawnLocations = new List<SpawnLocation>();

    private void Awake()
    {
        m_music = FindObjectOfType<AtmosphereScript>();
        m_roomBCollider = GetComponent<BoxCollider>();

        m_timer = m_spawnDelay;
        foreach (var gate in m_gates)
        {
            gate.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_spawnCount; i++)
        {
            if (m_roomBCollider != null)
                GenerateASpawnPointInBox(m_size);
            else
                GenerateASpawnPointInSphere(m_radius);
        }
    }

    private void GenerateASpawnPointInBox(Vector3 size)
    {
        SpawnLocation generated = new SpawnLocation();
        int safety = 0;
        bool failed = false;

        do
        {
            failed = false;
            if (safety > 5) //If failed 5 times;
                break;
            //Generate end location
            
            generated.m_end = transform.position + new Vector3(0.5f * Random.Range(-size.x, size.x), 0, 0.5f * Random.Range(-size.z, size.z));

            //Generate start and forward
            generated.m_forward = (generated.m_end - transform.position).normalized;
            Vector3 edgePoint = transform.position + new Vector3(
                0.5f * Mathf.Clamp(size.x * generated.m_forward.x, -size.x, size.x), 
                0, 
                0.5f * Mathf.Clamp(size.z * generated.m_forward.z, -size.z, size.z)
                );

            generated.m_end = edgePoint;

            RaycastHit rhit;
            if (Physics.Raycast(edgePoint + generated.m_forward * m_distOffEdge, Vector3.down * 15.0f, out rhit, 1 << LayerMask.NameToLayer("Water")))
            {
                generated.m_start = rhit.point;
            }
            else
            {
                failed = true;
            }
            safety++;

        } while (!IsValidPoint(generated.m_end) || failed);

        if(safety < 5)
        {
            //Successful
            m_spawnLocations.Add(generated);
        }
    }

    private void GenerateASpawnPointInSphere(float radius)
    {
        SpawnLocation generated = new SpawnLocation();
        int safety = 0;
        bool failed = false;

        do
        {
            failed = false;
            if (safety > 5) //If failed 5 times;
                break;

            Vector2 randPoint = Random.insideUnitCircle * radius;
            generated.m_end = transform.position + new Vector3(randPoint.x, 0, randPoint.y);

            //Generate start and forward
            generated.m_forward = (generated.m_end - transform.position).normalized;
            Vector3 edgePoint = transform.position + (generated.m_forward * radius);
           
            RaycastHit rhit;
            if (Physics.Raycast(edgePoint + generated.m_forward * m_distOffEdge, Vector3.down * 15.0f, out rhit, 1 << LayerMask.NameToLayer("Water")))
            {
                generated.m_start = rhit.point;
            }
            else
            {
                failed = true;
            }
            safety++;


        } while (!IsValidPoint(generated.m_end) || failed);

        if (safety < 5)
        {
            //Successful
            m_spawnLocations.Add(generated);
        }
    }

    private bool IsValidPoint(Vector3 testPoint)
    {
        foreach (var item in m_spawnLocations)
        {
            if(Vector3.Distance(item.m_end, testPoint) <= (m_spawnWidth + m_spawnWidth)/2f)
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_timer < m_spawnDelay)
        {
            m_timer += Time.deltaTime;
        }
        else if(m_player != null && m_spawnLocations.Count > 0)
        {
            StartCombat();

            int selected = Random.Range(0, m_spawnLocations.Count);
            Quaternion rotation = Quaternion.LookRotation(-m_spawnLocations[selected].m_forward, Vector3.up);
            SpawnEnemyObject spawn = GameObject.Instantiate(m_EnemyToSpawn, m_spawnLocations[selected].m_start, rotation).GetComponent<SpawnEnemyObject>();
            spawn.m_start = m_spawnLocations[selected].m_start;
            spawn.m_end = m_spawnLocations[selected].m_end;
            spawn.m_height = m_spawnArcHeight;

            m_spawnLocations.RemoveAt(selected);
            m_timer = 0;
        }

        if( m_spawnLocations.Count == 0)
        {
            Collider[] enemies;
            if (m_isSphere)
            {
                 enemies = Physics.OverlapSphere(transform.position, m_radius, 1 << LayerMask.NameToLayer("Attackable"));
            }
            else
            {
                enemies = Physics.OverlapBox(transform.position, m_size/2f, Quaternion.identity, 1 << LayerMask.NameToLayer("Attackable"));
            }

            if (enemies.Length == 0)
            {
                foreach (var gate in m_gates)
                {
                    gate.GetComponent<Animator>().SetBool("Open", true);
                }
                m_reward.Show(true);
                m_music.EndCombat();
                Destroy(this);
            }
        }
    }

    public void StartCombat()
    {
        if (m_gates[0].activeInHierarchy)
            return;
        m_music.StartCombat();
        foreach (var gate in m_gates)
        {
            gate.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            m_player = other.GetComponent<Player_Controller>();
        }
        else if(other.GetComponent<SpawnEnemyObject>() != null)
        {
            other.GetComponent<SpawnEnemyObject>().PresetTarget(m_player?.gameObject);
        }
    }
    public void OnDrawGizmos()
    {
        if (m_isSphere)
            Gizmos.DrawWireSphere(transform.position, m_radius);
        else
            Gizmos.DrawWireCube(transform.position, m_size);

        foreach (var item in m_spawnLocations)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(item.m_end, m_spawnWidth);
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(item.m_start, m_spawnWidth);
            Gizmos.DrawLine(item.m_start, item.m_end);
        }
    }
}
