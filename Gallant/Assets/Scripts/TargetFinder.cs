﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Remove
public class TargetFinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            FindObjectOfType<Actor>().m_target = other.gameObject;
        }
    }
}
