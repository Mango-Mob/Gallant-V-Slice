using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "roomData", menuName = "Game Data/Room Data", order = 1)]
public class RoomData : ScriptableObject
{
    [Serializable]
    public struct Enemy
    {
        public GameObject spawnPrefab;
        public int count;
    }

    [Header("Wave Information")]
    public List<Enemy> m_waveInformation;

    public float CalculateCost()
    {
        float cost = 0;
        foreach (var wave in m_waveInformation)
        {
            cost += wave.spawnPrefab.GetComponent<SpawnEnemyObject>().m_spawnCost * wave.count;
        }
        return cost;
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
