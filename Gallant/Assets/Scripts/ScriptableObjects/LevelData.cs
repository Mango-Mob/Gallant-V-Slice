using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(fileName = "levelData", menuName = "Game Data/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public float m_width = 1024, m_height = 900;
    [SerializeField] public List<CollectableData> m_potentialCollectablesA;
    [SerializeField] public List<CollectableData> m_potentialCollectablesB;
    public List<WaveData> m_spawnableWaves;
    public Color m_portalColor;
    public float m_levelUpPerFloor = 1.45f;
    
    public SceneData m_root;

    public List<FloorData> m_levelFloors;

    [SerializeField] protected AnimationCurve m_probOfGrowth;

    public float Evaluate(int floor)
    {
        if (m_levelFloors == null || m_levelFloors.Count == 0)
            return 0;

        float t = (floor + 1) / (float)m_levelFloors.Count;

        return m_probOfGrowth.Evaluate(t);
    }

    public List<WaveData> EvaluateCombat(FloorData floor, bool spend_max = false)
    {
        List<WaveData> result = new List<WaveData>();
        List<WaveData> options = new List<WaveData>();
        List<WaveData> archive = new List<WaveData>(m_spawnableWaves);

        //Randomise the target count for waves:
        int waveCount = UnityEngine.Random.Range(floor.minWaves, floor.maxWaves + 1);
        //Budget overflow if weaker waves is used.
        float overflow = 0;
        for (int i = 1; i < waveCount + 1; i++)
        {
            //Determine the budget for this wave
            float budget = floor.difficultyBase * floor.difficultyCurve.Evaluate((float)i/waveCount) + overflow;
            float limit = floor.minDiffCost * floor.difficultyCurve.Evaluate((float)i / waveCount);

            if (archive.Count == 0)
                archive = new List<WaveData>(m_spawnableWaves);

            options = !spend_max ? GetWavesUnderBudget(archive, budget, limit) : GetWavesMax(archive, budget);

            //Randomly select a wave available.
            WaveData toAdd = null;
            while(budget > 0 && options.Count > 0)
            {
                int select = UnityEngine.Random.Range(0, options.Count);

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

                options = GetWavesUnderBudget(options, budget, limit);
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

    private List<WaveData> GetWavesUnderBudget(List<WaveData> archive, float budget, float limit = 0)
    {
        List<WaveData> result = new List<WaveData>(archive);
        //Calculate which options are available.
        for (int j = result.Count - 1; j >= 0; j--)
        {
            if (result[j].m_diffCost > budget || result[j].m_diffCost < limit)
            {
                result.RemoveAt(j);
            }
        }

        return result;
    }
    private List<WaveData> GetWavesMax(List<WaveData> archive, float budget)
    {
        List<WaveData> options = new List<WaveData>(archive);
        List<WaveData> result = new List<WaveData>();
        //Calculate which options are available.
        float current_max = 0;
        for (int j = options.Count - 1; j >= 0; j--)
        {
            if (options[j].m_diffCost >= current_max && options[j].m_diffCost <= budget)
            {
                if(options[j].m_diffCost > current_max)
                    result.Clear();

                current_max = options[j].m_diffCost;
                result.Add(options[j]);
            }
        }

        return result;
    }

    //public FloorData GetFloorScene(int index)
    //{
    //    return m_levelFloors[index].m_data;
    //}
}