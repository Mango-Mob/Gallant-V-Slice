using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    /**
     * An Inspector element for error nodes to display the hits, if they occured.
     * @author : Michael Jordan
     */
    [CustomEditor(typeof(ErrorNode))]
    class ErrorEditor : UnityEditor.Editor
    {
        /** a private variable. 
         * Status of the foldout.
         */
        private bool foldout = false;

        /** a private variable. 
         * Scroll position.
         */
        private Vector2 scrollPos = Vector2.zero;

        /**
         * OnInspectorGUI function.
         * Draws when an error node is on the inspector.
         */
        public override void OnInspectorGUI()
        {
            var node = target as ErrorNode;
            DrawDefaultInspector();
            GUILayout.Label("Note: Disable me at the editor window: Level Generator");

            GUILayout.Label($"Hit count: {node.m_hits.Count}");

            if(node.m_hits.Count > 0)
                foldout = EditorGUILayout.Foldout(foldout, "Hits:");

            if(foldout && node.m_hits.Count > 0)
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                foreach (var hit in node.m_hits)
                {
                    GUILayout.Label($" > Hit: {hit.name}");
                }

                GUILayout.EndScrollView();
            }
        }
    }
}
