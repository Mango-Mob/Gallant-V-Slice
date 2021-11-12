using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemData", menuName = "Game Data/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    public ItemEffect itemEffect;
    public Sprite itemIcon;

    [TextArea(10, 15)]
    public string description;
}
