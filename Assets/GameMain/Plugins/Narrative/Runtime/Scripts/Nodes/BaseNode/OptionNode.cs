using Narrative.Runtime.Scripts.EventConfig;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine;
using VisualGraphRuntime;

[NodePortAggregateAttribute(NodePortAggregateAttribute.PortAggregate.Single,
    NodePortAggregateAttribute.PortAggregate.Single)]
// Override the default settings for the Port Capacity
[PortCapacity]
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
            var key = string.Format("OptionNode.Triggered.{0}", guid);
            return ins.DataBase.GetBool(key);
        }
        private set
        {
            var ins = NarrativeManager.Instance;
            if (!ins || !ins.DataBase)
                return;
            var key = string.Format("OptionNode.Triggered.{0}", guid);
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