using System;
using UnityEngine;


[CreateAssetMenu(fileName = "spikePatternData", menuName = "Game Data/Other/SpikeData", order = 1)]
public class SpikePatternData : ScriptableObject
{
    public float m_baseTimer = 1.0f;

    [Serializable]
    public struct SpikeRow
    {
        public float[] m_spikeMod;
    }

    public SpikeRow[] m_spikeGridModifier;
}
