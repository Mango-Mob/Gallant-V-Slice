using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    /**
     * An Inspector element for exit nodes to display the hits, if they occured.
     * @author : Michael Jordan
     */
    [CustomEditor(typeof(ExitNode))]
    class ExitEditor : UnityEditor.Editor
    {
        /**
         * OnInspectorGUI function.
         * Draws when an error node is on the inspector.
         */
        public override void OnInspectorGUI()
        {
            var node = target as ExitNode;

            DrawDefaultInspector();

            if (node.transform.childCount > 0)
                GUILayout.Label("Note: Any objects attached to this object will be deleted upon selection.");
        }
    }
}
