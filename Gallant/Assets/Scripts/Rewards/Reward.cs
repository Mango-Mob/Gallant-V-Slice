using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reward : MonoBehaviour
{
    public Color m_selectedColour;
    public abstract void GiveReward();
    public abstract void Unselect();

    public int m_id;
    public virtual void Select()
    {
        RewardManager.Instance.Select(m_id);
    }

    public virtual void ShowMyDescription()
    {
        RewardManager.Instance.Hover(m_id);
    }
}
