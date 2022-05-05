using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ActorSystem;

[CreateAssetMenu(fileName = "waveData", menuName = "Game Data/Wave Data", order = 1)]
public class WaveData : ScriptableObject
{
    [Serializable]
    public struct Actor
    {
        public ActorData actor;
        public int count;
    }

    [Header("Wave Information")]
    public List<Actor> m_waveInformation;
    public float m_diffCost;
    public int Count()
    {
        int count = 0;
        foreach (var info in m_waveInformation)
        {
            count += info.count;
        }
        return count;
    }

    public static int SortAlgorithm(WaveData a, WaveData b)
    {
        //Edge cases where a room data is null:
        int aCost = (a != null) ? Mathf.RoundToInt(a.m_diffCost) : int.MaxValue;
        int bCost = (b != null) ? Mathf.RoundToInt(b.m_diffCost) : int.MaxValue;

        if (aCost == bCost)
            return 0;

        return (aCost - bCost) / Mathf.Abs(aCost - bCost);
    }
}
