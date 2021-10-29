using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemReward : Reward
{
    public Text m_title;

    public Image m_weaponImageLoc;

    public Text m_description;

    private ItemData m_currentlyLoaded;

    public void LoadItem(ItemData data)
    {
        m_title.text = data.itemName;

        m_description.text = "Description: \n" + data.description;

        m_weaponImageLoc.sprite = data.itemIcon;

        m_currentlyLoaded = data;
    }

    public override void GiveReward(Player_Controller player)
    {
        player.playerStats.AddEffect(m_currentlyLoaded.itemEffect);
    }
}
