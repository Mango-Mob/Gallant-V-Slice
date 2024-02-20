using UnityEngine;
using UnityEditor;
using XNodeEditor;
using BTSystem.Core;
using BTSystem.Nodes.Flow;
using System.Linq;
using BTSystem.Interfaces;

namespace BTSystem.Editor
{
    [CustomNodeEditor(typeof(FlowNode))]
    public class FlowNodeDrawer : BaseNodeDrawer
    {
        public FlowNodeDrawer()
        {
            ColorUtility.TryParseHtmlString("#02D6A9", out tint);
        }
    }
}
