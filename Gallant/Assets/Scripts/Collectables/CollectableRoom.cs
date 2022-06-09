using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollectableRoom : MonoBehaviour
{
    public GameObject m_display;
    public float m_reqDist = 2.5f;

    public void Awake()
    {
        m_display.SetActive(false);
    }

    public void Update()
    {
        GetComponent<Interactable>().m_isReady = Vector3.Distance(GameManager.Instance.m_player.transform.position, transform.position) <= m_reqDist;
    }

    public void Show()
    {
        m_display.SetActive(true);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = true;
    }

    public void Hide()
    {
        m_display.SetActive(false);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = false;
    }
}