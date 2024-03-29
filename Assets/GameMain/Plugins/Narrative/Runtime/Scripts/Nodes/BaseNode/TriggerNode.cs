using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.BaseNode
{
    [NodePortAggregate(NodePortAggregateAttribute.PortAggregate.Single,
        NodePortAggregateAttribute.PortAggregate.None)]
// Override the default settings for the Port Capacity
    [PortCapacity]
    [CustomNodeStyle("TriggerNodeStyle")]
    public abstract class TriggerNode : NarrativeNode
    {
        public abstract void Trigger();
    }
}