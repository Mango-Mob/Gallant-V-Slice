using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GEN_LevelGenCreatorWindow : EditorWindow
{
    private List<GameObject> m_sectionPrefabs = new List<GameObject>();
    private GameObject m_endCapPrefab;

    private GameObject m_selected;

    public static void Init()
    {
        EditorWindow.GetWindow<GEN_LevelGenCreatorWindow>("Start Component");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_selected = Selection.activeObject as GameObject;
        Repaint();
    }

    private void OnGUI()
    {
        if(m_selected == null)
            EditorGUILayout.LabelField($"Selected Object: null");
        else
            EditorGUILayout.LabelField($"Selected Object: {m_selected.name}");
        EditorGUILayout.Space();

        int count = EditorGUILayout.DelayedIntField("Prefab Size: ", m_sectionPrefabs.Count);

        while (count < m_sectionPrefabs.Count)
            m_sectionPrefabs.RemoveAt(m_sectionPrefabs.Count - 1);
        while (count > m_sectionPrefabs.Count)
            m_sectionPrefabs.Add(null);

        for (int i = 0; i < m_sectionPrefabs.Count; i++)
        {
            GameObject temp = EditorGUILayout.ObjectField($"  Element {i}:", m_sectionPrefabs[i], typeof(GameObject), false) as GameObject;

            if(temp != null && m_sectionPrefabs[i] != temp)
            { 
                m_sectionPrefabs[i] = ValidatePrefab(temp, 2, 1);
            }
        }

        EditorGUILayout.Space();
        m_endCapPrefab = ValidatePrefab(EditorGUILayout.ObjectField($"End Cap:", m_endCapPrefab, typeof(GameObject), false) as GameObject, 2, 0);

        if(GUILayout.Button("Clear"))
        {
            m_sectionPrefabs.Clear();
            m_endCapPrefab = null;
        }
        GUI.enabled = m_selected.GetComponent<GEN_LevelStart>() != null;
        if (GUILayout.Button("Copy"))
        {
            m_selected.GetComponent<GEN_LevelStart>().Copy(out m_sectionPrefabs, out m_endCapPrefab);
        }
        GUI.enabled = CanGenerate() && m_selected.GetComponent<GEN_LevelStart>() != null;
        if (GUILayout.Button("Paste"))
        {
            m_selected.GetComponent<GEN_LevelStart>().SetLevelPrefabs(m_sectionPrefabs.ToArray(), m_endCapPrefab);
        }
        GUI.enabled = CanGenerate();
        if (GUILayout.Button("Generate Start Object"))
        {
            GameObject temp = new GameObject();
            temp.name = "Level Start";
            if (m_selected == null)
            {
                Vector3 pos = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
                Vector3 forward = UnityEditor.SceneView.lastActiveSceneView.camera.transform.forward;
                temp.transform.position = pos + forward * 2.0f;

            }
            else
            {
                temp.transform.SetParent(m_selected.transform);
                temp.transform.localPosition = Vector3.zero;
                temp.transform.localRotation = Quaternion.identity;
            }

            temp.AddComponent<GEN_LevelStart>().SetLevelPrefabs(m_sectionPrefabs.ToArray(), m_endCapPrefab);
            Selection.activeObject = temp;
        }
        GUI.enabled = true;
    }

    private GameObject ValidatePrefab(GameObject prefabIn, int EnteranceStatus, int ExitStatus)
    {
        if (prefabIn == null)
            return null;

        GEN_EntryNode entry = prefabIn.GetComponentInChildren<GEN_EntryNode>();
        GEN_ExitNode exit = prefabIn.GetComponentInChildren<GEN_ExitNode>();

        if(entry == null && EnteranceStatus >= 1)
        {
            string error = $"<GEN 001> : Prefab ({prefabIn.name}) doesn't contain an entry node.";
            if(EnteranceStatus > 1)
            {
                Debug.LogError(error);
                return null;
            }
            else
            {
                Debug.LogWarning(error);
            }
        }
        if (exit == null && ExitStatus >= 1)
        {
            string error = $"<GEN 002> : Prefab ({prefabIn.name}) doesn't contain an exit node.";
            if (ExitStatus > 1)
            {
                Debug.LogError(error);
                return null;
            }
            else
            {
                Debug.LogWarning(error);
            }
        }
        return prefabIn;
    }

    private bool CanGenerate()
    {
        foreach (var item in m_sectionPrefabs)
        {
            if(item == null || item.GetComponentInChildren<GEN_EntryNode>() == null)
            {
                return false;
            }
        }

        return m_endCapPrefab != null && m_sectionPrefabs.Count != 0;
    }
}
