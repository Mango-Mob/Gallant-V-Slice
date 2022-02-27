using ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool m_finalRoom;
    public GameObject[] m_gates;
    private ActorSpawner m_mySpawnner;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_mySpawnner = GetComponentInChildren<ActorSpawner>();

        foreach (var item in m_gates)
        {
            item.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_mySpawnner == null)
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
            else
                Destroy(this);
        }
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
