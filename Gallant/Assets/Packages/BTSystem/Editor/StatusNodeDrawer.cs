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
    public class StatusNodeDrawer : BaseNodeDrawer
    {
        public StatusNodeDrawer()
        {
            ColorUtility.TryParseHtmlString("#B0E439", out tint);
        }
    }
}
