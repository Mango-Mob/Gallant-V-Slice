using UnityEngine;
using UnityEditor;
using XNodeEditor;
using BTSystem.Core;
using BTSystem.Nodes.Flow;
using System.Linq;
using BTSystem.Interfaces;
using BTSystem.Nodes;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(StatusNode))]
    public class StatusNodeDrawer : NodeEditor
    {
        private GUIStyle style;

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            var color = Color.red;

            if ((target.graph as BTGraph).currentNodes != null && (target.graph as BTGraph).currentNodes.Count > 0)
                color = (target.graph as BTGraph).currentNodes.Contains(target) ? Color.green : Color.red;

            EditorGUI.DrawRect(new Rect(6.5f, 28, GetWidth() - 11.5f, 6), color);
        }

        public override void OnBodyGUI()
        {
            if (style == null)
                style = new GUIStyle(EditorStyles.label);

            EditorStyles.label.normal.textColor = Color.black;

            EditorGUILayout.BeginHorizontal();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("decoration"));
            EditorGUILayout.EndHorizontal();

            string[] excludes = { "m_Script", "graph", "position", "ports", "entry", "decoration" };
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator, true);
            }

            if (target is SubGraphNode && (target as SubGraphNode).SubGraph != null)
            {
                if (GUILayout.Button("Graph"))
                {
                    NodeEditorWindow.Open((target as SubGraphNode).SubGraph);
                }
            }

            EditorStyles.label.normal = style.normal;
        }
    }
}
