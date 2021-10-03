using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Narrative.Runtime.Scripts.EventConfig;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using VisualGraphRuntime;

[NodePortAggregateAttribute(NodePortAggregateAttribute.PortAggregate.Single,
    NodePortAggregateAttribute.PortAggregate.Single)]
// Override the default settings for the Port Capacity
[PortCapacity(PortCapacityAttribute.Capacity.Multi, PortCapacityAttribute.Capacity.Single)]
[CanNotConnectSameNode]
[CustomNodeStyle("OptionNodeStyle")]
public abstract class OptionNode : NarrativeNode
{
    [SerializeField] private bool once;
    [SerializeField] private EventConfig eventConfig;
    
    public bool Triggered
    {
        get
        {
            var ins = NarrativeManager.Instance;
            if (!ins || !ins.DataBase)
                return false;
            var key = String.Format("OptionNode.Triggered.{0}", guid);
            return ins.DataBase.GetBool(key, false);
        }
        private set
        {
            var ins = NarrativeManager.Instance;
            if (!ins || !ins.DataBase)
                return;
            var key = String.Format("OptionNode.Triggered.{0}", guid);
            ins.DataBase.SetBool(key, value);
        }
    }

    public bool Once
    {
        get => once;
        set => once = value;
    }


    public EventConfig EventConfig
    {
        get => eventConfig;
        set => eventConfig = value;
    }

    public override bool CompatiblePortCondition(Direction direction, VisualGraphNode targetPortNode)
    {
        if (direction == Direction.Input)
        {
            if (targetPortNode.GetType().BaseType == typeof(DisplayNode))
            {
                var displayNode = (DisplayNode)targetPortNode;
                if (!displayNode.OutputsHasConnect)
                    return true; //If the target display node, the output is not connected to the node
                return displayNode.OutputsHasType(GetType()); //check target display node outputs has OptionType？
            }
        }

        return true;
    }

    public override void SwitchNextDefaultNode()
    {
        base.SwitchNextDefaultNode();

        if (once && Triggered)
            return;

        if (eventConfig)
            eventConfig.Trigger();

        if (!Triggered)
            Triggered = true;
    }
}