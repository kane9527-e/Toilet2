// ReSharper disable once CheckNamespace

using UnityEngine;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.TriggerNode
{
    [NodeName("TriggerNode/Event Node")]
    public class EventTriggerNode : BaseNode.TriggerNode
    {
        [SerializeField] private EventConfig.EventConfig config;

        public EventConfig.EventConfig Config
        {
            get => config;
            set => config = value;
        }

        public override void OnEnter(object args)
        {
            Trigger();
        }

        public override void Trigger()
        {
            if (config)
                config.Trigger();
        }
    }
}