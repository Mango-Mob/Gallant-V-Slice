using UnityEngine;
using XNode;
using BTSystem.Nodes;
using System.Collections.Generic;

namespace BTSystem.Core
{
    [CreateAssetMenu(menuName = "BT Graph/Graph")]
    public class BTGraph : NodeGraph
    {
        public List<Node> currentNodes;

        private RootNode rootNode;

        [Header("Agent Data")]
        public EntitySystem.Core.AI.AIEntity Owner;
        public EntitySystem.Core.Entity Target;
        public bool IsDead;
        public bool IsAttacking;
        public float HealthPercent;
        public float StaminaPercent;

        public void Execute()
        {
            if (rootNode == null)
            {
                foreach (var node in nodes)
                {
                    if (node is RootNode)
                    {
                        rootNode = node as RootNode;
                        break;
                    }
                }
            }

            currentNodes.Clear();
            rootNode.Execute();
        }

        public Node GetNodeOfName(string _name)
        {
            foreach (var item in nodes)
            {
                if (item.name == _name)
                    return item;
            }

            return null;
        }
        public T GetNodeOfType<T>() where T : Node
        {
            foreach (var item in nodes)
            {
                if (item.GetType().IsEquivalentTo(typeof(T)))
                    return item as T;
            }

            return null;
        }

        public void Apply(BTGraph _other)
        {
            this.Owner = _other.Owner;
            this.Target = _other.Target;
            this.HealthPercent = _other.HealthPercent;
            this.StaminaPercent = _other.StaminaPercent;
            this.IsDead = _other.IsDead;
            this.IsAttacking = _other.IsAttacking;
        }

        protected override void OnDestroy()
        {
            //Might be causing memory leaks?
            //base.OnDestroy();
        }
    }
}