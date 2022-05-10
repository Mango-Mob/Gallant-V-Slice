﻿using System.Collections;
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
    public Vector3 navLocalPosition;

    public List<GameObject> prefabPropsToLoad = new List<GameObject>();
}