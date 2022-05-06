using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(fileName = "levelData", menuName = "Game Data/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public List<WaveData> m_spawnableWaves;
    public enum FloorType { REST, EVENT, COMBAT, SPECIAL};

    [System.Serializable]
    public struct Floor
    {
        public FloorType type;
        public AnimationCurve difficultyCurve;
        public float difficultyBase;
        public int minWaves;
        public int maxWaves;
        public SceneData[] potentialScenes;
    }

    public SceneData m_root;

    public List<Floor> m_levelFloors;
    public uint m_minRoomCount;
    public uint m_maxRoomCount;
    [SerializeField] protected AnimationCurve m_probOfGrowth;

    public float Evaluate(int floor)
    {
        if (m_levelFloors == null || m_levelFloors.Count == 0)
            return 0;

        float t = (floor + 1) / (float)m_levelFloors.Count;

        return m_probOfGrowth.Evaluate(t);
    }

    public List<WaveData> EvaluateCombat(uint floor)
    {
        if (floor > m_levelFloors.Count)
            return null;

        if (m_levelFloors[(int)floor].type != FloorType.COMBAT)
            return null;

        List<WaveData> result = new List<WaveData>();
        List<WaveData> options = new List<WaveData>();
        List<WaveData> archive = new List<WaveData>(m_spawnableWaves);

        //Randomise the target count for waves:
        int waveCount = Random.Range(m_levelFloors[(int)floor].minWaves, m_levelFloors[(int)floor].maxWaves + 1);
        //Budget overflow if weaker waves is used.
        float overflow = 0;
        for (int i = 1; i < waveCount + 1; i++)
        {
            //Determine the budget for this wave
            float budget = m_levelFloors[(int)floor].difficultyBase * m_levelFloors[(int)floor].difficultyCurve.Evaluate((float)i/waveCount) + overflow;

            if (archive.Count == 0)
                archive = new List<WaveData>(m_spawnableWaves);

            options = GetWavesUnderBudget(archive, budget);

            //Randomly select a wave available.
            WaveData toAdd = null;
            while(budget > 0 && options.Count > 0)
            {
                int select = Random.Range(0, options.Count);

                if(toAdd == null)
                {
                    toAdd = options[select];
                }
                else
                {
                    toAdd = WaveData.CombineWaves(toAdd, options[select]);
                }

                //Remove from archive, so it won't be picked again.
                archive.Remove(options[select]);
                
                budget -= options[select].m_diffCost;
                options.RemoveAt(select);

                options = GetWavesUnderBudget(options, budget);
            }

            if(toAdd != null)
            {
                result.Add(toAdd);
            }
            
            //Add remaining budget to the overflow account
            overflow += budget;
        }
        result.Sort(WaveData.SortAlgorithm);
        return result;
    }

    private List<WaveData> GetWavesUnderBudget(List<WaveData> archive, float budget)
    {
        List<WaveData> result = new List<WaveData>(archive);
        //Calculate which options are available.
        for (int j = result.Count - 1; j >= 0; j--)
        {
            if (result[j].m_diffCost > budget)
            {
                result.RemoveAt(j);
            }
        }

        return result;
    }
}