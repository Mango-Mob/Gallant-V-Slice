using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsVFX : MonoBehaviour
{
    public GameObject target;
    [SerializeField] private GameObject barrier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.forward = (target.transform.position + transform.up * 0.5f - transform.position).normalized;

        barrier.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime;
    }
}
