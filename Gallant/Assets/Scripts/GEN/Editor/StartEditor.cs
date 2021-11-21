using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    [CustomEditor(typeof(StartNode))]
    class StartEditor : UnityEditor.Editor
    {
        private PreviewNode m_preview;
        private RenderTexture m_displayTexture;

        public bool m_showSettings = true;

        private GUIStyle m_warning;

        public void Awake()
        {
            m_warning = new GUIStyle(EditorStyles.label);
            m_warning.wordWrap = true;
            m_warning.normal.textColor = Color.yellow;

            m_preview = PreviewNode.Instantiate((target as StartNode).transform, -Vector3.up, (target as StartNode).m_layersToCheck);
            m_preview.SetOffset(new Vector3(0, 20, 0), 150);
        }

        public void OnDestroy()
        {
            PreviewNode.CleanAll();
        }

        public override void OnInspectorGUI()
        {
            var levelStart = target as StartNode;

            if (m_displayTexture != null)
            {
                m_preview.Render();

                GUI.DrawTexture(new Rect(15, 15, EditorGUIUtility.currentViewWidth - 30, 270), m_displayTexture);

                GUILayout.Label("Preview:");

                if (m_showSettings)
                {
                    Vector3 posOffset = m_preview.m_positionOffset;
                    float sizeOffset = m_preview.m_sizeOffset;

                    posOffset.x = EditorGUILayout.Slider("Offset x: ", m_preview.m_positionOffset.x, -500f, 500f);
                    posOffset.y = EditorGUILayout.Slider("Offset y: ", m_preview.m_positionOffset.y, 0, 500f);
                    posOffset.z = EditorGUILayout.Slider("Offset z: ", m_preview.m_positionOffset.z, -500f, 500f);
                    sizeOffset = EditorGUILayout.Slider("    Zoom: ", m_preview.m_sizeOffset, 0, 500f);


                    if (posOffset != m_preview.m_positionOffset || sizeOffset != m_preview.m_sizeOffset)
                    {
                        m_preview.SetOffset(posOffset, sizeOffset);
                    }
                }

                GUILayout.Space(m_showSettings ? 260 : 300);
                if (GUILayout.Button(m_showSettings ? "Hide Preview Settings" : "Show Preview Settings"))
                {
                    m_showSettings = !m_showSettings;
                }
            }

            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

            DrawDefaultInspector();

            if (levelStart.m_GenerateLevelOnAwake && levelStart.transform.childCount > 0)
                GUILayout.Label("Warning: Do not have any objects attached to this object! They will be deleted upon play.", m_warning);

            if (GUILayout.Button(m_displayTexture == null ? "Show Preview" : "Hide Preview"))
            {
                //Enable the preview in the inspector
                if (m_displayTexture == null)
                    m_displayTexture = m_preview.m_texture;
                else
                    m_displayTexture = null;
            }

            if (GUILayout.Button("Generate"))
            {
                levelStart.Clear();
                if (levelStart.m_GenerateSeedOnAwake)
                    levelStart.Generate();
                else
                    levelStart.Generate(levelStart.m_seed);

                //Reset preview position
                m_preview.m_positionInitial = (target as StartNode).CalculateAveragePosition();
            }

            if (GUILayout.Button("Clear"))
            {
                levelStart.Clear();
                m_displayTexture = null;

                //Reset preview position
                m_preview.m_positionInitial = (target as StartNode).transform.position;
            }
        }
    }
}
