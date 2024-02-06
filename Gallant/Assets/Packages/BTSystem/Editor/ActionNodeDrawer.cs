using UnityEngine;
using XNodeEditor;
using BTSystem.Nodes;
using UnityEditor;
using System.Linq;
using BTSystem.Core;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(ActionNode))]
    public class ActionNodeDrawer : BaseNodeDrawer
    {
        public ActionNodeDrawer()
        {
            ColorUtility.TryParseHtmlString("#f5a742", out tint);
        }

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
        }
    }
}