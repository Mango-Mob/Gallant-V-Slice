using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    [CustomEditor(typeof(ExitNode))]
    class ExitEditor : UnityEditor.Editor
    {
        public void Awake()
        {

        }

        public override void OnInspectorGUI()
        {
            var node = target as ExitNode;

            DrawDefaultInspector();

            if (node.transform.childCount > 0)
                GUILayout.Label("Note: Any objects attached to this object will be deleted upon selection.");
        }
    }
}
