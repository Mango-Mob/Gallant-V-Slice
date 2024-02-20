using UnityEngine;
using UnityEditor;
using XNodeEditor;
using BTSystem.Nodes;
using System.Linq;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(DecoratorNode))]
    public class DecoratorNodeDrawer : BaseNodeDrawer
    {
        public DecoratorNodeDrawer()
        {
            ColorUtility.TryParseHtmlString("#39A2E4", out tint);
        }
    }
}