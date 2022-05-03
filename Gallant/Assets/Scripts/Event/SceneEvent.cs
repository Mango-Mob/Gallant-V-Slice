using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneEvent : MonoBehaviour
{
    public List<ItemData> m_data = new List<ItemData>();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        HUDManager.Instance.gameObject.SetActive(false);
        DialogManager.Instance.Show();
    }

    protected abstract void GenerateCase();
    public abstract void Interact();
    void Update()
    {
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = true;
    }
}
