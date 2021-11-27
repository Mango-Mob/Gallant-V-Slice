using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemReward : Reward
{
    public Text m_title;

    public Image m_itemImageLoc;
    private Image m_background;

    private Color m_baseColor;
    public AudioClip m_collectAudio;

    public ItemData m_currentlyLoaded { get; private set; }
    private Player_Controller m_activePlayer;

    private void Start()
    {
        m_background = GetComponent<Image>();
        m_baseColor = m_background.color;
    }

    public void LoadItem(ItemData data, Player_Controller player)
    {
        m_title.text = data.itemName;

        m_itemImageLoc.sprite = data.itemIcon;

        m_currentlyLoaded = data;
        m_activePlayer = player;
    }

    public override void GiveReward()
    {
        m_activePlayer?.playerStats.AddEffect(m_currentlyLoaded.itemEffect);
        GetComponentInParent<RewardWindow>().Hide();
        if(m_activePlayer != null)
            AudioManager.instance.PlayAudioTemporary(m_activePlayer.transform.position, m_collectAudio);
    }

    public override void Select()
    {
        base.Select();
        m_background.color = m_selectedColour;
    }

    public override void Unselect()
    {
        m_background.color = m_baseColor;
    }
}

