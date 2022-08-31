using BTSystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace BTSystem.Nodes.Flow
{
    [CreateNodeMenu("Flow/SubGraph", order = 1)]
    public class SubGraphNode : FlowNode
    {
        public BTGraph SubGraph;

        public void Awake()
        {
            if(SubGraph != null && Application.isPlaying)
                SubGraph = SubGraph.Copy() as BTGraph;
        }

        // Use this for initialization
        public override void Execute()
        {
            base.Execute();
            SubGraph.Apply(this.graph as BTGraph);
            SubGraph.Execute();
        }
    }
}