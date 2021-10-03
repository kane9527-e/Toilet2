using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.TriggerNode
{
    [NodeName("TriggerNode/SwitchGraph Node")]
    public class SwitchGraphTriggerNode : BaseNode.TriggerNode
    {
        [SerializeField] private NarrativeGraph targetGraph;
        [SerializeField] private BaseNode.DisplayNode targetNode;
        
        public override void OnEnter()
        {
            Trigger();
        }

        public override void Trigger()
        {
            NarrativeManager.Instance.StartNarrative(targetGraph,targetNode);
        }
    }
}