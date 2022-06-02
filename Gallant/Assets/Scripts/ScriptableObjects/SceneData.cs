using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ActorSystem;

[CreateAssetMenu(fileName = "sceneData", menuName = "Game Data/Scene Data")]
public class SceneData : ScriptableObject
{
    public Sprite sceneIcon;
    public Sprite sceneCompleteIcon;
    public Sprite iconBack;
    public string sceneToLoad;
    public GameObject prefabToLoad;

    public enum SceneType { Rest, Event, Combat, Boss, Special}
    public SceneType sType;

    public List<GameObject> prefabPropsToLoad = new List<GameObject>();

    public static List<Extentions.WeightedOption<SceneData>> EvaluateWeights(SceneData previous, List<Extentions.WeightedOption<SceneData>> existing)
    {
        List<Extentions.WeightedOption<SceneData>> result = new List<Extentions.WeightedOption<SceneData>>(existing);
        
        if(result.Count == 1)
            return result;

        for (int i = 0; i < result.Count; i++)
        {
            switch (result[i].data.sType)
            {
                case SceneType.Rest:
                    //Rule 1: A rest scene must not proceed with another rest scene.
                    //Rule 2: A rest scene must not proceed from an event scene.
                    if (previous.sType == SceneType.Event || previous.sType == SceneType.Rest)
                    {
                        result.SetWeightAt(i, 0); //Set weight to zero
                    }
                    break;
                case SceneType.Event:
                    //Rule 3: An event scene must not proceed from an event scene.
                    //Rule 4: An event scene must not proceed from an rest scene.
                    if (previous.sType == SceneType.Event || previous.sType == SceneType.Rest)
                    {
                        result.SetWeightAt(i, 0); //Set weight to zero
                    }
                    break;
                case SceneType.Combat:
                    //Rule 5: A combat event must not proceed from itself.
                    if(previous.prefabToLoad == result[i].data.prefabToLoad)
                    {
                        result.SetWeightAt(i, 0); //Set weight to zero
                    }
                    break;
                case SceneType.Boss:
                    break;
                default:
                    break;
            }
        }

        return result;
    }
}