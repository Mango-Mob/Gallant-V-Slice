using GEN.Nodes;
using UnityEditor;
using UnityEngine;

namespace GEN.Editor
{
    [CustomEditor(typeof(EntryNode))]
    class EntryEditor : UnityEditor.Editor
    {
        public void Awake()
        {

        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
