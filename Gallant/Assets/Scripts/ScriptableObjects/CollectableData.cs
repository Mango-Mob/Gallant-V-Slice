using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "collectableData", menuName = "Game Data/Collectalbe Data", order = 1)]
public class CollectableData : ScriptableObject
{
    [Header("Item Information")]
    public string collectableID;
    public string itemName;
    public Sprite itemIcon;

    [TextArea(10, 15)]
    public List<string> descriptions;
}
