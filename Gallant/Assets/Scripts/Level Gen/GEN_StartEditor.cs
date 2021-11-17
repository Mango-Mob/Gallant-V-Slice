using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GEN_LevelStart))]
class GEN_StartEditor : Editor
{
    private Camera m_previewCamera;
    private Texture2D m_displayTexture;

    private Vector3 m_offset = new Vector3(0, 50, 0);
    public bool m_showSettings = true;
    public void Awake()
    {
        GameObject m_cam = new GameObject();
        m_previewCamera = m_cam.AddComponent<Camera>();
        m_cam.name = "EditorCamera";
        m_previewCamera.hideFlags = HideFlags.HideAndDontSave;
        m_previewCamera.transform.position = (target as GEN_LevelStart).transform.position;
        m_previewCamera.transform.position += (target as GEN_LevelStart).transform.up * 50.0f;
        m_previewCamera.transform.forward = Vector3.down;
        m_previewCamera.orthographic = true;
        m_previewCamera.orthographicSize = 150;
    }

    public void OnDestroy()
    {
        DestroyImmediate(m_previewCamera.gameObject);
    }
    public override void OnInspectorGUI()
    {       
        var levelStart = target as GEN_LevelStart;

        if(m_displayTexture != null)
        {
            GUI.DrawTexture(new Rect(15, 15, EditorGUIUtility.currentViewWidth - 30, 270), m_displayTexture);

            GUILayout.Label("Preview:");

            if(m_showSettings)
            {
                m_offset.x = EditorGUILayout.Slider("Offset x: ", m_offset.x, -500f, 500f);
                m_previewCamera.orthographicSize = EditorGUILayout.Slider("Height: ", m_previewCamera.orthographicSize, 0, 500f);
                m_offset.z = EditorGUILayout.Slider("Offset z: ", m_offset.z, -500f, 500f);
            }
            
            GUILayout.Space(m_showSettings ? 260 : 300);
            if(GUILayout.Button(m_showSettings ? "Hide Preview Settings" : "Show Preview Settings"))
            {
                m_showSettings = !m_showSettings;
            }
        }

        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        DrawDefaultInspector();

        if (GUILayout.Button(m_displayTexture == null ? "Show Preview" : "Hide Preview"))
        {
            if(m_displayTexture == null)
            {
                m_previewCamera.transform.position = (target as GEN_LevelStart).CalculateAveragePosition() + m_offset;
                m_previewCamera.targetTexture = new RenderTexture((int)1920, (int)1080, 24, RenderTextureFormat.ARGB32);

                m_previewCamera.Render();

                RenderTexture.active = m_previewCamera.targetTexture;

                m_displayTexture = new Texture2D(m_previewCamera.targetTexture.width, m_previewCamera.targetTexture.height, TextureFormat.ARGB32, false);
                m_displayTexture.ReadPixels(new Rect(0, 0, m_previewCamera.targetTexture.width, m_previewCamera.targetTexture.height), 0, 0);
                m_displayTexture.Apply();

                RenderTexture.active = null;
            }
            else
            {
                m_displayTexture = null;
            }
        }
        if (GUILayout.Button("Generate"))
        {
            levelStart.Clear();
            if (levelStart.m_GenerateSeedOnAwake)
                levelStart.Generate();
            else
                levelStart.Generate(levelStart.m_seed);
        }
        if(GUILayout.Button("Clear"))
        {
            levelStart.Clear();
            m_displayTexture = null;
        }
    }

    public IEnumerator<int> Preview()
    { 

        yield return 0;
    }
}
