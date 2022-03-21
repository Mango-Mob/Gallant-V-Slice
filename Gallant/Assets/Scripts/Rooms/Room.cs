using ActorSystem;
using ActorSystem.AI;
using ActorSystem.Spawning;
using Exceed.Debug;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool m_finalRoom;
    public GameObject[] m_gates;

    public Actor[] m_existingActors;

    private Renderer m_debugRenderer;

    public ActorSpawner m_mySpawnner { get; private set; }
    public RewardManager.RewardType m_rewardType = RewardManager.RewardType.STANDARD;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_mySpawnner = GetComponentInChildren<ActorSpawner>();
        m_debugRenderer = GetComponent<Renderer>();
        foreach (var item in m_gates)
        {
            item.SetActive(false);
        }
    }

    private void Start()
    {
        if(m_mySpawnner != null)
        {
            foreach (var actor in m_existingActors)
            {
                m_mySpawnner.m_myActors.Add(actor);
                actor.m_lastSpawner = m_mySpawnner;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_debugRenderer.enabled = DebugManager.showRoomLocations;

        if (m_mySpawnner.m_waves.Count == 0 && !m_mySpawnner.m_hasStarted)
        {
            foreach (var gate in m_gates)
            {
                gate.SetActive(false);
            }
            if(m_finalRoom)
            {
                if(!RewardManager.isShowing)
                {
                    GameManager.Instance.FinishLevel();
                }
            }
        }
    }

    public bool IsComplete()
    {
        return m_mySpawnner.m_waves.Count == 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(m_mySpawnner.enabled && !m_mySpawnner.m_hasStarted)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if(m_mySpawnner.StartCombat())
                {
                    foreach (var gate in m_gates)
                    {
                        gate.SetActive(true);
                    }
                }
            }
        }
    }
}
