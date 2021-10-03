﻿using System;
using System.Collections;
using System.Collections.Generic;
using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.NarrativeDataBase;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using VisualGraphRuntime;

public class NarrativeBehaviour : VisualGraphMonoBehaviour<NarrativeGraph>
{
    private DisplayNode lastNode;
    [SerializeField] private NarrativeDataBase dataBase;
    public NarrativeDataBase DataBase => dataBase;

    protected override void Start()
    {
        base.Start();
        Graph.OnNodeChangedEvent += OnNarrativeNodeChange;
        OnNarrativeNodeChange(Graph.CurrentNode);
    }

    void Update()
    {
        if (Graph != null)
        {
            // if (Graph.CurrentNode != lastNode)
            // {
            //     lastNode = Graph.CurrentNode;
            //     OnNarrativeNodeChange(lastNode);
            // }
            Graph.Update();
        }
    }

    protected virtual void OnNarrativeNodeChange(DisplayNode node)
    {
        
    }

    public List<T> GetAllPortNodes<T>(NarrativeNode node) where T : NarrativeNode
    {
        List<T> nodes = new List<T>();
        foreach (var port in node.Outputs)
        {
            NarrativePort narrativePort = (NarrativePort)port;
            // We assume only one connection based off settings
            if (port.Connections.Count >= 0)
            {
                foreach (var connection in port.Connections)
                {
                    if (connection.Node is T)
                        nodes.Add((T)connection.Node);
                }
            }
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