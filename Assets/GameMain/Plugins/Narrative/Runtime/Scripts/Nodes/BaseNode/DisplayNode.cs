using UnityEngine;
using VisualGraphRuntime;

namespace Narrative.Runtime.Scripts.Nodes.BaseNode
{
    [CustomNodeStyle("DisplayNodeStyle")]
    public abstract class DisplayNode : NarrativeNode
    {
        [HideInInspector] [SerializeField] private EventConfig.EventConfig eventConfig;

        public EventConfig.EventConfig EventConfig
        {
            get => eventConfig;
            set => eventConfig = value;
        }

        public override void OnEnter(object args)
        {
            base.OnEnter(args);
            TriggerEvent();
        }

        public virtual void TriggerEvent()
        {
            if (eventConfig)
                eventConfig.Trigger();
        }
    }
}