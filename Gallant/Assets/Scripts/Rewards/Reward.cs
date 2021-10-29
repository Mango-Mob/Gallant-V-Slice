using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reward : MonoBehaviour
{
    public abstract void GiveReward(Player_Controller player);
}
