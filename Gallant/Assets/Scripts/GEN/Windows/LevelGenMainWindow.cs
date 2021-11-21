using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GEN.Nodes;
using GEN.Users;

namespace GEN.Windows
{
    public class LevelGenMainWindow : EditorWindow
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

        private GUIStyle m_headerStyle;
        private GUIStyle m_normalStyle;
        private GUIStyle m_boldLinkStyle;
        private GUIStyle m_confirmButton;
        private GUIStyle m_declineButton;
        private GUIStyle m_confirmText;
        private GUIStyle m_declineText;

        private bool m_showConfirmation = false;

        private enum Window
        {
            MAIN, START, PREFAB,
        }

        private enum PrefabWindow
        {
            NULL, SECTION, CAP
        }

        private Window m_currentWindow = Window.MAIN;
        private PrefabWindow m_currentPrefabWindow = PrefabWindow.NULL;

        [MenuItem("Tools/Level Generator")]
        public static void Init()
        {
            EditorWindow.GetWindow<LevelGenMainWindow>("Level Generator");
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

            m_headerStyle = new GUIStyle(EditorStyles.label);
            m_headerStyle.fontSize = 25;

            m_normalStyle = new GUIStyle(EditorStyles.label);
            m_normalStyle.alignment = EditorStyles.linkLabel.alignment;
            m_normalStyle.fontStyle = FontStyle.Bold;

            m_confirmText = new GUIStyle(EditorStyles.label);
            m_confirmText.normal.textColor = Color.green;
            m_confirmText.active.textColor = Color.green;
            m_confirmText.hover.textColor = Color.green;
            m_confirmText.fontSize = 25;

            m_declineText = new GUIStyle(EditorStyles.label);
            m_declineText.normal.textColor = Color.red;
            m_declineText.active.textColor = Color.red;
            m_declineText.hover.textColor = Color.red;
            m_declineText.fontSize = 25;

            m_boldLinkStyle = new GUIStyle(EditorStyles.linkLabel);
            m_boldLinkStyle.fontStyle = FontStyle.Bold;
            m_boldLinkStyle.alignment = EditorStyles.boldLabel.alignment;

            m_confirmButton = new GUIStyle(EditorStyles.miniButton);
            m_confirmButton.normal.textColor = Color.green;
            m_declineButton = new GUIStyle(EditorStyles.miniButton);
            m_declineButton.normal.textColor = Color.red;

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
            GameObject select = Selection.activeObject as GameObject;

            if(m_currentWindow == Window.PREFAB && select != null)
            {
                //Get the top parent
                while(select.transform.parent != null)
                {
                    select = select.transform.parent.gameObject;
                }
            }

            if(m_selected != select)
            {
                m_selected = select;
                Repaint();
            }
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
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Menu", m_headerStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

            bool status = EditorGUILayout.Toggle(new GUIContent(" Show Build Errors:", "Allows all generators to create error node. Error nodes are created when a level detects a section can't be used, because of a collision."), showErrors);
            if(status != showErrors)
            {
                showErrors = status;
                if(!showErrors)
                {
                    var list = FindObjectsOfType<ErrorNode>();
                    for (int i = list.Length - 1; i >= 0; i--)
                    {
                        DestroyImmediate(list[i].gameObject);
                    }
                }
            }

            if (GUILayout.Button("Level Generator", GUILayout.Height(40)))
            {
                m_currentWindow = Window.START;
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Prefab Tools", GUILayout.Height(40)))
            {
                m_currentWindow = Window.PREFAB;
            }
        }

        private void LevelStartMenu()
        {
            HyperLinkHeader();

            LabelSelected();

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
        
            if (GUILayout.Button(" Clear "))
            {
                m_sectionPrefabs.Clear();
                m_endCapPrefab = null;
            }
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = m_selected != null && m_selected.GetComponent<StartNode>() != null;
            string name = (m_selected != null) ? "\n"+m_selected.name : "";
            if (GUILayout.Button($"Copy{name}", GUILayout.Height(40)))
            {
                m_selected.GetComponent<StartNode>().Copy(out m_sectionPrefabs, out m_endCapPrefab);
            }
            GUI.enabled = m_selected != null && CanGenerate() && m_selected.GetComponent<StartNode>() != null;
            if (GUILayout.Button($"Paste{name}", GUILayout.Height(40)))
            {
                m_selected.GetComponent<StartNode>().Paste(m_sectionPrefabs.ToArray(), m_endCapPrefab);
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

                temp.AddComponent<StartNode>().Paste(m_sectionPrefabs.ToArray(), m_endCapPrefab);
                Selection.activeObject = temp;
            }
            GUI.enabled = true;
       
            if (GUILayout.Button("Back", GUILayout.Height(50)))
            {
                m_currentWindow = Window.MAIN;
            }
        }
    
        private void PrefabMenu()
        {
            switch (m_currentPrefabWindow)
            {
                default:
                case PrefabWindow.NULL:
                    {
                        HyperLinkHeader();

                        LabelSelected();

                        EditorGUILayout.Space();
                    
                        if (GUILayout.Button("Section", GUILayout.Height(40)))
                        {
                            m_currentPrefabWindow = PrefabWindow.SECTION;
                        }
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Cap", GUILayout.Height(40)))
                        {
                            m_currentPrefabWindow = PrefabWindow.CAP;
                        }
                        GUILayout.FlexibleSpace();
                        if (m_showConfirmation)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("Are you sure?", EditorStyles.boldLabel, GUILayout.Width(EditorStyles.boldLabel.CalcSize(new GUIContent("Are you sure? ")).x));
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();

                            if (GUILayout.Button("Yes", m_confirmButton))
                            {
                                PrefabSection start = m_selected.GetComponent<PrefabSection>();
                                EntryNode entry = m_selected.GetComponentInChildren<EntryNode>();
                                ExitNode[] exits = m_selected.GetComponentsInChildren<ExitNode>();
                                ColliderNode[] colliders = m_selected.GetComponentsInChildren<ColliderNode>();

                                if (start != null)
                                    DestroyImmediate(start);

                                if (entry != null)
                                {
                                    if (entry.gameObject.name == "EntryNode")
                                        entry.gameObject.name = "CleanedNode";
                                    DestroyImmediate(entry);
                                }


                                if (exits.Length != 0)
                                {
                                    for (int i = exits.Length - 1; i >= 0; i--)
                                    {
                                        if (exits[i].gameObject.name == "ExitNode")
                                            exits[i].gameObject.name = "CleanedNode";

                                        DestroyImmediate(exits[i]);
                                    }
                                }

                                if (colliders.Length != 0)
                                {
                                    for (int i = colliders.Length - 1; i >= 0; i--)
                                    {
                                        DestroyImmediate(colliders[i]);
                                    }
                                }
                            }
                            if (GUILayout.Button("No", m_declineButton))
                            {
                                m_showConfirmation = false;
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        GUI.enabled = !m_showConfirmation && m_selected != null;
                        if (GUILayout.Button("Clean Prefab"))
                        {
                            m_showConfirmation = true;

                        }
                        GUI.enabled = true;
                        if (GUILayout.Button("Back", GUILayout.Height(50)))
                        {
                            m_currentWindow = Window.MAIN;
                        }
                    }

                
                    break;
                case PrefabWindow.SECTION:
                    SectionMenu();
                    break;
                case PrefabWindow.CAP:
                    CapWindow();
                    break;

            }
        }

        private void SectionMenu()
        {
            HyperLinkHeader();

            LabelSelected();
            bool isValid = false;
            if (m_selected != null)
            {
                bool canSelect = false;
            
                EditorGUILayout.BeginHorizontal();
                canSelect = EditorGUILayout.Toggle("Contains a Prefab Section", m_selected.GetComponent<PrefabSection>(), GUILayout.Width(200));
                isValid = canSelect;
                GUI.enabled = !canSelect;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject = m_selected.AddComponent<PrefabSection>().gameObject;
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject = m_selected;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                canSelect = EditorGUILayout.Toggle("Contains a Entry Node", m_selected.GetComponentInChildren<EntryNode>(), GUILayout.Width(200));
                isValid = canSelect && isValid;
                GUI.enabled = !canSelect;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject = new GameObject();
                    Selection.activeGameObject.name = "EntryNode";
                    Selection.activeGameObject.transform.SetParent(m_selected.transform);
                    Selection.activeGameObject.transform.localPosition = Vector3.zero;
                    Selection.activeGameObject.transform.localRotation = Quaternion.identity;
                    Selection.activeGameObject.AddComponent<EntryNode>();
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeObject = m_selected.GetComponentInChildren<EntryNode>().gameObject;
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                ExitNode[] exitList = m_selected.GetComponentsInChildren<ExitNode>();
                canSelect = EditorGUILayout.IntField("Contains Exit Nodes", exitList.Length, GUILayout.Width(200)) > 0;
                isValid = canSelect && isValid;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Transform parent = Selection.activeGameObject.transform;
                    Selection.activeGameObject = new GameObject();
                    Selection.activeGameObject.name = "ExitNode";
                    Selection.activeGameObject.transform.SetParent(parent);
                    Selection.activeGameObject.transform.localPosition = Vector3.zero;
                    Selection.activeGameObject.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    Selection.activeGameObject.AddComponent<ExitNode>();
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Object[] selectList = new Object[exitList.Length];
                    for (int i = 0; i < exitList.Length; i++)
                    {
                        selectList[i] = exitList[i].gameObject;
                    }
                    Selection.objects = selectList;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                ColliderNode[] colliderList = m_selected.GetComponentsInChildren<ColliderNode>();
                canSelect = EditorGUILayout.IntField("Contains Level Colliders", colliderList.Length, GUILayout.Width(200)) > 0;
                isValid = canSelect && isValid;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject.AddComponent<ColliderNode>();
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Object[] selectList = new Object[colliderList.Length];
                    for (int i = 0; i < colliderList.Length; i++)
                    {
                        selectList[i] = colliderList[i].gameObject;
                    }
                    Selection.objects = selectList;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("No game object selected.");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (isValid)
            {
                GUILayout.Label("Is a valid section prefab", m_confirmText);
            }
            else
            {
                GUILayout.Label("Is not a valid section prefab", m_declineText);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Back", GUILayout.Height(50)))
            {
                m_currentPrefabWindow = PrefabWindow.NULL;
            }
        }

        private void CapWindow()
        {
            HyperLinkHeader();

            LabelSelected();
            bool isValid = false;
            if (m_selected != null)
            {
                bool canSelect = false;
                EditorGUILayout.BeginHorizontal();
                canSelect = EditorGUILayout.Toggle("Contains a Prefab Section", m_selected.GetComponent<PrefabSection>(), GUILayout.Width(200));
                isValid = canSelect;
                GUI.enabled = !canSelect;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject = m_selected.AddComponent<PrefabSection>().gameObject;
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject = m_selected;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                canSelect = EditorGUILayout.Toggle("Contains a Entry Node", m_selected.GetComponentInChildren<EntryNode>(), GUILayout.Width(200));
                isValid = isValid && canSelect;
                GUI.enabled = !canSelect;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject = new GameObject();
                    Selection.activeGameObject.name = "EntryNode";
                    Selection.activeGameObject.transform.SetParent(m_selected.transform);
                    Selection.activeGameObject.transform.localPosition = Vector3.zero;
                    Selection.activeGameObject.transform.localRotation = Quaternion.identity;
                    Selection.activeGameObject.AddComponent<EntryNode>();
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeObject = m_selected.GetComponentInChildren<EntryNode>().gameObject;
                }
                EditorGUILayout.EndHorizontal();
                GUI.enabled = true;

                EditorGUILayout.BeginHorizontal();
                ExitNode[] exitList = m_selected.GetComponentsInChildren<ExitNode>();
                canSelect = EditorGUILayout.Toggle("Contains NO Exit Nodes", exitList.Length == 0, GUILayout.Width(200));
                isValid = isValid && canSelect;
                GUI.enabled = !canSelect;
                if (GUILayout.Button("Clean", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    for (int i = exitList.Length - 1; i >= 0; i--)
                    {
                        if (exitList[i].gameObject.name == "ExitNode")
                            exitList[i].gameObject.name = "CleanedNode";

                        DestroyImmediate(exitList[i]);
                    }
                }
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Object[] selectList = new Object[exitList.Length];
                    for (int i = 0; i < exitList.Length; i++)
                    {
                        selectList[i] = exitList[i].gameObject;
                    }
                    Selection.objects = selectList;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                ColliderNode[] colliderList = m_selected.GetComponentsInChildren<ColliderNode>();
                canSelect = EditorGUILayout.IntField("Contains Level Colliders", colliderList.Length, GUILayout.Width(200)) > 0;
                isValid = isValid && canSelect;
                if (GUILayout.Button("Create", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Selection.activeGameObject.AddComponent<ColliderNode>();
                }
                GUI.enabled = canSelect;
                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.2f - 2)))
                {
                    Object[] selectList = new Object[colliderList.Length];
                    for (int i = 0; i < colliderList.Length; i++)
                    {
                        selectList[i] = colliderList[i].gameObject;
                    }
                    Selection.objects = selectList;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("No game object selected.");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (isValid)
            {
                GUILayout.Label("Is a valid cap prefab", m_confirmText);
            }
            else
            {
                GUILayout.Label("Is not a valid cap prefab", m_declineText);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Back", GUILayout.Height(50)))
            {
                m_currentPrefabWindow = PrefabWindow.NULL;
            }
        }

        private void HyperLinkHeader()
        {
            GUILayout.BeginHorizontal();

            if(m_currentWindow != Window.MAIN)
            {
                if (GUILayout.Button("Main", m_boldLinkStyle, GUILayout.Width(m_boldLinkStyle.CalcSize(new GUIContent("Main ")).x)))
                {
                    m_currentWindow = Window.MAIN;
                    m_currentPrefabWindow = PrefabWindow.NULL;
                }
                GUILayout.Label("> ", m_normalStyle, GUILayout.Width(m_normalStyle.CalcSize(new GUIContent("> ")).x));
            }

            switch (m_currentWindow)
            {
                default:
                    break;
                case Window.START:
                    {
                        GUILayout.Label("Level Generator", m_normalStyle, GUILayout.Width(m_normalStyle.CalcSize(new GUIContent("Level Generator")).x));
                        break;
                    }
                case Window.PREFAB:
                    {
                        if(m_currentPrefabWindow != PrefabWindow.NULL)
                        {
                            if (GUILayout.Button("Prefab Tools", m_boldLinkStyle, GUILayout.Width(m_boldLinkStyle.CalcSize(new GUIContent("Prefab Tools ")).x)))
                            {
                                m_currentPrefabWindow = PrefabWindow.NULL;
                            }
                            GUILayout.Label("> ", m_normalStyle, GUILayout.Width(m_normalStyle.CalcSize(new GUIContent("> ")).x));
                        }

                        switch (m_currentPrefabWindow)
                        {
                            default:
                            case PrefabWindow.NULL:
                                GUILayout.Label("Prefab Tools", m_normalStyle, GUILayout.Width(m_normalStyle.CalcSize(new GUIContent("Prefab Tools")).x));
                                break;
                            case PrefabWindow.SECTION:
                                GUILayout.Label("Section", m_normalStyle, GUILayout.Width(m_normalStyle.CalcSize(new GUIContent("Section ")).x));
                                break;
                            case PrefabWindow.CAP:
                                GUILayout.Label("Cap", m_normalStyle, GUILayout.Width(m_normalStyle.CalcSize(new GUIContent("Cap ")).x));
                                break;
                        }
                        break;
                    }                
            }
            GUILayout.EndHorizontal();

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        private void LabelSelected()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Object:", EditorStyles.boldLabel, GUILayout.Width(EditorStyles.boldLabel.CalcSize(new GUIContent("Selected Object:")).x));
            GUI.enabled = false;
            EditorGUILayout.TextField((m_selected != null) ? m_selected.name : "null", GUILayout.Width(position.width - EditorStyles.boldLabel.CalcSize(new GUIContent("Selected Object:")).x - 5));
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }


        private GameObject ValidatePrefab(GameObject prefabIn, int EnteranceStatus, int ExitStatus)
        {
            if (prefabIn == null)
                return null;

            EntryNode entry = prefabIn.GetComponentInChildren<EntryNode>();
            ExitNode exit = prefabIn.GetComponentInChildren<ExitNode>();

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
                if (item == null || item.GetComponentInChildren<EntryNode>() == null)
                {
                    return false;
                }
            }

            return m_endCapPrefab != null && m_sectionPrefabs.Count != 0;
        }
    }

}
