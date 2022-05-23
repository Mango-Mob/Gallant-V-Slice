using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneSync : MonoBehaviour
{
    public Transform m_boneToSyncWith;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = m_boneToSyncWith.position;
        transform.rotation = m_boneToSyncWith.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_boneToSyncWith.position;
        transform.rotation = m_boneToSyncWith.rotation;
    }
}
