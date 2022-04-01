using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ActorSystem;

[CreateAssetMenu(fileName = "sceneData", menuName = "Game Data/Scene Data")]
public class SceneData : ScriptableObject
{
    public string sceneToLoad;
    public GameObject prefabToLoad;
}