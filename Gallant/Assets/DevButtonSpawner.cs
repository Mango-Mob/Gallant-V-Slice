using ActorSystem.AI;
using ActorSystem.Spawning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevButtonSpawner : MonoBehaviour
{
    public string actorName;
    public SpawnDataGenerator data;
    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;

    private Interactable m_myInterface;

    private void Awake()
    {
        m_myInterface = GetComponentInChildren<Interactable>();
        m_myInterface.m_interactFunction.AddListener(Spawn);
    }

    private void Update()
    {
        m_keyboardInput.transform.parent.gameObject.SetActive(m_myInterface.m_isReady);
        m_keyboardInput.gameObject.SetActive(m_myInterface.m_isReady && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_myInterface.m_isReady && InputManager.Instance.isInGamepadMode);
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
}
