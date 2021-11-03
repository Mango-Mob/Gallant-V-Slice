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
}
