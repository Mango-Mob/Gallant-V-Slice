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

    private void OnInspectorUpdate()
    {
        m_selected = Selection.activeObject as GameObject;

        Repaint();
    }

    private void OnGUI()
    {
        MainWindow();
    }

    private void MainWindow()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("<b><size=20> Generators:</size></b>", m_style);

        if (GUILayout.Button(m_levelStartButton, GUILayout.Width(75), GUILayout.Height(75)))
        {
            GEN_LevelGenCreatorWindow.Init();
        }
        GUILayout.Label("<b><size=20> Prefab Components:</size></b>", m_style);
        GUILayout.BeginHorizontal();
        GUI.enabled = (m_selected != null);
        if (GUILayout.Button(m_levelEntryButton, GUILayout.Width(75), GUILayout.Height(75)))
        {
            GameObject enterance = new GameObject();
            enterance.name = "EntryNode";
            enterance.transform.SetParent(m_selected.transform);
            enterance.transform.localPosition = Vector3.zero;
            enterance.transform.localRotation = Quaternion.identity;
            enterance.AddComponent<GEN_EntryNode>();
        }
        if (GUILayout.Button(m_levelExitButton, GUILayout.Width(75), GUILayout.Height(75)))
        {
            GameObject exit = new GameObject();
            exit.name = "ExitNode";
            exit.transform.SetParent(m_selected.transform);
            exit.transform.localPosition = Vector3.zero;
            exit.transform.localRotation = Quaternion.identity;
            exit.AddComponent<GEN_ExitNode>();
        }
        if (GUILayout.Button(m_levelColliderButton, GUILayout.Width(75), GUILayout.Height(75)))
        {

        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }
}
