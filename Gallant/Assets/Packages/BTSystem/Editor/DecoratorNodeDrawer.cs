using UnityEngine;
using UnityEditor;
using XNodeEditor;
using BTSystem.Nodes;
using System.Linq;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(DecoratorNode))]
    public class DecoratorNodeDrawer : NodeEditor
    {
        private GUIStyle style;
        public override void OnBodyGUI()
        {
            if (style == null)
                style = new GUIStyle(EditorStyles.label);

            EditorStyles.label.normal.textColor = Color.black;
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));

            string[] excludes = { "m_Script", "graph", "position", "ports", "entry" };
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
            ColorUtility.TryParseHtmlString("#22c7bc", out tint);
            return tint;
        }
    }
}