using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    [CustomEditor(typeof(ErrorNode))]
    class ErrorEditor : UnityEditor.Editor
    {
        public void Awake()
        {

        }

        public override void OnInspectorGUI()
        {
            var node = target as ExitNode;
            DrawDefaultInspector();
            GUILayout.Label("Note: Disable me at the editor window: Level Generator");
        }
    }
}
