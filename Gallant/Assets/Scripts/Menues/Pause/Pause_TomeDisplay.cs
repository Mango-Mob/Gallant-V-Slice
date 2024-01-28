using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class Pause_TomeDisplay : MonoBehaviour
{
    public GameObject m_tomePrefab;

    public List<Sprite> m_itemImages = new List<Sprite>();
    private Dictionary<ItemEffect, Pause_Tome> m_tomeDisplays = new Dictionary<ItemEffect, Pause_Tome>();
    private Player_Controller m_player;

    // Start is called before the first frame update
    void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    public void UpdateTomes()
    {
        if(m_player != null && m_player.playerStats != null)
            foreach (var item in m_player.playerStats.m_effects)
            {
                Pause_Tome temp = null;
                if(m_tomeDisplays.TryGetValue(item.Key.effect, out temp))
                {
                    temp.SetAmount(item.Value);
                }
                else
                {
                    temp = Instantiate(m_tomePrefab, transform).GetComponent<Pause_Tome>();
                    temp.SetTome(m_itemImages[(int)item.Key.effect]);
                    temp.SetAmount(item.Value);
                    m_tomeDisplays.Add(item.Key.effect, temp);
                }
            }
    }
}
