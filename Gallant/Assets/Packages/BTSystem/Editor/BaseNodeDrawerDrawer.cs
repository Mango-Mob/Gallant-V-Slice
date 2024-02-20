using UnityEngine;
using UnityEditor;
using XNodeEditor;
using BTSystem.Core;
using BTSystem.Nodes.Flow;
using System.Linq;
using BTSystem.Interfaces;
using static AmplifyShaderEditor.ParentGraph;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(BaseNode))]
    public class BaseNodeDrawer : NodeEditor
    {
        [SerializeField] public Color tint;
        [SerializeField] public Color font = Color.black;
        protected GUIStyle style, header_style;

        private string[] excludes = { "m_Script", "graph", "position", "ports", "entry", "timer", "exits", "exit", "TimerDisplay", "decoration" };

        public BaseNodeDrawer()
        {
            style = new GUIStyle();
            style.normal.textColor = font;
            style.alignment = TextAnchor.MiddleLeft;

            header_style = new GUIStyle();
            header_style.alignment = TextAnchor.MiddleCenter;
            header_style.fontStyle = FontStyle.Bold;
            header_style.normal.textColor = font;
        }

        public override void OnHeaderGUI()
        {
            Color before = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = font;

            GUILayout.Label(target.name, header_style, GUILayout.Height(30));
            var color = Color.red;

            if ((target.graph as BTGraph).currentNodes != null && (target.graph as BTGraph).currentNodes.Count > 0)
                color = (target.graph as BTGraph).currentNodes.Contains(target) ? Color.green : Color.red;

            EditorGUI.DrawRect(new Rect(6.5f, 28, GetWidth() - 11.5f, 6), color);

            EditorGUILayout.BeginHorizontal();
            var entry = target.GetInputPort("entry");
            var index = entry.GetConnections().Count > 0 ? entry.GetConnection(0).GetConnectionIndex(target.GetInputPort("entry")).ToString() : "-1";
            NodeEditorGUILayout.PortField(new GUIContent("Entry [" + index+"]"), entry);

            var multi_exit = serializedObject.FindProperty("exits");
            if (multi_exit != null)
                NodeEditorGUILayout.PropertyField(multi_exit);

            var single_exit = serializedObject.FindProperty("exit");
            if (single_exit != null)
                NodeEditorGUILayout.PropertyField(single_exit);
            EditorGUILayout.EndHorizontal();

            var decor = target.GetOutputPort("decoration");
            if (decor != null)
                NodeEditorGUILayout.PortInputField(new GUIContent("Decorations [" + decor.GetConnections().Count() + "]"), decor);

            var timer = serializedObject.FindProperty("timer");
            if (timer != null)
                NodeEditorGUILayout.PropertyField(timer);

            EditorStyles.label.normal.textColor = before;
        }

        public override void OnBodyGUI()
        {
            Color before = EditorStyles.label.normal.textColor;
            EditorStyles.label.normal.textColor = font;
            if (target is ITimer && Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUILayout.Slider((target as ITimer).GetRemainder(), 0, 1);
                GUI.enabled = true;
            }

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

            EditorStyles.label.normal.textColor = before;
        }

        public override Color GetTint()
        {
            return tint;
        }
    }
}
