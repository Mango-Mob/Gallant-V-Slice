using UnityEngine;
using XNodeEditor;
using BTSystem.Nodes;
using UnityEditor;
using BTSystem.Core;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(RootNode))]
    public class RootNodeDrawer : NodeEditor
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

            base.OnBodyGUI();

            if(GUILayout.Button("Graph"))
            {
                Selection.activeObject = target.graph;
            }

            EditorStyles.label.normal = style.normal;
        }

        public override Color GetTint()
        {
            Color tint = base.GetTint();
            ColorUtility.TryParseHtmlString("#edd142", out tint);
            return tint;
        }
    }
}