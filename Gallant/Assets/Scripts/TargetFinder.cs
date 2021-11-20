using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Remove
public class TargetFinder : MonoBehaviour
{
    public Actor testActor;

    public GameObject gate;
    public AtmosphereScript script;
    private float m_time = 0;
    private Player_Controller m_player;
    public int nextSceneIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        script = FindObjectOfType<AtmosphereScript>();
        if (gate != null)
        {
            gate.GetComponent<Animator>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(testActor.m_currentStateDisplay == "DEAD")
        {
            m_time += Time.deltaTime;
            if (m_time > 3.0f)
            {
                m_player.StorePlayerInfo();
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            testActor.m_target = other.gameObject;
            m_player = other.GetComponentInChildren<Player_Controller>();
            if (gate != null)
            {
                gate.GetComponent<Animator>().enabled = true;
                script.StartCombat();
            }
            GetComponent<Collider>().enabled = false;
        }
    }
}
