using System;
using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class SpikeTrapGroup : MonoBehaviour
    {
        [Serializable]
        public struct SpikeGroup
        {
            public SpikeTrap[] m_spikeRow;
        }

        public SpikeGroup[] m_spikeGrid;

        public SpikePatternData m_testData;


        public void Start()
        {
            if(m_testData != null && m_testData.m_spikeGridModifier.Length == m_spikeGrid.Length)
            {
                for (int i = 0; i < m_testData.m_spikeGridModifier.Length; i++)
                {
                    for (int j = 0; j < m_testData.m_spikeGridModifier[i].m_spikeMod.Length; j++)
                    {
                        if(m_testData.m_spikeGridModifier[i].m_spikeMod[j] > 0f)
                        {
                            m_spikeGrid[i].m_spikeRow[j].ExtendSpikes(m_testData.m_spikeGridModifier[i].m_spikeMod[j] * m_testData.m_baseTimer);
                        }
                    }
                }
            }
        }
    }
}
