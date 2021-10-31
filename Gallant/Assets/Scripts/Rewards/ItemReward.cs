using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemReward : Reward
{
    public Text m_title;

    public Image m_itemImageLoc;

    public Text m_description;

    private ItemData m_currentlyLoaded;
    private Player_Controller m_activePlayer;

    public void LoadItem(ItemData data, Player_Controller player)
    {
        m_title.text = data.itemName;

        m_description.text = "Description: \n" + data.description;

        m_itemImageLoc.sprite = data.itemIcon;

        m_currentlyLoaded = data;
        m_activePlayer = player;
    }

    public override void GiveReward()
    {
        m_activePlayer?.playerStats.AddEffect(m_currentlyLoaded.itemEffect);
        GetComponentInParent<RewardWindow>().Hide();
    }
}
