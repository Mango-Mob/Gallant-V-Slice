using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

[CreateAssetMenu(fileName = "itemData", menuName = "Game Data/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    public ItemEffect itemEffect;

    public enum UtilityType { RUNE, FORGE, ORB};
    public UtilityType itemType = UtilityType.RUNE;
    public Sprite itemIcon;

    [TextArea(10, 15)]
    public string description;

    public void Apply()
    {
        if (GameManager.Instance.m_player == null)
            return;

        switch (itemType)
        {
            case UtilityType.RUNE:
                GameManager.Instance?.m_player?.GetComponent<Player_Controller>()?.playerStats.AddEffect(itemEffect);
                break;
            case UtilityType.FORGE:
                DroppedWeapon.CreateWeaponUpgrade(GameManager.Instance.m_player.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f);
                break;
            case UtilityType.ORB:
                GameManager.Instance?.m_player?.GetComponent<Player_Controller>()?.playerResources.ChangeAdrenaline(1);
                break;
            default:
                break;
        }
    }
}
