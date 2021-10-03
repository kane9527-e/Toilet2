using System.Collections.Generic;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.BaseNode
{
    [NodeName("BranchNode/BranchNode")]
    public class BranchNode : NarrativeNode
    {
        public List<NarrativeNode> GetResultNodes()
        {
            List<NarrativeNode> result = new List<NarrativeNode>();
            var nodes = GetPortNodesWithCondition<NarrativeNode>(VisualGraphPort.PortDirection.Output);
            foreach (var node in nodes)
            {
                if (node.GetType().BaseType != typeof(BranchNode))
                {
                    result.Add(node);
                }
                else
                {
                    foreach (var resultNode in ((BranchNode)node).GetResultNodes())
                    {
                        if (resultNode.GetType().BaseType != typeof(BranchNode))
                            result.Add(node);
                    }
                }
            }
            return result;
        }
    }
}