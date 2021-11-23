using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    /**
     * An Inspector element for start nodes to display the hits, if they occured.
     * @author : Michael Jordan
     */
    [CustomEditor(typeof(StartNode))]
    class StartEditor : UnityEditor.Editor
    {
        /** a private variable. 
         * Preview node used to display a preview of the level.
         */
        private PreviewNode m_preview;

        /** a private variable. 
         * A copy of the reference texture to display onto the inspector.
         */
        private RenderTexture m_displayTexture;

        /** a private variable. 
         * Status of the exposed settings.
         */
        private bool m_showSettings = true;

        /** a private variable. 
         * Text style for warnings.
         */
        private GUIStyle m_warning;

        /**
         * Awake function.
         * Called when the inspector is awake and showing a startNode.
         */
        private void Awake()
        {
            m_warning = new GUIStyle(EditorStyles.label);
            m_warning.wordWrap = true;
            m_warning.normal.textColor = Color.yellow;

            m_preview = PreviewNode.Instantiate((target as StartNode).transform, -Vector3.up, (target as StartNode).m_layersToCheck);
            m_preview.SetOffset(new Vector3(0, 20, 0), 150);
        }

        /**
         * OnDestroy function.
         * Called when the inspector changes/closes view of the node.
         */
        private void OnDestroy()
        {
            PreviewNode.CleanAll();
        }

        /**
         * OnInspectorGUI function.
         * Draws when an error node is on the inspector.
         */
        public override void OnInspectorGUI()
        {
            var levelStart = target as StartNode;

            //Preview section
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("~ Preview ~", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (GUILayout.Button(m_displayTexture == null ? "Show Preview" : "Hide Preview"))
            {
                //Enable the preview in the inspector
                if (m_displayTexture == null)
                    m_displayTexture = m_preview.m_texture;
                else
                    m_displayTexture = null;
            }

            if (m_displayTexture != null)
            {
                m_preview.Render();
                

                m_showSettings = EditorGUILayout.Foldout(m_showSettings, "Settings:");
                float yPos = (m_showSettings) ? 150: 70;

                GUI.DrawTexture(new Rect(15, yPos, EditorGUIUtility.currentViewWidth - 30, 270), m_displayTexture);
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

                

                GUILayout.Space(m_showSettings ? 300 : 305);
            }
            GUILayout.Space(10);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Space(10);

            //Default:
            DrawDefaultInspector();
            
            //Buttons and Warnings:
            if (levelStart.m_GenerateLevelOnAwake && levelStart.transform.childCount > 0)
            {
                rect = EditorGUILayout.GetControlRect(false, 1);
                EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

                GUILayout.Label("Warning: Do not have any objects attached to this object! They will be deleted upon play.", m_warning);
            }
                
            rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Space(10);
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
