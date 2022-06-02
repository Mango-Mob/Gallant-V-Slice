using System;
using System.Collections.Generic;
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
        public float m_delayInSeconds;
        public float m_playingInSeconds = 0.0f;
        public List<SpikePatternData> m_patterns;
        private int? m_selected = null;
        private float m_timer = 0.0f;

        public void Update()
        {
            if(m_playingInSeconds > 0)
            {
                m_playingInSeconds = Mathf.Max(m_playingInSeconds - Time.deltaTime, 0);
                m_timer = Mathf.Max(m_timer - Time.deltaTime, 0);
                if (!m_selected.HasValue && m_timer <= 0)
                {
                    m_selected = UnityEngine.Random.Range(0, m_patterns.Count);
                    for (int i = 0; i < m_patterns[m_selected.Value].m_spikeGridModifier.Length; i++)
                    {
                        for (int j = 0; j < m_patterns[m_selected.Value].m_spikeGridModifier[i].m_spikeMod.Length; j++)
                        {
                            if (m_patterns[m_selected.Value].m_spikeGridModifier[i].m_spikeMod[j] > 0f)
                            {
                                m_spikeGrid[i].m_spikeRow[j].ExtendSpikes(m_patterns[m_selected.Value].m_spikeGridModifier[i].m_spikeMod[j] * m_patterns[m_selected.Value].m_baseTimer);
                            }
                        }
                    }
                    m_timer = m_delayInSeconds;
                }
                else 
                {
                    if (IsReady())
                        m_selected = null;
                }
            }
            else
            {
                m_timer = m_delayInSeconds;
            }
        }

        public bool IsReady()
        {
            for (int i = 0; i < m_spikeGrid.Length; i++)
            {
                for (int j = 0; j < m_spikeGrid[i].m_spikeRow.Length; j++)
                {
                    if(!m_spikeGrid[i].m_spikeRow[j].isFinished)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
