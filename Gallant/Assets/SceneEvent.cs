using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        HUDManager.Instance.gameObject.SetActive(false);
        DialogManager.Instance.Show();
    }

    void Update()
    {
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = true;
    }

    public void Interact()
    {
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = false;
        HUDManager.Instance.gameObject.SetActive(true);
    }
}
