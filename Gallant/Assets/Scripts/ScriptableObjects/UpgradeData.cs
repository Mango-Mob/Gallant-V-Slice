using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

[CreateAssetMenu(fileName = "upgradeData", menuName = "Game Data/Upgrade Data", order = 1)]
public class UpgradeData : ScriptableObject
{
    [Header("Upgrade Information")]
    public string itemName;
    public ItemEffect itemEffect;
    public Sprite itemIcon;

    [TextArea(10, 15)]
    public string description;
}
