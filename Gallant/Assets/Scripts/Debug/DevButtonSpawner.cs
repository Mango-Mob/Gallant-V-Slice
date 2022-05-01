using ActorSystem.AI;
using ActorSystem.Spawning;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevButtonSpawner : MonoBehaviour
{
    public string actorName;
    public SpawnDataGenerator data;

    private Interactable m_myInterface;
    public TMP_Text m_counter;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
    }

    private void Update()
    {
        m_counter?.SetText(ActorManager.Instance.GetActorCount(actorName).ToString());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_myInterface.m_isReady = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_myInterface.m_isReady = false;
        }
    }

    public void Spawn()
    {
        int spawnSelect = 0;
        Vector3 spawnLoc;
        Actor spawn = ActorManager.Instance.GetReservedActor(actorName);

        if (spawn == null)
            return;

        if(data.GetASpawnPoint(spawn.m_myData.radius, out spawnLoc))
        {
            spawn.Spawn((uint)Mathf.FloorToInt(GameManager.currentLevel), spawnLoc);
            return;
        }

        ActorManager.Instance.Kill(spawn);        
    }

    public void Despawn()
    {
        ActorManager.Instance.KillAll();
    }

    public void ToggleWalls(GameObject walls)
    {
        walls.SetActive(!walls.activeInHierarchy);
    }
}
