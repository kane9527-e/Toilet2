using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
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

        public override void OnEnter()
        {
            base.OnEnter();
            TriggerEvent();
        }

        public virtual void TriggerEvent()
        {
            if (eventConfig)
                eventConfig.Trigger();
        }

        public override bool CompatiblePortCondition(Direction direction, VisualGraphNode targetPortNode)
        {
            if (direction == Direction.Output)
            {
                var targetType = targetPortNode.GetType();
                if (targetType == typeof(TriggerNode) || targetType.BaseType == typeof(TriggerNode)) return true;
                if (targetType == typeof(BranchNode) || targetType.BaseType == typeof(BranchNode)) return true;
                if (!OutputsHasConnect) return true;
                if (OutputsIsExtensionNode()) return true;
                return OutputsHasType(targetType);
            }

            if (direction == Direction.Input)
            {
                //if (!InputsHasConnect) return true;
                if (targetPortNode.GetType().BaseType == typeof(DisplayNode))
                {
                    var displayNode = (DisplayNode)targetPortNode;
                    if (!displayNode.OutputsHasConnect) return true;
                    return displayNode.OutputsHasType(GetType());
                }
            }

            return true;
        }

        public bool OutputsHasType(Type type)
        {
            foreach (var port in Outputs)
            {
                foreach (var connection in port.Connections)
                {
                    if (connection.Node.GetType() == type)
                        return true;
                }
            }

            return false;
        }

        public bool OutputsIsExtensionNode()
        {
            foreach (var port in Outputs)
            {
                foreach (var connection in port.Connections)
                {
                    if (connection.Node.GetType().BaseType != typeof(TriggerNode) &&
                        connection.Node.GetType().BaseType != typeof(BranchNode))
                        return false;
                }
            }

            return true;
        }

        public bool OutputsHasConnect
        {
            get
            {
                foreach (var port in Outputs)
                {
                    foreach (var connection in port.Connections)
                    {
                        if (connection.Node)
                            return true;
                    }
                }

                return false;
            }
        }

        public bool InputsHasConnect
        {
            get
            {
                foreach (var port in Inputs)
                {
                    foreach (var connection in port.Connections)
                    {
                        if (connection.Node)
                            return true;
                    }
                }

                return false;
            }
        }
    }
}