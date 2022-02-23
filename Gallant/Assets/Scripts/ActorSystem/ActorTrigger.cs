using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class ActorTrigger : MonoBehaviour
{
    public GameObject gate;
    private float m_time = 0;
    private Player_Controller m_player;
    public int nextSceneIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        //if (gate != null)
        //{
        //    gate.GetComponent<Animator>().enabled = false;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if(testActor.m_myBrain.IsDead)
        //{
        //    m_time += Time.deltaTime;
        //    if (m_time > 3.0f)
        //    {
        //        //m_player.StorePlayerInfo();
        //        //EndScreenMenu.levelReached++;
        //        //LevelManager.Instance.LoadNewLevel("MainLevel_2");
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach (var actor in ActorManager.Instance.m_subscribed)
            {
                actor.SetTarget(other.gameObject);
            }
            //ActorManager.Instance.Subscribe(testActor);
            //testActor.m_target = other.gameObject;
            //m_player = other.GetComponentInChildren<Player_Controller>();
            //if (gate != null)
            //{
            //    gate.GetComponent<Animator>().enabled = true;
            //}
            //GetComponent<Collider>().enabled = false;
        }
    }
}
