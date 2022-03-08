using ActorSystem.AI;
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
        public bool m_spawnOnAwake = false;
        public float m_value;

        [Header("Wave Information")]
        public bool m_generateWavesOnAwake = false;
        public List<RoomData> m_waves = new List<RoomData>();
        public List<RoomData> m_allWaves = new List<RoomData>();

        protected List<RoomData> m_waveArchive;

        public List<Actor> m_myActors { get; private set; }
        public bool m_hasStarted { get; private set; } = false;
        public SpawnDataGenerator m_generator { get; private set; }

        //MonoBehaviour
        private void Awake()
        {
            m_waveArchive = new List<RoomData>(m_waves);
            foreach (var wave in m_waves)
            {
                m_value += wave.CalculateCost();
            }
            m_generator = GetComponentInChildren<SpawnDataGenerator>();
            m_myActors = new List<Actor>();
            if (m_spawnOnAwake && m_waves.Count > 0)
            {
                StartCombat();
            }
        }

        /*******************
         * StartCombat : Starts combat for the player
         * @author : Michael Jordan
         */
        public bool StartCombat()
        {
            if(m_waves.Count > 0 || m_myActors.Count != 0)
            {
                m_hasStarted = true;

                if (GameManager.Instance.music != null && !GameManager.Instance.music.IsCombatPlaying)
                    GameManager.Instance.music.StartCombat();

                if(m_myActors.Count != 0)
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

            var wave = m_waves[0];
            m_waves.RemoveAt(0);

            SpawnWave(wave);
        }

        /*******************
         * Restart : Restarts the waves.
         * @author : Michael Jordan
         */
        public void Restart()
        {
            m_waves = new List<RoomData>(m_waveArchive);
            Stop();
        }

        /*******************
         * Stop : Stops the combat
         * @author : Michael Jordan
         */
        public void Stop()
        {
            m_hasStarted = false;
            GameManager.Instance.music.EndCombat();
        }

        /*******************
         * SpawnWave : Spawns the wave based upon the data provided.
         * @author : Michael Jordan
         * @param (RoomData) The wave to spawn
         */
        public void SpawnWave(RoomData wave)
        {
            if(m_generator.m_spawnPoints == null || m_generator.m_spawnPoints.Count == 0)
            {
                Debug.LogError("Spawner doesn't have any valid spawn points.");
                return;
            }
            //Create a copy of the scriptableObject
            RoomData data = ScriptableObject.CreateInstance<RoomData>();
            data.m_waveInformation = new List<RoomData.Actor>(wave.m_waveInformation);
            int count = data.Count();

            while (count > 0) //Until the last enemy is spawnned.
            {
                //For each select type of unit
                int selectUnit = Random.Range(0, data.m_waveInformation.Count);
                for (int i = 0; i < wave.m_waveInformation[selectUnit].count; i++)
                {
                    int selectSpawn = Random.Range(0, m_generator.m_spawnPoints.Count);
                    
                    //Get/Create actor in the reserves
                    Actor spawn = ActorManager.Instance.GetReservedActor(data.m_waveInformation[selectUnit].actor.ActorName);
                    m_myActors.Add(spawn);
                    spawn.SetTarget(GameManager.Instance.m_player);
                    spawn.m_lastSpawner = this;

                    //Start Spawn animation
                    spawn.Spawn((uint)Mathf.FloorToInt(GameManager.currentLevel), m_generator.m_spawnPoints[selectSpawn].startPoint, m_generator.m_spawnPoints[selectSpawn].endPoint, m_generator.m_spawnPoints[selectSpawn].navPoint);

                    count--;
                }

                //Remove option
                data.m_waveInformation.RemoveAt(selectUnit);
            }
        }

        //MonoBehaviour
        void Update()
        {
            if(m_hasStarted && m_myActors.Count == 0)
            {
                 if (m_waves.Count == 0)
                 {
                     Stop();
                     RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel));
                     GameManager.Advance();
                 }
                 else
                 {
                     SpawnWave(m_waves[0]);
                     m_waves.RemoveAt(0);
                 }
            }
        }

        public bool GetClosestSpawn(Vector3 position, out SpawnData closest)
        {
            float dist = float.MaxValue;
            closest = new SpawnData();
            foreach (var item in m_generator.m_spawnPoints)
            {
                if(Vector3.Distance(position, item.startPoint) < dist)
                {
                    dist = Vector3.Distance(position, item.startPoint);
                    closest = item;
                }
            }
            if(dist == float.MaxValue)
            {
                return false;
            }
            return true;
        }
    }
}
