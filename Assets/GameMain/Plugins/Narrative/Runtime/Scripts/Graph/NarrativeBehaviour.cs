using System.Collections.Generic;
using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.NarrativeDataBase;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine;
using VisualGraphRuntime;

public class NarrativeBehaviour : VisualGraphMonoBehaviour<NarrativeGraph>
{
    [SerializeField] private NarrativeDataBase dataBase;
    public NarrativeDataBase DataBase => dataBase;

    public DisplayNode LastNode { get => Graph.LastNode; }

    protected override void AutoInitGraph()
    {
        if (Graph == null)
        {
            base.AutoInitGraph();
            if (Graph)
            {
                Graph.OnNodeChangedEvent += OnNarrativeNodeChange;
                OnNarrativeNodeChange(Graph.CurrentNode);
            }
        }
    }

    public void SetNarrativeGraph(NarrativeGraph graph, DisplayNode node = null)
    {
        if (!graph) return;
        if (Graph && Graph.CurrentNode)
        {
            Graph.OnNodeChangedEvent -= OnNarrativeNodeChange;
            Graph.CurrentNode.OnExit(node);
        }

        SetGraph(graph);
        Graph.OnNodeChangedEvent += OnNarrativeNodeChange;
        if (node)
            Graph.SwitchNode(graph.GetIndexByNode(node));

        Graph.Init();
    }

    private void Update()
    {
        if (Graph != null)
            // if (Graph.CurrentNode != lastNode)
            // {
            //     lastNode = Graph.CurrentNode;
            //     OnNarrativeNodeChange(lastNode);
            // }
            Graph.Update();
    }

    protected virtual void OnNarrativeNodeChange(DisplayNode node)
    {
    }

    public List<T> GetAllPortNodes<T>(NarrativeNode node) where T : NarrativeNode
    {
        var nodes = new List<T>();
        foreach (var port in node.Outputs)
        {
            var narrativePort = (NarrativePort)port;
            // We assume only one connection based off settings
            if (port.Connections.Count >= 0)
                foreach (var connection in port.Connections)
                    if (connection.Node is T)
                        nodes.Add((T)connection.Node);
        }

        return nodes;
    }

    public List<T> GetPortNodesWithCondition<T>(NarrativeNode node) where T : NarrativeNode
    {
        if (!node) return null;
        return node.GetPortNodesWithCondition<T>();
    }

    public List<T> GetPortNodesWithCondition<T>(NarrativeNode node, VisualGraphPort.PortDirection direction)
        where T : NarrativeNode
    {
        if (!node) return null;
        return node.GetPortNodesWithCondition<T>(direction);
    }
}