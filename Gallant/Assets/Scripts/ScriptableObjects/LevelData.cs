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
    public enum FloorType { REST, EVENT, COMBAT, SPECIAL};

    [System.Serializable]
    public struct Floor
    {
        public FloorType type;
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
}