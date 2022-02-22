using ActorSystem.AI;
using GEN.Nodes;
using GEN.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem
{
    public class ActorSpawner : MonoBehaviour
    {
        //public const float budgetPerDepth = 100f;
        //public bool IsRoom = true;
        //
        //public float m_spawnWidth = 0.25f;
        //public float m_spawnArcHeight = 5.0f;
        //public float m_distOffEdge = 1.0f;
        //public float m_spawnDelay = 1.0f;
        //
        //public GameObject m_ActorToSpawn;
        //
        //private Player_Controller m_player = null;
        //public GameObject m_gatePrefab;
        //public GameObject[] m_gatesLoc;
        //
        public bool m_isSphere = true;
        public float m_innerRadius = 10.0f;
        public float m_innerHeight = 0;
        public float m_outerRadius = 10.0f;
        //public Vector3 m_size = new Vector3(1f, 1f, 1f);
        //
        //public RewardWindow m_reward;
        //
        //public int m_spawnSpots = 12;
        //
        //public bool m_generateWavesOnAwake = false;
        //public List<RoomData> m_waves = new List<RoomData>();
        //public List<RoomData> m_allWaves = new List<RoomData>();
        private struct SpawnLocation
        {
            public Vector3 m_start;
            public Vector3 m_end;
        
            public Vector3 m_forward;
        }
        //private bool m_hasCombatStarted = false;
        //private Coroutine m_isSpawning = null;
        //
        public uint m_spawnLocationCount;
        private List<SpawnLocation> m_spawnLocations = new List<SpawnLocation>();
        //private List<Actor> m_enemies = new List<Actor>();

        private void Awake()
        {
            for (int i = 0; i < m_spawnLocationCount; i++)
            {
                CreateSpawn();
            }
            //m_reward = FindObjectOfType<RewardWindow>();
            //
            //if (m_generateWavesOnAwake && m_allWaves.Count > 0)
            //{
            //    GenerateWaves();
            //}
        }

        private void CreateSpawn()
        {
            var loc = new SpawnLocation();
            int safety = 5;
            NavMeshHit sample;
            Vector2 direct;
            do
            {
                safety--;
                if (m_isSphere)
                {
                    direct = UnityEngine.Random.insideUnitCircle;
                    loc.m_start = transform.position + new Vector3(direct.x, transform.position.y, direct.y).normalized * m_outerRadius;
                }
                else
                {
                    direct = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                    Vector2 onSquare = Extentions.OnUnitSquare(direct);
                    loc.m_start = transform.position + new Vector3(onSquare.x, transform.position.y, onSquare.y) * m_outerRadius;
                }

                if (NavMesh.SamplePosition(transform.position + new Vector3(direct.x * m_innerRadius, m_innerHeight, direct.y * m_innerRadius), out sample, 0.5f, NavMesh.AllAreas))
                {
                    loc.m_end = sample.position;
                }
            } while (!sample.hit && safety > 0);
            loc.m_forward = loc.m_start.DirectionTo(loc.m_end);

            m_spawnLocations.Add(loc);
        }

        // Start is called before the first frame update
        void Start()
        {
            //foreach (var gate in m_gatesLoc)
            //{
            //    if (gate.GetComponentInChildren<ExitNode>())
            //        continue;
            //
            //    Instantiate(m_gatePrefab, gate.transform);
            //    gate.SetActive(false);
            //}
            //
            //for (int i = 0; i < m_spawnSpots; i++)
            //{
            //    if (!m_isSphere)
            //        GenerateASpawnPointInBox(m_size);
            //    else
            //        GenerateASpawnPointInSphere(m_radius);
            //}
        }

        //private IEnumerator SpawnWave()
        //{
        //
        //    foreach (var wave in m_waves[0].m_waveInformation)
        //    {
        //        for (int i = 0; i < wave.count; i++)
        //        {
        //            yield return new WaitForSeconds(m_spawnDelay);
        //
        //            int selected = UnityEngine.Random.Range(0, m_spawnLocations.Count);
        //            Quaternion rotation = Quaternion.LookRotation(-m_spawnLocations[selected].m_forward, Vector3.up);
        //            SpawnActorObject spawn = GameObject.Instantiate(wave.spawnPrefab, m_spawnLocations[selected].m_start, rotation).GetComponent<SpawnActorObject>();
        //            spawn.m_start = m_spawnLocations[selected].m_start;
        //            spawn.m_end = m_spawnLocations[selected].m_end;
        //            spawn.m_height = m_spawnArcHeight;
        //            spawn.PresetTarget(m_player.gameObject);
        //            //spawn.SetOwner(this);
        //        }
        //    }
        //    m_waves.RemoveAt(0);
        //    m_isSpawning = null;
        //    yield return null;
        //}

        //public void AddActor(Actor actor)
        //{
        //    m_enemies.Add(actor);
        //}

        //private void GenerateASpawnPointInBox(Vector3 size)
        //{
        //    SpawnLocation generated = new SpawnLocation();
        //    int safety = 0;
        //    bool failed = false;
        //
        //    do
        //    {
        //        failed = false;
        //        if (safety > 15) //If failed 5 times;
        //            break;
        //        //Generate end location
        //
        //        generated.m_end = transform.position + new Vector3(0.5f * UnityEngine.Random.Range(-size.x, size.x), 0, 0.5f * UnityEngine.Random.Range(-size.z, size.z));
        //
        //        //Generate start and forward
        //        generated.m_forward = (generated.m_end - transform.position).normalized;
        //        Vector3 edgePoint = transform.position + new Vector3(
        //            0.5f * Mathf.Clamp(size.x * generated.m_forward.x, -size.x, size.x),
        //            0,
        //            0.5f * Mathf.Clamp(size.z * generated.m_forward.z, -size.z, size.z)
        //            );
        //
        //        generated.m_start = CalculateStartPoint(generated.m_end);
        //
        //        if (Physics.OverlapSphere(generated.m_start, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
        //        {
        //
        //        }
        //        else
        //        {
        //            //Failed
        //            failed = true;
        //        }
        //
        //        if (Physics.OverlapSphere(generated.m_end, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
        //        {
        //
        //        }
        //        else
        //        {
        //            //Failed
        //            failed = true;
        //        }
        //        safety++;
        //
        //    } while (failed);
        //
        //    if (safety < 15)
        //    {
        //        //Successful
        //        m_spawnLocations.Add(generated);
        //    }
        //}
        //
        //private void GenerateASpawnPointInSphere(float radius)
        //{
        //    SpawnLocation generated = new SpawnLocation();
        //    int safety = 0;
        //    bool failed = false;
        //
        //    do
        //    {
        //        failed = false;
        //        if (safety > 15) //If failed 5 times;
        //            break;
        //
        //        Vector2 randPoint = UnityEngine.Random.insideUnitCircle * radius;
        //        generated.m_end = transform.position + new Vector3(randPoint.x, 0, randPoint.y);
        //
        //        generated.m_forward = (generated.m_end - transform.position).normalized;
        //
        //        generated.m_start = CalculateStartPoint(generated.m_end);
        //
        //        if (Physics.OverlapSphere(generated.m_start, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
        //        {
        //
        //        }
        //        else
        //        {
        //            //Failed
        //            failed = true;
        //        }
        //
        //        if (Physics.OverlapSphere(generated.m_end, m_spawnWidth, 1 << LayerMask.NameToLayer("Environment")).Length == 0)
        //        {
        //
        //        }
        //        else
        //        {
        //            //Failed
        //            failed = true;
        //        }
        //        safety++;
        //
        //    } while (failed);
        //
        //    if (safety < 15)
        //    {
        //        //Successful
        //        m_spawnLocations.Add(generated);
        //    }
        //}

        // Update is called once per frame
        void Update()
        {
            //if (m_player != null)
            //{
            //    StartCombat();
            //    if (m_enemies.Count == 0 && m_isSpawning == null)
            //    {
            //        if (m_waves.Count > 0)
            //        {
            //            m_isSpawning = StartCoroutine(SpawnWave());
            //            return;
            //        }
            //
            //        if (IsRoom)
            //        {
            //            foreach (var gate in m_gatesLoc)
            //            {
            //                gate.GetComponentInChildren<Animator>()?.SetBool("Open", true);
            //            }
            //
            //            GameManager.Advance();
            //            EndScreenMenu.roomsCleared++;
            //            m_reward.Show(Mathf.FloorToInt(GameManager.currentLevel));
            //            GetComponent<MultiAudioAgent>().PlayOnce("GateOpen");
            //        }
            //        Destroy(this);
            //    }
            //    else
            //    {
            //        for (int i = m_enemies.Count - 1; i >= 0; i--)
            //        {
            //            if (m_enemies[i].m_myBrain.IsDead)
            //            {
            //                m_enemies.RemoveAt(i);
            //            }
            //        }
            //    }
            //}
        }

        //public void GenerateWaves()
        //{
        //    PrefabSection section = GetComponentInParent<PrefabSection>();
        //    int depth = (section != null) ? section.depth : Mathf.RoundToInt(GameManager.currentLevel);
        //    float budget = (depth + 1) * budgetPerDepth;
        //
        //    //Remove all waves that cost more than the budget.
        //    for (int i = m_allWaves.Count - 1; i >= 0; i--)
        //    {
        //        if (m_allWaves[i].CalculateCost() > budget)
        //        {
        //            m_allWaves.RemoveAt(i);
        //        }
        //    }
        //
        //    while (m_allWaves.Count > 0 && budget > 0)
        //    {
        //        int select = UnityEngine.Random.Range(0, m_allWaves.Count);
        //        m_waves.Add(m_allWaves[select]);
        //        budget -= m_allWaves[select].CalculateCost();
        //        //Remove all waves that cost more than the budget.
        //        for (int i = m_allWaves.Count - 1; i >= 0; i--)
        //        {
        //            if (m_allWaves[i].CalculateCost() > budget)
        //            {
        //                m_allWaves.RemoveAt(i);
        //            }
        //        }
        //    }
        //
        //    m_waves.Sort(RoomData.SortAlgorithm);
        //}

        //public void StartCombat()
        //{
        //    if (m_hasCombatStarted)
        //        return;
        //
        //    m_hasCombatStarted = true;
        //    if (IsRoom)
        //    {
        //        GetComponent<MultiAudioAgent>().PlayOnce("GateClose");
        //        foreach (var gate in m_gatesLoc)
        //        {
        //            gate.SetActive(true);
        //        }
        //    }
        //}

        //private Vector3 CalculateStartPoint(Vector3 endPoint)
        //{
        //    Vector3 direct = (endPoint - transform.position).normalized;
        //
        //    return transform.position + direct * (m_radius + m_distOffEdge);
        //}

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            if (m_isSphere)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position + new Vector3(0, m_innerHeight, 0), Quaternion.identity, new Vector3(1f, 0f, 1f));
                
                Gizmos.DrawWireSphere(Vector3.zero, m_innerRadius);

                Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
                Gizmos.DrawWireSphere(Vector3.zero, m_outerRadius);

                Gizmos.matrix = Matrix4x4.identity;
            }
            else
            {
                Gizmos.DrawWireCube(transform.position + new Vector3(0, m_innerHeight, 0), new Vector3(m_innerRadius * 2, 0, m_innerRadius * 2));
                Gizmos.DrawWireCube(transform.position, new Vector3(m_outerRadius * 2, 0, m_outerRadius * 2));
            }


            foreach (var item in m_spawnLocations)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(item.m_start, 0.25f);
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(item.m_end, 0.25f);
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(item.m_start, item.m_end);

            }
        }
    }
}
