using ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] m_gates;
    private ActorSpawner m_mySpawnner;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_mySpawnner = GetComponentInChildren<ActorSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_mySpawnner?.StartCombat();
            foreach (var gate in m_gates)
            {
                gate.SetActive(true);
            }
            GetComponent<Collider>().enabled = false;
        }
    }
}
