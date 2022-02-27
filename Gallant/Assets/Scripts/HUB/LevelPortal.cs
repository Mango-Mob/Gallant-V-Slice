using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string m_portalDestination = "";

    // Start is called before the first frame update
    void Start()
    {
        
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
