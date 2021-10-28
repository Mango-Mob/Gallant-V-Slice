using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public int m_spawnCount;
    public float m_spawnWidth = 0.25f;
    public float m_distOffEdge = 1.0f;
    public float m_spawnDelay = 1.0f;

    public GameObject m_EnemyToSpawn;

    private float m_timer;
    
    private BoxCollider m_roomCollider;

    private struct SpawnLocation
    {
        public Vector3 m_start;
        public Vector3 m_end;

        public Vector3 m_forward;
    }


    private List<SpawnLocation> m_spawnLocations = new List<SpawnLocation>();

    private void Awake()
    {
        m_roomCollider = GetComponent<BoxCollider>();
        m_timer = m_spawnDelay;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_spawnCount; i++)
        {
            GenerateASpawnPoint(m_roomCollider.bounds);
        }
    }

    private void GenerateASpawnPoint(Bounds bounds)
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
            RaycastHit rhit;
            generated.m_end = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));

            if (Physics.Raycast(generated.m_end, Vector3.down, out rhit, 1.0f))
            {
                generated.m_end = rhit.point;
            }

            //Generate start and forward
            NavMeshHit nhit;
            if (NavMesh.FindClosestEdge(generated.m_end, out nhit, NavMesh.AllAreas))
            {
                generated.m_forward = (nhit.position - generated.m_end).normalized;
                if (Physics.Raycast(nhit.position + rhit.point * m_distOffEdge, Vector3.down, out rhit, LayerMask.NameToLayer("Water")))
                {
                    generated.m_start = rhit.point;
                }
                else
                {
                    failed = true;
                }
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
        else if(m_spawnLocations.Count > 0)
        {
            int selected = Random.Range(0, m_spawnLocations.Count);
            Quaternion rotation = Quaternion.LookRotation(-m_spawnLocations[selected].m_forward, Vector3.up);
            SpawnEnemyObject spawn = GameObject.Instantiate(m_EnemyToSpawn, m_spawnLocations[selected].m_start, rotation).GetComponent<SpawnEnemyObject>();
            spawn.m_start = m_spawnLocations[selected].m_start;
            spawn.m_end = m_spawnLocations[selected].m_end;

            m_spawnLocations.RemoveAt(selected);
            m_timer = 0;
        }
    }

    public void OnDrawGizmos()
    {
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
