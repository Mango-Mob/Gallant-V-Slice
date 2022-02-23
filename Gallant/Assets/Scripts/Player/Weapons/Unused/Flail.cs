using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flail : MonoBehaviour
{
    [Header("Weapon Components")]
    public Transform chainAnchor1;
    public Transform chainAnchor2;
    public LineRenderer chain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        chain.SetPosition(0, chainAnchor1.position - transform.position);
        chain.SetPosition(1, chainAnchor2.position - transform.position);
    }
}
