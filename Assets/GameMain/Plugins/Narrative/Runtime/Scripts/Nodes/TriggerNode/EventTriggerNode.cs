// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using ConditionSetting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;
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

        public override void OnEnter()
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