using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;



public class DialogWriter : EditorWindow
{
    private string m_currentPath = "";
    private DialogFile m_fileData;

    private CharacterData m_character;

    private string m_activeDialog;

    private int m_bodyIndex = 0;
    private int m_faceIndex = 0;
    private int m_nameIndex = 0;

    private int m_currentIndex;
    private int m_maxIndex;

    private bool m_displayFoldout = false;
    private Vector2 m_scrollPos; 

    public List<DialogOption> m_options;

    private GUIStyle m_wrapLabel;
    [MenuItem("Tools/Dialog Writer")]
    public static void Init()
    {
        EditorWindow.CreateWindow<DialogWriter>("Dialog Writer");
    }

    public void Awake()
    {
        m_currentPath = "";
        m_options = new List<DialogOption>();

        m_wrapLabel = EditorStyles.label;
        m_wrapLabel.wordWrap = true;
    }

    private void Update()
    {
        
    }

    private void OnGUI()
    {
        if(m_fileData == null)
        {
            InitialGUI();
            return;
        }
        else
        {
            MainGUI();
            return;
        }
    }

    private void Header()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("New  "))
        {
            string path = Application.dataPath + $"/Dialog/NewDialog-{DateTime.Now.ToString("yyyyMMddTHHmmss")}.json";
            StreamWriter sw = File.CreateText(path);
            sw.Close();
            AssetDatabase.Refresh();
            if (TryLoadFile(path))
            {

            }
        }
        if (GUILayout.Button("Load "))
        {
            string path = EditorUtility.OpenFilePanel("Select dialog", "Assets/Dialog/", "json");
            if (TryLoadFile(path))
            {

            }
        }
        GUI.enabled = m_fileData != null;
        if (GUILayout.Button("Save "))
        {
            SaveFile();
        }
        GUI.enabled = m_fileData != null;
        if (GUILayout.Button("Close"))
        {
            m_fileData = null;
            m_currentPath = "";
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        EditorExtentions.DrawLineOnGUI();
    }

    private void InitialGUI()
    {
        Header();
    }

    private void MainGUI()
    {
        Header();

        m_character = EditorGUILayout.ObjectField("Character Data: ", m_character, typeof(CharacterData), true) as CharacterData;
        
        GUI.enabled = m_maxIndex > 0;
        if (m_character != null)
        {
            EditorGUILayout.LabelField(m_character.m_names[m_nameIndex], EditorStyles.boldLabel);
            m_character?.DrawToGUI(new Rect(0, 60, 150, 150), m_bodyIndex, m_faceIndex);
        }
        else
        {
            EditorGUILayout.LabelField("NullCharacter");
        }
        m_activeDialog = GUI.TextArea(new Rect(95, 65, position.width - 98, 150), m_activeDialog);
        
        GUILayout.Space(140);

        if (m_character != null)
        {
            m_nameIndex = EditorGUILayout.IntSlider("Name:", m_nameIndex, 0, m_character.m_names.Length - 1);
            m_bodyIndex = EditorGUILayout.IntSlider("Body:", m_bodyIndex, 0, m_character.m_characterBody.Length - 1);
            m_faceIndex = EditorGUILayout.IntSlider("Face:", m_faceIndex, 0, m_character.m_characterFace.Length - 1);
        }
        
        EditorGUILayout.LabelField($"Index: {m_currentIndex + 1} (of {m_maxIndex})");
        EditorExtentions.DrawLineOnGUI();

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
                else if(m_options[i].result == DialogResult.INTERACT)
                {
                    m_options[i].interact = EditorGUILayout.IntField("Event:", m_options[i].interact);
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
            LoadItem((m_maxIndex == m_currentIndex) ? --m_currentIndex : m_currentIndex);
        }
        GUILayout.EndHorizontal();
    }

    private void SaveFile()
    {
        if(m_currentPath != "")
        {
            DateTime start = DateTime.Now;
            SaveItem();
            if (m_character != null)
            {
                m_fileData.m_characterFile = "Data/Characters/" + m_character.name;
            }
            StreamWriter sw = new StreamWriter(m_currentPath);
            sw.Write(JsonUtility.ToJson(m_fileData, true));
            sw.Close();
            Debug.Log($"Saved dialog file {m_currentPath} in: {(DateTime.Now - start).TotalMilliseconds}ms");
        }

    }

    public bool TryLoadFile(string filePath)
    {
        if (filePath == "")
            return false;

        DateTime start = DateTime.Now;
        StreamReader sr = new StreamReader(filePath);
        string data = sr.ReadToEnd();
        sr.Close();

        if (data == "")
        {
            //Fresh file
            m_fileData = new DialogFile();
            return true;
        }
        m_fileData = JsonUtility.FromJson<DialogFile>(data);
        if(m_fileData == null)
        {
            //Other file
            Debug.LogError($"Failed to load dialog file: {filePath}");
            return false;
        }
        m_character = Resources.Load<CharacterData>(m_fileData.m_characterFile);

        m_currentIndex = 0;
        m_maxIndex = m_fileData.m_list.Count;
        LoadItem(m_currentIndex);

        m_currentPath = filePath;
        Debug.Log($"Loaded dialog file {m_currentPath} in: {(DateTime.Now - start).TotalMilliseconds}ms");
        return true;
    }

    public void LoadItem(int index)
    {
        if(index < m_maxIndex)
        {
            m_nameIndex = m_fileData.m_list[index].nameID;
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
        m_fileData.m_list[m_currentIndex].nameID = m_nameIndex;
        m_fileData.m_list[m_currentIndex].bodyID = m_bodyIndex;
        m_fileData.m_list[m_currentIndex].faceID = m_faceIndex;
        m_fileData.m_list[m_currentIndex].m_dialog = m_activeDialog;

        //Save player options
        m_fileData.m_list[m_currentIndex].SetResultSize(m_options.Count);

        for (int i = 0; i < m_options.Count; i++)
        {
            m_fileData.m_list[m_currentIndex].results[i].resultText = m_options[i].text;
            m_fileData.m_list[m_currentIndex].results[i].resultType = m_options[i].GetTypeText();
            
            if (m_fileData.m_list[m_currentIndex].results[i].resultType == "INTERACT")
            {
                m_fileData.m_list[m_currentIndex].results[i].other = (m_options[i].interact).ToString();
            }
            else
            {
                m_fileData.m_list[m_currentIndex].results[i].other = (m_options[i].nextDialog - 1).ToString();
            }
            
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