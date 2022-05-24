using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "floorData", menuName = "Game Data/Floor Data", order = 1)]
public class FloorData : ScriptableObject
{
    public enum FloorType { REST, EVENT, COMBAT, SPECIAL };
    public FloorType m_type;

    [Header("Combat Data")]
    public float difficultyBase;
    public AnimationCurve difficultyCurve;
    public float minDiffCost;
    public int minWaves;
    public int maxWaves;

    [Header("Scene Data")]
    public List<Extentions.WeightedOption<SceneData>> potentialScenes;

    public SceneData GetScene()
    {
        return Extentions.GetFromList<SceneData>(potentialScenes);
    }
}
