using ActorSystem.AI;
using ActorSystem.AI.Components;
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
        public bool m_spawnOnAwake = false;
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
        [Header("Wave Information")]
        public bool m_generateWavesOnAwake = false;
        public List<RoomData> m_waves = new List<RoomData>();
        public List<RoomData> m_allWaves = new List<RoomData>();

        public List<Actor> m_myActors { get; private set; }
        public struct SpawnLocation
        {
            public Vector3 m_start;
            public Vector3 m_end;
        
            public Vector3 m_forward;
        }
        //private bool m_hasCombatStarted = false;
        //private Coroutine m_isSpawning = null;
        //
        private bool m_hasStarted = false;

        public uint m_spawnLocationCount;
        private List<SpawnLocation> m_spawnLocations = new List<SpawnLocation>();
        //private List<Actor> m_enemies = new List<Actor>();

        private void Awake()
        {
            for (int i = 0; i < m_spawnLocationCount; i++)
            {
                CreateSpawn();
            }
            m_myActors = new List<Actor>();
            if (m_spawnOnAwake && m_waves.Count > 0)
            {
                var wave = m_waves[0];
                m_waves.RemoveAt(0);

                SpawnWave(wave);
            }
            
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

        public SpawnLocation CreateSpawn(Vector3 start)
        {
            var loc = new SpawnLocation();
            int safety = 5;
            NavMeshHit sample;
            Vector2 direct;
            do
            {
                safety--;
                direct = transform.position.DirectionTo(start);
                if (m_isSphere)
                {
                    loc.m_start = transform.position + new Vector3(direct.x, transform.position.y, direct.y).normalized * m_outerRadius;
                }
                else
                {
                    Vector2 onSquare = Extentions.OnUnitSquare(direct);
                    loc.m_start = transform.position + new Vector3(onSquare.x, transform.position.y, onSquare.y) * m_outerRadius;
                }

                loc.m_end = transform.position + new Vector3(direct.x, transform.position.y, direct.y).normalized * UnityEngine.Random.Range(0, m_innerRadius);

                if (NavMesh.SamplePosition(transform.position + new Vector3(direct.x * m_innerRadius, m_innerHeight, direct.y * m_innerRadius), out sample, 0.5f, NavMesh.AllAreas))
                {
                    loc.m_end = sample.position;
                }

            } while (!sample.hit && safety > 0);

            loc.m_forward = loc.m_start.DirectionTo(loc.m_end);

            return loc;
        }

        public void SpawnWave(RoomData wave)
        {
            m_hasStarted = true;
            RoomData data = ScriptableObject.CreateInstance<RoomData>();
            data.m_waveInformation = new List<RoomData.Actor>(wave.m_waveInformation);
            int count = data.Count();

            while (count > 0)
            {
                //For each select type of unit
                int selectUnit = UnityEngine.Random.Range(0, data.m_waveInformation.Count);
                for (int i = 0; i < wave.m_waveInformation[selectUnit].count; i++)
                {
                    int selectSpawn = UnityEngine.Random.Range(0, m_spawnLocations.Count);
                    
                    //Get/Create actor in the reserves
                    Actor spawn = ActorManager.Instance.GetReservedActor(data.m_waveInformation[selectUnit].spawnName);
                    m_myActors.Add(spawn);
                    spawn.SetTarget(GameManager.instance.m_player);
                    spawn.m_lastSpawner = this;

                    //Start Spawn animation
                    spawn.Spawn((uint)Mathf.FloorToInt(GameManager.currentLevel), m_spawnLocations[selectSpawn].m_start, m_spawnLocations[selectSpawn].m_end, m_spawnLocations[selectSpawn].m_forward);

                    //Create new spawn location
                    m_spawnLocations.RemoveAt(selectSpawn);
                    CreateSpawn();
                    count--;
                }

                //Remove option
                data.m_waveInformation.RemoveAt(selectUnit);
            }
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
            if(m_hasStarted && m_myActors.Count == 0)
            {
                 if (m_waves.Count == 0)
                 {
                     Destroy(gameObject);
                 }
                 else
                 {
                     SpawnWave(m_waves[0]);
                     m_waves.RemoveAt(0);
                 }
            }
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
