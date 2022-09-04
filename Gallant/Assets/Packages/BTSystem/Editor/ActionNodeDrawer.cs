using UnityEngine;
using XNodeEditor;
using BTSystem.Nodes;
using UnityEditor;
using System.Linq;
using BTSystem.Core;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(ActionNode))]
    public class ActionNodeDrawer : NodeEditor
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
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("exit"));
            EditorGUILayout.EndHorizontal();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("decoration"));

            string[] excludes = { "m_Script", "graph", "position", "ports", "entry", "exit", "decoration" };
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator, true);
            }

            EditorStyles.label.normal = style.normal;
        }

        public override Color GetTint()
        {
            Color tint = base.GetTint();
            ColorUtility.TryParseHtmlString("#f5a742", out tint);
            return tint;
        }
    }
}