using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GEN_LevelStart : MonoBehaviour
{
    [SerializeField] public int m_seed = 0;
    [Range(1, 20)]
    [SerializeField] public int m_distance = 1;
    [SerializeField] public LayerMask m_layersToCheck = ~0;

    public bool m_GenerateLevelOnAwake = false;
    public bool m_GenerateSeedOnAwake = true;
    [HideInInspector]
    [SerializeField] private GameObject[] m_levelPrefabs;
    [HideInInspector]
    [SerializeField] private GameObject m_levelEnd;

    private struct Section
    {
        public Section (GameObject _gameobject, int _depth)
        {
            depth = _depth;
            gameobject = _gameobject;
            gameobject.GetComponentInChildren<GEN_PrefabSection>().depth = _depth;
            GEN_EntryNode entry = gameobject.GetComponentInChildren<GEN_EntryNode>();
            gameobject.transform.position += (gameobject.transform.position - entry.transform.position);

            exitList = new List<GEN_ExitNode>(gameobject.GetComponentsInChildren<GEN_ExitNode>());
            SetActiveColliders(false);
        }
        public Section(Section parent, GEN_ExitNode exit, GameObject prefab)
        {
            depth = parent.depth + 1;

            //Instantiate the prefab under the selected exit.
            prefab.GetComponent<GEN_PrefabSection>().depth = depth;
            gameobject = Instantiate(prefab, exit.transform);
            GEN_EntryNode entry = gameobject.GetComponentInChildren<GEN_EntryNode>();

            //gameobject.transform.localRotation = Quaternion.Euler(0, 180, 0);
            gameobject.transform.position += (gameobject.transform.position - entry.transform.position);
            
            exitList = new List<GEN_ExitNode>(gameobject.GetComponentsInChildren<GEN_ExitNode>());
            SetActiveColliders(false);

            //Remove exit
            parent.exitList.Remove(exit);
            DestroyImmediate(entry);
            DestroyImmediate(exit);
        }

        public Section(Section parent, int selectedExit, GameObject prefab)
        {
            depth = parent.depth + 1;

            //Find selected exit
            GEN_ExitNode exit = parent.exitList[selectedExit];

            //Instantiate the prefab under the selected exit.
            prefab.GetComponent<GEN_PrefabSection>().depth = depth;
            gameobject = Instantiate(prefab, exit.transform);
            GEN_EntryNode entry = gameobject.GetComponentInChildren<GEN_EntryNode>();

            gameobject.transform.localRotation = Quaternion.Euler(0, 180, 0);
            gameobject.transform.position += (gameobject.transform.position - entry.transform.position);
            exitList = new List<GEN_ExitNode>(gameobject.GetComponentsInChildren<GEN_ExitNode>());
            SetActiveColliders(false);

            //Remove exit
            parent.exitList.Remove(exit);
            DestroyImmediate(entry);
            DestroyImmediate(exit);
        }

        public void SetActiveColliders(bool status)
        {
            foreach (var item in exitList)
            {
                foreach (var collider in item.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = status;
                }
            }
        }

        public int depth;
        public GameObject gameobject;
        public List<GEN_ExitNode> exitList;
    }

    [HideInInspector]
    [SerializeField] private List<Section> m_levelSections = new List<Section>();
    private void Awake()
    {
        if(m_GenerateSeedOnAwake)
            m_seed = (int)System.DateTime.Now.Ticks;
        if (m_GenerateLevelOnAwake)
        {
            Clear();
            Generate(m_seed);
        }
    }

    public List<GameObject> GetLevelPrefabs()
    {
        return new List<GameObject>(m_levelPrefabs);
    }

    internal void Copy(out List<GameObject> m_sectionPrefabs, out GameObject m_endCapPrefab)
    {
        m_sectionPrefabs = new List<GameObject>(m_levelPrefabs);
        m_endCapPrefab = m_levelEnd;
    }

    public bool SetLevelPrefabs(GameObject[] prefabs, GameObject endCap)
    {
        if(!IsArrayFull(prefabs))
        {
            Debug.LogError("<GEN> Level start was created without a complete list of prefabs.");
            return false;
        }

        m_levelPrefabs = prefabs;
        m_levelEnd = endCap;
        return true;
    }

    public void Generate(int seed = 0)
    {
        if(!IsArrayFull(m_levelPrefabs))
        {
            DestroyImmediate(this);
            Debug.LogError("<GEN> Level start was created without a complete list of prefabs.");
            return;
        }

        ClearSections();

        //Set seed and start timer
        DateTime start = DateTime.Now;
        m_seed = seed;
        if (seed == 0)
        {
            m_seed = (int)System.DateTime.Now.Ticks;
        }

        UnityEngine.Random.InitState(m_seed);
        //Select initial prefab
        List<GameObject> prefabList = GetListOfValidPrefabs(transform, Quaternion.Euler(0, 180, 0));

        if (prefabList.Count == 0)
            return;

        int prefabSelect = UnityEngine.Random.Range(0, prefabList.Count);
        GameObject _gameObject = Instantiate(prefabList[prefabSelect], transform);
        _gameObject.name = $"<Gen> Section [{m_levelSections.Count}]";
        _gameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
        m_levelSections.Add(new Section(_gameObject, 0));

        if(m_distance - 1 == 0)
        {
            GenerateBossRoom();
        }

        List<Section> currentSections = new List<Section>(m_levelSections);
        while (currentSections.Count > 0) //m_levelSections[m_levelSections.Count - 1].depth < m_distance && 
        {
            Physics.SyncTransforms();
            if (m_levelSections.Count >= Mathf.Pow(m_distance, 2))
            {
                break;
            }
                
            int parentSelected = UnityEngine.Random.Range(0, currentSections.Count);
            int exitSelected = UnityEngine.Random.Range(0, currentSections[parentSelected].exitList.Count);

            prefabList = GetListOfValidPrefabs(currentSections[parentSelected].exitList[exitSelected].transform, Quaternion.Euler(0, 180, 0));          

            if (prefabList.Count == 0)
            {
                currentSections[parentSelected].exitList.RemoveAt(exitSelected);
            }
            else
            {
                prefabSelect = UnityEngine.Random.Range(0, prefabList.Count);

                m_levelSections.Add(new Section(currentSections[parentSelected], exitSelected, prefabList[prefabSelect]));
                currentSections.Add(m_levelSections[m_levelSections.Count - 1]);
                m_levelSections[m_levelSections.Count - 1].gameobject.name = $"<Gen> Section [{m_levelSections.Count - 1}]";
                m_levelSections[m_levelSections.Count - 1].gameobject.transform.SetParent(transform);
            }

            //BOSS AREA START
            if (m_levelSections[m_levelSections.Count - 1].depth >= m_distance - 1)
            {
                if(GenerateBossRoom())
                {
                    break;
                }
            }
            //BOSS AREA END

            //Clear any sections with zero exits
            for (int i = currentSections.Count - 1; i >= 0; i--)
            {
                if (currentSections[i].exitList.Count == 0)
                {
                    currentSections.RemoveAt(i);
                }
            }
        }

        for (int i = 0; i < m_levelSections.Count; i++)
        {
            m_levelSections[i].gameobject.transform.SetParent(transform);
            m_levelSections[i].SetActiveColliders(true);
        }

        Debug.Log($"Generate level in: {(DateTime.Now - start).TotalMilliseconds} ms");
    }

    private bool GenerateBossRoom()
    {
        Physics.SyncTransforms();

        GEN_PrefabSection owner = m_levelEnd.GetComponent<GEN_PrefabSection>();
        owner.Awake();

        List<GEN_ExitNode> exits = new List<GEN_ExitNode>(m_levelSections[m_levelSections.Count - 1].exitList);
        for (int i = exits.Count - 1; i >= 0; i--)
        {
            foreach (var collider in owner.m_levelColliders)
            {
                if (!collider.enabled)
                    continue;

                List<Collider> hitColliders = collider.IsOverlapping(exits[i].transform, Quaternion.Euler(0, 180, 0), m_layersToCheck, GEN_LevelGenMainWindow.showErrors);
                if (hitColliders.Count > 0)
                {
                    exits.RemoveAt(i);
                    break;
                }
            }
        }

        if (exits.Count > 0)
        {
            int select = UnityEngine.Random.Range(0, exits.Count);
            m_levelSections.Add(new Section(m_levelSections[m_levelSections.Count - 1], exits[select], m_levelEnd));
            m_levelSections[m_levelSections.Count - 1].gameobject.transform.SetParent(transform);
            return true;
        }
        return false;
    }

    private List<GameObject> GetListOfValidPrefabs(Transform parent, Quaternion prefabLocalRotation)
    {
        //Select initial prefab
        List<GameObject> prefabList = new List<GameObject>(m_levelPrefabs);
        for (int i = prefabList.Count - 1; i >= 0; i--)
        {
            GEN_PrefabSection owner = prefabList[i].GetComponent<GEN_PrefabSection>();
            owner.Awake();

            //Check each collider for if the selected prefab is valid.
            foreach (var collider in owner.m_levelColliders)
            {
                if (!collider.enabled)
                    continue;

                List<Collider> hitColliders = collider.IsOverlapping(parent, prefabLocalRotation, m_layersToCheck, GEN_LevelGenMainWindow.showErrors);
                if (hitColliders.Count > 0)
                {
                    prefabList.RemoveAt(i);
                    break; //Stop looping
                }
            }
        }
        return prefabList;
    }

    public void Clear()
    {
        ClearSections();
        var list = FindObjectsOfType<GEN_ErrorNode>();
        for (int i = list.Length - 1; i >= 0; i--)
        {
            DestroyImmediate(list[i].gameObject);
        }
    }

    public Vector3 CalculateAveragePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach (var item in m_levelSections)
        {
            pos += item.gameobject.transform.position;
        }
        return (m_levelSections.Count > 0) ? pos / m_levelSections.Count : transform.position;
    }

    private void ClearSections()
    {
        while(m_levelSections.Count > 0)
        {
            DestroyImmediate(m_levelSections[m_levelSections.Count - 1].gameobject);
            m_levelSections.RemoveAt(m_levelSections.Count - 1);
        }
    }

    private bool IsArrayFull(GameObject[] prefabs)
    {
        if (prefabs == null)
            return false;

        if (prefabs.Length == 0)
            return false;

        foreach (var item in prefabs)
        {
            if (item == null)
            {
                return false;
            }
        }
        return true;
    }

    public void GenerateErrorNodeAt(Transform location, List<Collider> hits)
    {
        GEN_ErrorNode.CreateErrorAt(location.position + location.forward, Quaternion.identity, Vector3.one, hits);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Handles.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
        Handles.ConeHandleCap(0, transform.position + transform.forward * 0.5f, Quaternion.LookRotation(transform.forward, Vector3.up), 0.20f, EventType.Repaint);
    }
}
