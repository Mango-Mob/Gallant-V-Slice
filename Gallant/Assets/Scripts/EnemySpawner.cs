using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public float m_spawnWidth = 0.25f;
    public float m_spawnArcHeight = 5.0f;
    public float m_distOffEdge = 1.0f;
    public float m_spawnDelay = 1.0f;

    public GameObject m_EnemyToSpawn;
    public AtmosphereScript m_music;

    private Player_Controller m_player = null;
    private BoxCollider m_roomBCollider;
    public GameObject[] m_gates;

    public bool m_isSphere = true;
    public float m_radius = 10.0f;
    public Vector3 m_size = new Vector3(1f, 1f, 1f);

    public RewardWindow m_reward;


    public int m_spawnSpots = 12; 

    public List<RoomData> m_waves = new List<RoomData>();
    private struct SpawnLocation
    {
        public Vector3 m_start;
        public Vector3 m_end;

        public Vector3 m_forward;
    }

    private Coroutine m_isSpawning = null;

    private List<SpawnLocation> m_spawnLocations = new List<SpawnLocation>();
    private List<Actor> m_enemies = new List<Actor>();

    private void Awake()
    {
        m_music = FindObjectOfType<AtmosphereScript>();
        m_roomBCollider = GetComponent<BoxCollider>();

        foreach (var gate in m_gates)
        {
            gate.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_spawnSpots; i++)
        {
            if (m_roomBCollider != null)
                GenerateASpawnPointInBox(m_size);
            else
                GenerateASpawnPointInSphere(m_radius);
        }

    }

    private IEnumerator SpawnWave()
    {

        foreach (var wave in m_waves[0].m_waveInformation)
        {
            for (int i = 0; i < wave.count; i++)
            {
                yield return new WaitForSeconds(m_spawnDelay);

                int selected = UnityEngine.Random.Range(0, m_spawnLocations.Count);
                Quaternion rotation = Quaternion.LookRotation(-m_spawnLocations[selected].m_forward, Vector3.up);
                SpawnEnemyObject spawn = GameObject.Instantiate(wave.spawnPrefab, m_spawnLocations[selected].m_start, rotation).GetComponent<SpawnEnemyObject>();
                spawn.m_start = m_spawnLocations[selected].m_start;
                spawn.m_end = m_spawnLocations[selected].m_end;
                spawn.m_height = m_spawnArcHeight;
                spawn.PresetTarget(m_player.gameObject);
                spawn.SetOwner(this);
            }
        }
        m_waves.RemoveAt(0);
        m_isSpawning = null;
        yield return null;
    }

    public void AddEnemy(Actor actor)
    {
        m_enemies.Add(actor);
    }

    private void GenerateASpawnPointInBox(Vector3 size)
    {
        SpawnLocation generated = new SpawnLocation();
        int safety = 0;
        bool failed = false;

        do
        {
            failed = false;
            if (safety > 15) //If failed 5 times;
                break;
            //Generate end location
            
            generated.m_end = transform.position + new Vector3(0.5f * UnityEngine.Random.Range(-size.x, size.x), 0, 0.5f * UnityEngine.Random.Range(-size.z, size.z));

            //Generate start and forward
            generated.m_forward = (generated.m_end - transform.position).normalized;
            Vector3 edgePoint = transform.position + new Vector3(
                0.5f * Mathf.Clamp(size.x * generated.m_forward.x, -size.x, size.x), 
                0, 
                0.5f * Mathf.Clamp(size.z * generated.m_forward.z, -size.z, size.z)
                );

            generated.m_start = CalculateStartPoint(generated.m_end);

            if (Physics.OverlapSphere(generated.m_start, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
            {

            }
            else
            {
                //Failed
                failed = true;
            }

            if (Physics.OverlapSphere(generated.m_end, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
            {

            }
            else
            {
                //Failed
                failed = true;
            }
            safety++;

        } while (failed);

        if(safety < 15)
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
            if (safety > 15) //If failed 5 times;
                break;

            Vector2 randPoint = UnityEngine.Random.insideUnitCircle * radius;
            generated.m_end = transform.position + new Vector3(randPoint.x, 0, randPoint.y);

            generated.m_forward = (generated.m_end - transform.position).normalized;

            generated.m_start = CalculateStartPoint(generated.m_end);

            if(Physics.OverlapSphere(generated.m_start, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
            {
                
            }
            else
            {
                //Failed
                failed = true;
            }

            if (Physics.OverlapSphere(generated.m_end, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
            {

            }
            else
            {
                //Failed
                failed = true;
            }
            safety++;

        } while (failed);

        if (safety < 15)
        {
            //Successful
            m_spawnLocations.Add(generated);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_player != null)
        {
            StartCombat();
            if(m_enemies.Count == 0 && m_isSpawning == null)
            {
                if (m_waves.Count > 0)
                {
                    m_isSpawning = StartCoroutine(SpawnWave());
                    return;
                }
            
                foreach (var gate in m_gates)
                {
                    gate.GetComponent<Animator>().SetBool("Open", true);
                }
            
                m_reward.Show(++GameManager.currentLevel);
                m_music.EndCombat();
                Destroy(this);
            }
            else
            {
                for (int i = m_enemies.Count - 1; i >= 0; i--)
                {
                    if(m_enemies[i].m_currentStateDisplay == "DEAD")
                    {
                        m_enemies.RemoveAt(i);
                    }
                }
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
    }
    private Vector3 CalculateStartPoint(Vector3 endPoint)
    {
        Vector3 direct = (endPoint - transform.position).normalized;

        return transform.position + direct * (m_radius + m_distOffEdge);
    }
    public void OnDrawGizmos()
    {
        if (m_isSphere)
            Gizmos.DrawWireSphere(transform.position, m_radius);
        else
            Gizmos.DrawWireCube(transform.position, m_size);

        foreach (var item in m_spawnLocations)
        {
            for (int i = 0; i <= 12; i++)
            {
                Gizmos.color = Color.Lerp(Color.green, Color.red, i / 12.0f);
                Gizmos.DrawSphere(MathParabola.Parabola(CalculateStartPoint(item.m_end), item.m_end, m_spawnArcHeight, i / 12.0f), m_spawnWidth);
            }
        }
    }
}
