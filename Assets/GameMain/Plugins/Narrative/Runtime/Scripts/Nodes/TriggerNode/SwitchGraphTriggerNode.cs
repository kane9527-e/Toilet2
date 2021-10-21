using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.MonoBehaviour;
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

        public override void OnEnter(object args)
        {
            Trigger();
        }

        public override void Trigger()
        {
            NarrativeManager.Instance.PlayNarrative(targetGraph, targetNode);
        }
    }
}