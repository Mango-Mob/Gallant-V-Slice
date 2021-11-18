using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public enum DialogResult
{
    PROGRESS,
    TRANSFER,
    INTERACT,
    END
}

public class DialogWriter : EditorWindow
{
    private TextAsset m_currentFile = null;
    private DialogFile m_fileData;

    private CharacterData m_character;

    private string m_activeDialog;

    private int m_bodyIndex = 0;
    private int m_faceIndex = 0;

    private int m_currentIndex;
    private int m_maxIndex;

    private bool m_displayFoldout = false;
    private Vector2 m_scrollPos; 

    public List<DialogOption> m_options;

    [MenuItem("Tools/Dialog Writer")]
    public static void Init()
    {
        EditorWindow.CreateWindow<DialogWriter>("Dialog Writer");
    }
    public void Awake()
    {
        m_currentFile = null;
        m_options = new List<DialogOption>();
    }
    private void Update()
    {
        
    }
    private void OnGUI()
    {
        if(m_fileData == null)
        {
            TextAsset current = EditorGUILayout.ObjectField("Load File: ", m_currentFile, typeof(TextAsset), true) as TextAsset;
            if(TryLoadFile(current))
            {
                
            }
        }
        else
        {
            if (m_currentFile == null)
                return;

            EditorGUILayout.LabelField($"Load File: {m_currentFile.name}");

            if (m_fileData != null)
            {
                m_character = EditorGUILayout.ObjectField("Character Data: ", m_character, typeof(CharacterData), true) as CharacterData;

                GUI.enabled = m_maxIndex > 0;
                if (m_character != null)
                {
                    EditorGUILayout.LabelField(m_character.m_name);
                    m_character?.DrawToGUI(new Rect(0, 40, 150, 150), m_bodyIndex, m_faceIndex);
                }
                else
                {
                    EditorGUILayout.LabelField("NullCharacter");
                }
                m_activeDialog = GUI.TextArea(new Rect(95, 45, position.width - 100, 150), m_activeDialog);

                GUILayout.Space(125);
                m_character?.DrawSliders(ref m_bodyIndex, ref m_faceIndex);
                EditorGUILayout.LabelField($"Index: {m_currentIndex + 1} (of {m_maxIndex})");

                GUILayout.BeginHorizontal();
                m_displayFoldout = EditorGUILayout.Foldout(m_displayFoldout, "Player options");
                EditorGUILayout.Space();
                int count = EditorGUILayout.IntField(m_options != null ? m_options.Count : 0);
                GUILayout.EndHorizontal();

                if (m_options == null && count >= 0)
                    m_options = new List<DialogOption>();

                while (count > m_options.Count)
                    m_options.Add(new DialogOption(m_currentIndex));
                while (count < m_options.Count) 
                    m_options.RemoveAt(m_options.Count - 1);

                if(m_displayFoldout)
                {
                    m_scrollPos = EditorGUILayout.BeginScrollView(m_scrollPos);
                    for (int i = 0; i < m_options.Count; i++)
                    {
                        EditorGUILayout.LabelField($"Option {i}:");
                        m_options[i].text = EditorGUILayout.TextField(m_options[i].text);
                        m_options[i].result = (DialogResult)EditorGUILayout.EnumPopup(m_options[i].result);

                        if (m_options[i].result == DialogResult.TRANSFER)
                        {
                            m_options[i].nextDialog = EditorGUILayout.IntField("Transfer to:", m_options[i].nextDialog);
                        }
                        Rect rect = EditorGUILayout.GetControlRect(false, 1);
                        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
                    }
                    EditorGUILayout.EndScrollView();
                }
                
                GUILayout.BeginHorizontal();
                GUI.enabled = m_currentIndex > 0;
                if (GUILayout.Button("Prev", GUILayout.Width((position.width - 15) / 4)))
                {
                    SaveItem();
                    m_currentIndex--;
                    LoadItem(m_currentIndex);
                }
                GUI.enabled = m_currentIndex < m_maxIndex - 1;
                if (GUILayout.Button("Next", GUILayout.Width((position.width - 15) / 4)))
                {
                    SaveItem();
                    m_currentIndex++;
                    LoadItem(m_currentIndex);
                }
                GUI.enabled = true;
                if (GUILayout.Button("New", GUILayout.Width((position.width - 15) / 4)))
                {
                    if (m_currentIndex >= 0)
                        SaveItem();

                    m_currentIndex = m_maxIndex;
                    m_fileData.AddNewEntry();
                    m_maxIndex = m_fileData.m_list.Count;
                    LoadItem(m_currentIndex);
                }
                if (GUILayout.Button("Delete", GUILayout.Width((position.width - 15) / 4)))
                {
                    m_fileData.RemoveEntry(m_currentIndex);
                    m_maxIndex = m_fileData.m_list.Count;
                    LoadItem(m_currentIndex);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Save", GUILayout.Width((position.width - 5))))
            {
                if (m_currentIndex >= 0)
                    SaveItem();

                SaveFile(m_currentFile);
            }
            if (GUILayout.Button("Close file and save", GUILayout.Width((position.width - 5))))
            {
                if (m_currentIndex >= 0)
                    SaveItem();

                SaveFile(m_currentFile);
                Clear();
                m_currentFile = null;
            }
        }

        if (GUILayout.Button("Create blank file", GUILayout.Width((position.width - 5))))
        {
            string path = Application.dataPath + "/NewDialog.json";
            File.CreateText(path);
        }
    }

    private void SaveFile(TextAsset currentFile)
    {
        DateTime start = DateTime.Now;
        SaveItem();
        if (m_character != null)
        {
            m_fileData.m_characterFile = "Data/Characters/" + m_character.name;
        }

        File.WriteAllText(AssetDatabase.GetAssetPath(m_currentFile), JsonUtility.ToJson(m_fileData));

        Debug.Log($"Saved dialog file {m_currentFile.name} in: {(DateTime.Now - start).TotalMilliseconds}ms");
    }

    public bool TryLoadFile(TextAsset file)
    {
        if (file == null)
            return false;

        DateTime start = DateTime.Now;

        if (file.text.Length == 0)
        {
            //Fresh file
            m_fileData = new DialogFile();
            return true;
        }
        m_fileData = JsonUtility.FromJson<DialogFile>(file.text);
        if(m_fileData == null)
        {
            //Other file
            Debug.LogError($"Failed to load dialog file: {file.name}");
            return false;
        }
        m_character = Resources.Load<CharacterData>(m_fileData.m_characterFile);

        m_currentIndex = 0;
        m_maxIndex = m_fileData.m_list.Count;
        LoadItem(m_currentIndex);

        m_currentFile = file;
        Debug.Log($"Loaded dialog file {m_currentFile.name} in: {(DateTime.Now - start).TotalMilliseconds}ms");
        return true;
    }

    public void LoadItem(int index)
    {
        if(index < m_maxIndex)
        {
            m_bodyIndex = m_fileData.m_list[index].bodyID;
            m_faceIndex = m_fileData.m_list[index].faceID;
            m_activeDialog = m_fileData.m_list[index].m_dialog;
            m_options.Clear();

            for (int i = 0; i < m_fileData.m_list[m_currentIndex].results.Count; i++)
            {
                m_options.Add(new DialogOption(
                    index, 
                    m_fileData.m_list[m_currentIndex].results[i].resultText,
                    m_fileData.m_list[m_currentIndex].results[i].resultType,
                    m_fileData.m_list[m_currentIndex].results[i].other
                    ));
            }
        }
    }

    public void SaveItem()
    {
        m_fileData.m_list[m_currentIndex].bodyID = m_bodyIndex;
        m_fileData.m_list[m_currentIndex].faceID = m_faceIndex;
        m_fileData.m_list[m_currentIndex].m_dialog = m_activeDialog;

        //Save player options
        m_fileData.m_list[m_currentIndex].SetResultSize(m_options.Count);

        for (int i = 0; i < m_options.Count; i++)
        {
            m_fileData.m_list[m_currentIndex].results[i].resultText = m_options[i].text;
            m_fileData.m_list[m_currentIndex].results[i].resultType = m_options[i].GetTypeText();
            m_fileData.m_list[m_currentIndex].results[i].other = (m_options[i].nextDialog - 1).ToString();
        }
    }

    public void Clear()
    {
        m_fileData = null;
        m_currentIndex = -1;
        m_maxIndex = -1;
        m_activeDialog = "";
    }
}

public class DialogOption
{
    public string text;
    public int nextDialog;
    public DialogResult result;

    public DialogOption(int currentScene)
    {
        text = "";
        nextDialog = -1;
        result = DialogResult.END;
    }

    public DialogOption(int currentScene, string _text, string _result, string other)
    {
        text = _text;
        switch (_result)
        {
            case "PROGRESS":
                result = DialogResult.PROGRESS;
                nextDialog = currentScene + 1;
                return;
            case "END":
                result = DialogResult.END;
                nextDialog = -1;
                return;
            case "INTERACT":
                result = DialogResult.INTERACT;
                nextDialog = -1;
                return;
            case "TRANSFER":
                result = DialogResult.TRANSFER;
                nextDialog = int.Parse(other) + 1;
                return;
            default:
                result = DialogResult.END;
                nextDialog = -1;
                return;
        }
    }

    public string GetTypeText()
    {
        switch (result)
        {
            case DialogResult.PROGRESS:
                return "PROGRESS";
            case DialogResult.TRANSFER:
                return "TRANSFER";
            case DialogResult.INTERACT:
                return "INTERACT";
            case DialogResult.END:
                return "END";
            default:
                return "";
        }
    }
}