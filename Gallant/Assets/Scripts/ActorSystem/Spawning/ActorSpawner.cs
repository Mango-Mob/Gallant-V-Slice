using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.Spawning
{
    /****************
     * ActorSystem.Spawning / ActorSpawner : A class incharge of spawning waves of enemies by being the actor's interpretor for spawn point generators.
     * @author : Michael Jordan
     * @file : ActorSpawner.cs
     * @year : 2021
     */
    public class ActorSpawner : MonoBehaviour
    {
        [Header("Wave Information")]
        private List<WaveData> m_waves;

        protected List<WaveData> m_waveArchive;

        public List<Actor> m_myActors { get; private set; }
        public bool m_hasStarted { get; private set; } = false;
        public SpawnDataGenerator[] m_generators;
        public bool giveRewardUponCompletion = true;
        public bool isSpawnning = false;
        public bool spawnMaxOnly = false;
        private bool hasAReward = true;
        private int activeRoutines = 0;
        private float spawnDelay = 0.5f;
        private int m_currentWave = 0;
        private int m_maxWave = 0;
        //MonoBehaviour
        private void Awake()
        {
            m_generators = GetComponentsInChildren<SpawnDataGenerator>();
            m_myActors = new List<Actor>();
            if(NavigationManager.Instance.m_generatedLevel != null)
            {
                LevelData data = NavigationManager.Instance.m_generatedLevel;
                int floor = NavigationManager.Instance.GetFloor();
                if (floor < 0)
                    return;

                m_waves = data.EvaluateCombat(NavigationManager.Instance.GetActiveFloor(), spawnMaxOnly);
                m_maxWave = m_waves.Count;
                if (m_waves == null)
                {
                    Destroy(this);
                    return;
                }

                m_waveArchive = new List<WaveData>(m_waves);
                StartCombat();
            }
        }

        /*******************
         * StartCombat : Starts combat for the player
         * @author : Michael Jordan
         */
        public bool StartCombat(bool giveReward = true)
        {
            hasAReward = giveReward;
            if (m_waves.Count > 0 || m_myActors.Count != 0)
            {
                m_hasStarted = true;

                if(m_hasStarted)
                    ActorManager.Instance.m_activeSpawnners.Add(this);

                if(m_myActors == null)
                    m_myActors = new List<Actor>();

                if (m_myActors.Count != 0)
                {
                    foreach (var actor in m_myActors)
                    {
                        actor.SetLevel((uint)Mathf.FloorToInt(GameManager.currentLevel));
                        actor.SetTarget(GameManager.Instance.m_player);
                    }
                }

                return true;
            }
            
            return false;
        }

        /*******************
         * ForceWave : Forces the next wave to spawn
         * @author : Michael Jordan
         */
        public void ForceWave(bool startCombat = false)
        {
            m_hasStarted = (startCombat || m_hasStarted);

            if (m_hasStarted)
                ActorManager.Instance.m_activeSpawnners.Add(this);

            var wave = m_waves[0];
            m_waves.RemoveAt(0);

            SpawnWave(wave);
        }

        public void ForceEnd()
        {
            for (int i = m_myActors.Count - 1; i >= 0; i--)
            {
                m_myActors[i].DestroySelf();
            }
            m_myActors.Clear();
            Stop();
        }

        /*******************
         * Restart : Restarts the waves.
         * @author : Michael Jordan
         */
        public void Restart()
        {
            m_waves = new List<WaveData>(m_waveArchive);
            Stop();
        }

        public void OnDestroy()
        {
            ActorManager.Instance?.m_activeSpawnners.Remove(this);
        }

        /*******************
         * Stop : Stops the combat
         * @author : Michael Jordan
         */
        public void Stop()
        {
            m_hasStarted = false;
            ActorManager.Instance.m_activeSpawnners.Remove(this);
        }

        /*******************
         * SpawnWave : Spawns the wave based upon the data provided.
         * @author : Michael Jordan
         * @param (RoomData) The wave to spawn
         */
        public void SpawnWave(WaveData wave)
        {
            if (wave == null)
                return;

            HUDManager.Instance.DisplayWaveUpdate(++m_currentWave, m_maxWave);

            foreach (var item in wave.m_waveInformation)
            {
                StartCoroutine(SpawnActors(item.actor, 0.25f, item.count));
            }
        }

        private IEnumerator SpawnActors(ActorData actor, float delay, int qantity)
        {
            isSpawnning = true;
            if (ActorManager.Instance.m_reserved.ContainsKey(actor.ActorName))
            {
                activeRoutines++;
                for (int i = 0; i < qantity; i++)
                {
                    yield return new WaitForSeconds(delay);

                    int spawnSelect = 0;
                    Vector3 spawnLoc;

                    do
                    {
                        yield return new WaitForEndOfFrame();
                        spawnSelect = Random.Range(0, m_generators.Length);
                    } while (!m_generators[spawnSelect].GetASpawnPoint(actor.radius, out spawnLoc));

                    Actor spawn = ActorManager.Instance.GetReservedActor(actor.ActorName);
                    m_myActors.Add(spawn);
                    spawn.SetTarget(GameManager.Instance.m_player);
                    spawn.m_lastSpawner = this;
                    spawn.Spawn((uint)Mathf.FloorToInt(GameManager.currentLevel), spawnLoc, Quaternion.identity);
                }

                activeRoutines--;
            }
            isSpawnning = false;
            yield return null;
        }

        //MonoBehaviour
        void Update()
        {
            if(m_hasStarted && activeRoutines == 0)
                HUDManager.Instance.DisplayEnemyUpdate(m_myActors.Count, m_myActors.Count > 0 && m_myActors.Count <= 5);

            if (m_hasStarted && m_myActors.Count == 0 && activeRoutines == 0)
            {
                if (spawnDelay > 0.0f)
                    spawnDelay -= Time.deltaTime;

                
                if (m_waves.Count == 0 && spawnDelay <= 0)
                {
                    Stop();
                    if(hasAReward)
                    {
                        if(giveRewardUponCompletion)
                            RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel));
                        GameManager.Advance();
                        EndScreenMenu.roomsCleared++;
                    }
                }
                else if (spawnDelay <= 0)
                {
                    SpawnWave(m_waves[0]);
                    m_waves.RemoveAt(0);
                    spawnDelay = 0.5f;
                }
            }
        }
    }
}
