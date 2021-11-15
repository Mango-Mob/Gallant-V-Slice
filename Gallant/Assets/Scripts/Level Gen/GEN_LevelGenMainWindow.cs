using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GEN_LevelGenMainWindow : EditorWindow
{
    public static bool showErrors = true;
    private GameObject m_selected;

    private Texture m_levelStartIcon;
    private GUIContent m_levelStartButton;

    private Texture m_levelEntryIcon;
    private GUIContent m_levelEntryButton;

    private Texture m_levelExitIcon;
    private GUIContent m_levelExitButton;

    private Texture m_levelColliderIcon;
    private GUIContent m_levelColliderButton;

    private GUIStyle m_style;

    private Vector2 scrollPos;

    private List<GameObject> m_sectionPrefabs = new List<GameObject>();
    private GameObject m_endCapPrefab;

    private enum Window
    {
        MAIN, START, PREFAB,
    }

    private Window m_currentWindow = Window.MAIN;

    [MenuItem("Tools/Level Generator")]
    public static void Init()
    {
        EditorWindow.GetWindow<GEN_LevelGenMainWindow>("Level Generator");
    }

    private void Awake()
    {
        m_style = new GUIStyle();
        m_style.richText = true;

        m_levelStartIcon = Resources.Load("LevelStartIcon") as Texture;
        m_levelStartButton = new GUIContent(m_levelStartIcon);

        m_levelEntryIcon = Resources.Load("EnteranceIcon") as Texture;
        m_levelEntryButton = new GUIContent(m_levelEntryIcon);

        m_levelExitIcon = Resources.Load("ExitIcon") as Texture;
        m_levelExitButton = new GUIContent(m_levelExitIcon);

        m_levelColliderIcon = Resources.Load("ColliderIcon") as Texture;
        m_levelColliderButton = new GUIContent(m_levelColliderIcon);

        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        
    }

    private void OnInspectorUpdate()
    {
        m_selected = Selection.activeObject as GameObject;

        Repaint();
    }

    private void OnGUI()
    {
        switch (m_currentWindow)
        {
            default:
            case Window.MAIN:
                MainWindow();
                break;
            case Window.START:
                LevelStartMenu();
                break;
            case Window.PREFAB:
                PrefabMenu();
                break;
        }
    }

    private void MainWindow()
    {
        GUILayout.Label("Menu");
        if (GUILayout.Button("Level Generator"))
        {
            m_currentWindow = Window.START;
        }
        if (GUILayout.Button("Prefab Tools"))
        {
            m_currentWindow = Window.PREFAB;
        }
        //scrollPos = GUILayout.BeginScrollView(scrollPos);
        //GUILayout.Label("<b><size=20> Generators:</size></b>", m_style);
        //
        //if (GUILayout.Button(m_levelStartButton, GUILayout.Width(75), GUILayout.Height(75)))
        //{
        //    GEN_LevelGenCreatorWindow.Init();
        //}
        //GUILayout.Label("<b><size=20> Prefab Components:</size></b>", m_style);
        //GUILayout.BeginHorizontal();
        //GUI.enabled = (m_selected != null);
        //if (GUILayout.Button(m_levelEntryButton, GUILayout.Width(75), GUILayout.Height(75)))
        //{
        //    GameObject enterance = new GameObject();
        //    enterance.name = "EntryNode";
        //    enterance.transform.SetParent(m_selected.transform);
        //    enterance.transform.localPosition = Vector3.zero;
        //    enterance.transform.localRotation = Quaternion.identity;
        //    enterance.AddComponent<GEN_EntryNode>();
        //}
        //if (GUILayout.Button(m_levelExitButton, GUILayout.Width(75), GUILayout.Height(75)))
        //{
        //    GameObject exit = new GameObject();
        //    exit.name = "ExitNode";
        //    exit.transform.SetParent(m_selected.transform);
        //    exit.transform.localPosition = Vector3.zero;
        //    exit.transform.localRotation = Quaternion.identity;
        //    exit.AddComponent<GEN_ExitNode>();
        //}
        //if (GUILayout.Button(m_levelColliderButton, GUILayout.Width(75), GUILayout.Height(75)))
        //{
        //
        //}
        //GUI.enabled = true;
        //GUILayout.EndHorizontal();
        //GUILayout.EndScrollView();
    }

    private void LevelStartMenu()
    {
        GUILayout.Label("Menu > Level Generator");

        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        if (m_selected == null)
        {
            EditorGUILayout.LabelField($"Selected Object: null");
            EditorGUILayout.LabelField($"Target: as a new GameObject.");
        }
        else
        {
            EditorGUILayout.LabelField($"Selected Object: {m_selected.name}");
            EditorGUILayout.LabelField($"Target: as a child GameObject.");
        }          
        rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        EditorGUILayout.Space();

        int count = EditorGUILayout.DelayedIntField("Prefab Size: ", m_sectionPrefabs.Count);

        while (count < m_sectionPrefabs.Count)
            m_sectionPrefabs.RemoveAt(m_sectionPrefabs.Count - 1);
        while (count > m_sectionPrefabs.Count)
            m_sectionPrefabs.Add(null);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < m_sectionPrefabs.Count; i++)
        {
            GameObject temp = EditorGUILayout.ObjectField($"  Element {i}:", m_sectionPrefabs[i], typeof(GameObject), false) as GameObject;

            if (temp != null && m_sectionPrefabs[i] != temp)
            {
                m_sectionPrefabs[i] = ValidatePrefab(temp, 2, 1);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        m_endCapPrefab = ValidatePrefab(EditorGUILayout.ObjectField($"Level End:", m_endCapPrefab, typeof(GameObject), false) as GameObject, 2, 0);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(" Clear ", GUILayout.Height(40)))
        {
            m_sectionPrefabs.Clear();
            m_endCapPrefab = null;
        }
        GUI.enabled = m_selected.GetComponent<GEN_LevelStart>() != null;
        if (GUILayout.Button("Copy\nExisting", GUILayout.Height(40)))
        {
            m_selected.GetComponent<GEN_LevelStart>().Copy(out m_sectionPrefabs, out m_endCapPrefab);
        }
        GUI.enabled = CanGenerate() && m_selected.GetComponent<GEN_LevelStart>() != null;
        if (GUILayout.Button("Paste\nExisting", GUILayout.Height(40)))
        {
            m_selected.GetComponent<GEN_LevelStart>().SetLevelPrefabs(m_sectionPrefabs.ToArray(), m_endCapPrefab);
        }
        EditorGUILayout.EndHorizontal();
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

        if (GUILayout.Button("Back"))
        {
            m_currentWindow = Window.MAIN;
        }
    }
    
    private void PrefabMenu()
    {
        
        if (GUILayout.Button("Back"))
        {
            m_currentWindow = Window.MAIN;
        }
    }


    private GameObject ValidatePrefab(GameObject prefabIn, int EnteranceStatus, int ExitStatus)
    {
        if (prefabIn == null)
            return null;

        GEN_EntryNode entry = prefabIn.GetComponentInChildren<GEN_EntryNode>();
        GEN_ExitNode exit = prefabIn.GetComponentInChildren<GEN_ExitNode>();

        if (entry == null && EnteranceStatus >= 1)
        {
            string error = $"<GEN 001> : Prefab ({prefabIn.name}) doesn't contain an entry node.";
            if (EnteranceStatus > 1)
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
            if (item == null || item.GetComponentInChildren<GEN_EntryNode>() == null)
            {
                return false;
            }
        }

        return m_endCapPrefab != null && m_sectionPrefabs.Count != 0;
    }
}
