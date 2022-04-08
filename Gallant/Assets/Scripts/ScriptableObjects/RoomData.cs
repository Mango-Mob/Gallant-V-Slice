using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ActorSystem;

[CreateAssetMenu(fileName = "roomData", menuName = "Game Data/Room Data", order = 1)]
public class RoomData : ScriptableObject
{
    [Serializable]
    public struct Actor
    {
        public ActorData actor;
        public int count;
    }

    [Header("Wave Information")]
    public List<Actor> m_waveInformation;

    public float CalculateCost()
    {
        float cost = 0;

        if (m_waveInformation == null)
            return 0;

        return cost;
    }

    public int Count()
    {
        int count = 0;
        foreach (var info in m_waveInformation)
        {
            count += info.count;
        }
        return count;
    }

    public static int SortAlgorithm(RoomData a, RoomData b)
    {
        //Edge cases where a room data is null:
        int aCost = (a != null) ? Mathf.RoundToInt(a.CalculateCost()) : int.MaxValue;
        int bCost = (b != null) ? Mathf.RoundToInt(b.CalculateCost()) : int.MaxValue;

        if (aCost == bCost)
            return 0;

        return (aCost - bCost) / Mathf.Abs(aCost - bCost);
    }
}
