using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EntitySystem.Editor
{
    public class LogicOperatorCreator
    {
        [MenuItem("Tools/Execute/CreateLogicAnimator")]
        static void CreateLogicAnimator()
        {
            AssetDatabase.CreateAsset(new AnimatorController(), "");
        }
    }
}
