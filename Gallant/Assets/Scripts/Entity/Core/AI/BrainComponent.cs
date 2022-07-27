
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace EntitySystem.Core.AI
{
    public class BrainComponent : CoreComponent
    {
        protected Animator LogicAnimator;

        public BrainComponent(AIEntity _owner) : base(_owner)
        {
            LogicAnimator = _owner.GetComponent<Animator>();
        }

        public override void Update(float deltaTime)
        {
            UpdateParam();
        }

        private void UpdateParam()
        {
            if (HasParameter("IsDead", AnimatorControllerParameterType.Bool, true))
            {
                LogicAnimator.SetBool("IsDead", Owner.IsDead);
            }
            if (HasParameter("DistToTarget", AnimatorControllerParameterType.Float, true))
            {
                LogicAnimator.SetFloat("DistToTarget", (!Owner.Target) ? -1 : Vector3.Distance(Owner.transform.position, Owner.Target.transform.position));
            }
            if (HasParameter("Health%", AnimatorControllerParameterType.Float, true))
            {
                LogicAnimator.SetFloat("Health%", Owner.HP/Owner.DataOnLoad.HP);
            }
            if (HasParameter("Stamina%", AnimatorControllerParameterType.Float, true))
            {
                LogicAnimator.SetFloat("Stamina%", Owner.Stamina / Owner.DataOnLoad.Stamina);
            }
            if (HasParameter("NxtAtk", AnimatorControllerParameterType.Int, true))
            {
                LogicAnimator.SetInteger("NxtAtk", Owner.Attack.GetNextAttack());
            }
        }

        /// <summary>
        /// Checks the animator params, for if a specific one exists.
        /// </summary>
        /// <param name="_name">name of the parameter to test for, in the animator.</param>
        /// <param name="_paramType">type of the parameter</param>
        /// <param name="_createIfEmpty">if param should be created if it doesn't exist.</param>
        /// <returns>if the parameter exists.</returns>
        public bool HasParameter(string _name, AnimatorControllerParameterType _paramType, bool _createIfEmpty = false)
        {
            if (LogicAnimator == null)
                return false;

            foreach (var param in LogicAnimator.parameters)
            {
                if (param.name == _name)
                {
                    return param.type == _paramType;
                }
            }

#if UNITY_EDITOR
            if(_createIfEmpty)
            {
                Debug.LogWarning($"Param {_name} not found, Added to {LogicAnimator.runtimeAnimatorController.name }!");
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>($"Assets/LogicAnimator/" + LogicAnimator.runtimeAnimatorController.name + ".controller");
                foreach (var item in controller.parameters)
                {
                    if (item.name == _name)
                        return true;
                }
                controller.AddParameter(_name, _paramType);
                return true;
            }
#endif
            return false;
        }
    }
}
