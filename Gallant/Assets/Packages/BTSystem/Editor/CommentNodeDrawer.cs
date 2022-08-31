using UnityEngine;
using UnityEditor;
using BTSystem.Core;
using XNodeEditor;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(CommentNode))]
    public class CommentNodeDrawer : NodeEditor
    {
        private int height;
        private int width;
        private bool show = false;

        public override void OnCreate()
        {
            base.OnCreate();
            width = Mathf.Clamp(Mathf.FloorToInt((target as CommentNode).Size.x), base.GetWidth(), int.MaxValue);
            height = Mathf.Clamp(Mathf.FloorToInt((target as CommentNode).Size.y), 0, int.MaxValue);
        }

        public override void OnBodyGUI()
        {
            var before = GUI.contentColor;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.black;

            EditorGUILayout.Space(height);
            show = EditorGUILayout.Foldout(show, "Show Editor Information");

            if (show)
            {
                target.name = GUILayout.TextArea(target.name);
                this.width = Mathf.Clamp(EditorGUILayout.IntField("Width:", this.width), base.GetWidth(), int.MaxValue);
                this.height = Mathf.Clamp(EditorGUILayout.IntField("Height:", this.height), 0, int.MaxValue);
            }

            (target as CommentNode).Size = new Vector2(this.width, this.height);

            GUI.contentColor = before;
            GUI.backgroundColor = before;
        }

        public override int GetWidth()
        {
            return width;
        }

        public override Color GetTint()
        {
            return new Color(1, 1, 1, 0.25f);
        }
    }
}