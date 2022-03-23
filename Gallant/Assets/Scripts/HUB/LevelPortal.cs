using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour
{
    public string m_portalDestination = "";
    public string m_prefRequire = "";
    public GameObject gate;
    // Start is called before the first frame update
    void Start()
    {
        if (m_prefRequire == "")
            return;

        if(PlayerPrefs.GetInt(m_prefRequire, 0) != 1)
        {
            this.enabled = false;
            gate.SetActive(false);
            GetComponent<Collider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Controller>())
        {
            other.GetComponent<Player_Controller>().StorePlayerInfo();
            LevelManager.Instance.LoadNewLevel(m_portalDestination);
        }
    }
}
