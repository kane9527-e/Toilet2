using System;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace

[Serializable]
public class NarrativePort : VisualGraphPort
{
    public ConditionSetting.ConditionConfig conditionConfig;

    public VisualGraphPortConnection Connect(NarrativeNode node)
    {
        var connection = new VisualGraphPortConnection();
        connection.port = node.FindPortByName(Direction == PortDirection.Input ? "Exit" : "Input");
        connection.port_guid = connection.port.guid;
        connection.Node = node;
        connection.node_guid = node.guid;
        connection.initialized = true;
        if (!Connections.Exists(item => item.node_guid == node.guid))
            Connections.Add(connection);
        return connection;
    }
}