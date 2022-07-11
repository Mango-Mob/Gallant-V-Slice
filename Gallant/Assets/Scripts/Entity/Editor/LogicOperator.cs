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
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(AssetDatabase.GenerateUniqueAssetPath("Assets/LogicAnimator/StateMachine.controller"));

            controller.AddParameter("IsDead", AnimatorControllerParameterType.Bool);
            controller.AddParameter("DistToTarget", AnimatorControllerParameterType.Float);
            controller.AddParameter("Health%", AnimatorControllerParameterType.Float);
            controller.AddParameter("Stamina%", AnimatorControllerParameterType.Float);

        }
    }
}
