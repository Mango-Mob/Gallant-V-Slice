using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GEN_PrefabSection : MonoBehaviour
{
    public int depth;
    [SerializeField] public List<GEN_LevelCollider> m_levelColliders { get; private set; }
    [SerializeField] public List<Collider> m_colliders { get; private set; }

    [SerializeField] public GEN_EntryNode m_entry { get; private set; }
    [SerializeField] public Vector3 m_offset { get; private set; }

    public void Awake()
    {
        m_levelColliders = new List<GEN_LevelCollider>(GetComponentsInChildren<GEN_LevelCollider>());
        m_colliders = new List<Collider>(GetComponentsInChildren<Collider>());
        m_entry = GetComponentInChildren<GEN_EntryNode>();
        m_offset = transform.position - m_entry.transform.position;

        foreach (var item in m_levelColliders)
        {
            item.SetOwner(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
