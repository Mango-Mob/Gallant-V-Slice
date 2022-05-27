using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawn : MonoBehaviour
{
    public void Awake()
    {
        NarrativeManager.Instance.m_currentSpawns.Add(this);
    }

    public void SpawnCollectable(CollectableData data)
    {
        GameObject collectable = Instantiate(data.physicalPrefab, transform);
        collectable.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        collectable.GetComponentInChildren<Collectable>().m_data = data;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
