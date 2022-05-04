using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneEvent : MonoBehaviour
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        HUDManager.Instance.gameObject.SetActive(false);
        DialogManager.Instance.Show();
    }

    public virtual void EndEvent()
    {
        NavigationManager.Instance.SetVisibility(true, false);
        DialogManager.Instance.Hide();
        Destroy(gameObject);
    }

    public void Update()
    {
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = true;
    }
}
