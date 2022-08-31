using BTSystem.Core;
using UnityEditor;

namespace BTSystem.Editor
{
    [CustomEditor(typeof(BTGraph))]
    public class BTGraphEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
