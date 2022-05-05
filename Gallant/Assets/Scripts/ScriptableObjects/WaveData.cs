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
        public Actor(ActorData _ref, int _count) { actor = _ref; count = _count; }

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

    public static WaveData CombineWaves(WaveData a, WaveData b)
    {
        if (a == null || a.m_waveInformation == null || a.m_waveInformation.Count == 0)
            return b;

        if (b == null || b.m_waveInformation == null || b.m_waveInformation.Count == 0)
            return a;

        WaveData result = ScriptableObject.CreateInstance<WaveData>();

        List<Actor> unoptimisedList = new List<Actor>();
        unoptimisedList.AddRange(a.m_waveInformation);
        unoptimisedList.AddRange(b.m_waveInformation);

        result.m_waveInformation = new List<Actor>();
        for (int i = 0; i < unoptimisedList.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < result.m_waveInformation.Count; j++)
            {
                if(unoptimisedList[i].actor == result.m_waveInformation[j].actor)
                {
                    result.m_waveInformation[j] = new Actor(unoptimisedList[i].actor, unoptimisedList[i].count + result.m_waveInformation[j].count);
                    found = true;
                }
            }
            if(!found)
            {
                result.m_waveInformation.Add(unoptimisedList[i]);
            }
        }
        result.name = $"{a.name}+{b.name}";
        result.m_diffCost = a.m_diffCost + b.m_diffCost;
        return result;
    }
}
