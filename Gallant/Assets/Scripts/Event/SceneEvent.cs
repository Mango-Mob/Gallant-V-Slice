using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        foreach (var item in GetComponentsInChildren<Image>())
        {
            item.enabled = false;
        }
        Destroy(this);
    }

    public void Update()
    {
        if(GameManager.Instance.m_player != null)
        {
            GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = true;
        }
    }
}
