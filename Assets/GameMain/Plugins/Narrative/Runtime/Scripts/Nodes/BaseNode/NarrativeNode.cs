﻿using System.Collections.Generic;
using System.Linq;
using Narrative.Runtime.Scripts.Graph;
using UnityEngine;
using VisualGraphRuntime;

namespace Narrative.Runtime.Scripts.Nodes.BaseNode
{
    [CustomNodeStyle("NarrativeNodeStyle")]
    [DefaultPortType(typeof(NarrativePort))]
    public abstract class NarrativeNode : VisualGraphNode
    {
        [HideInInspector] public NarrativeGraph narrativeGraph;


        private List<TriggerNode> _inputTriggerNodes;

        private List<TriggerNode> _outputTriggerNodes;

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public override void Init()
        {
            base.Init();
        }


        public virtual void SwitchNextDefaultNode()
        {
            if (FindPortByName("Exit") == null) return;

            _outputTriggerNodes = GetPortNodesWithCondition<TriggerNode>(VisualGraphPort.PortDirection.Output);
            var branchNodes = GetPortNodesWithCondition<BranchNode>(VisualGraphPort.PortDirection.Output);
            foreach (var branchNode in branchNodes)
            {
                var resultNodes = branchNode.GetResultNodes();
                for (var i = resultNodes.Count - 1; i >= 0; i--)
                {
                    var resultNode = resultNodes[i];
                    if (typeof(DisplayNode).IsInstanceOfType(resultNode))
                        narrativeGraph.SwitchNode((DisplayNode)resultNode);
                    if (typeof(TriggerNode).IsInstanceOfType(resultNode))
                        _outputTriggerNodes.Add((TriggerNode)resultNode);
                }
            }

            foreach (var triggerNode in _outputTriggerNodes)
            {
                triggerNode.Trigger();
            }

            foreach (var visualGraphPort in Outputs)
            {
                var output = (NarrativePort)visualGraphPort;
                if (output.conditionConfig == null || output.conditionConfig.Result())
                {
                    var portConnections = output.Connections;
                    if (portConnections != null && portConnections.Count > 0)
                        for (var i = portConnections.Count - 1; i >= 0; i--)
                        {
                            var connection = portConnections[i];
                            var input = (NarrativePort)connection.port;
                            if (connection.Node && connection.Node is DisplayNode) //检测是否为显示节点
                                if (input.conditionConfig == null || input.conditionConfig.Result())
                                {
                                    narrativeGraph.SwitchNode((DisplayNode)connection.Node);
                                    return;
                                }
                        }
                }
            }
        }

        public List<T> GetPortNodesWithCondition<T>() where T : NarrativeNode
        {
            var nodes = new List<T>();
            foreach (var port in Ports)
            {
                var narrativePort = (NarrativePort)port;
                // We assume only one connection based off settings
                if (narrativePort.Connections.Count >= 0)
                    foreach (var connection in narrativePort.Connections)
                    {
                        var conditionConfig = narrativePort.conditionConfig;
                        // ReSharper disable once MergeCastWithTypeCheck
                        if (!(typeof(T).IsInstanceOfType(connection.Node))) continue;
                        if (!conditionConfig || conditionConfig && conditionConfig.Result())
                            nodes.Add((T)connection.Node);
                    }
            }

            return nodes;
        }


        public List<T> GetPortNodesWithCondition<T>(VisualGraphPort.PortDirection direction) where T : NarrativeNode
        {
            var nodes = new List<T>();
            var ports = direction == VisualGraphPort.PortDirection.Output ? Outputs : Inputs;
            var visualGraphPorts = ports.ToList();
            if (visualGraphPorts.Count <= 0) return nodes;

            foreach (var port in visualGraphPorts)
            {
                var narrativePort = (NarrativePort)port;
                // We assume only one connection based off settings
                if (narrativePort.Connections.Count >= 0)
                    foreach (var connection in narrativePort.Connections)
                    {
                        var conditionConfig = narrativePort.conditionConfig;
                        // ReSharper disable once MergeCastWithTypeCheck
                        if (!(typeof(T).IsInstanceOfType(connection.Node))) continue;
                        if (!conditionConfig || conditionConfig && conditionConfig.Result())
                            nodes.Add((T)connection.Node);
                    }
            }

            return nodes;
        }

        public virtual void OnEnter(object args)
        {
            editor_ActiveNode = true;

            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log(string.Format("{0}进入-guid:{1}", this.name, guid));
            
            //收集所有输入口，并且触发事件
            _inputTriggerNodes = GetPortNodesWithCondition<TriggerNode>(VisualGraphPort.PortDirection.Input);
            var branchNodes = GetPortNodesWithCondition<BranchNode>(VisualGraphPort.PortDirection.Input);
            foreach (var branchNode in branchNodes)
            {
                var resultNodes = branchNode.GetResultNodes();
                for (var i = resultNodes.Count - 1; i >= 0; i--)
                {
                    var resultNode = resultNodes[i];
                    if (typeof(TriggerNode).IsInstanceOfType(resultNode))
                        _inputTriggerNodes.Add((TriggerNode)resultNode);
                }
            }

            foreach (var triggerNode in _inputTriggerNodes)
            {
                triggerNode.Trigger();
            }
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnExit(object args)
        {
            editor_ActiveNode = false;

            if (Application.isEditor || Debug.isDebugBuild)
                Debug.Log(string.Format("{0}退出-guid:{1}", this.name, guid));

            if (_outputTriggerNodes != null) return;
            //收集所有输出口，并且触发事件
            _outputTriggerNodes = GetPortNodesWithCondition<TriggerNode>(VisualGraphPort.PortDirection.Output);
            var branchNodes = GetPortNodesWithCondition<BranchNode>(VisualGraphPort.PortDirection.Output);
            foreach (var branchNode in branchNodes)
            {
                var resultNodes = branchNode.GetResultNodes();
                for (var i = resultNodes.Count - 1; i >= 0; i--)
                {
                    var resultNode = resultNodes[i];
                    if (typeof(TriggerNode).IsInstanceOfType(resultNode))
                        _outputTriggerNodes.Add((TriggerNode)resultNode);
                }
            }

            foreach (var triggerNode in _outputTriggerNodes)
            {
                triggerNode.Trigger();
            }
        }

        public VisualGraphPort.VisualGraphPortConnection Connect(NarrativePort port, NarrativeNode targetNode)
        {
            if (Ports.Contains(port))
                return port.Connect(targetNode);
            return null;
        }

        public bool HasConnect(NarrativeNode node)
        {
            return Ports.Exists(item =>
                item.Connections.Exists(connect => connect.node_guid == guid));
        }

        public VisualGraphPort.PortDirection GetConnectDirection(NarrativeNode node)
        {
            var r = VisualGraphPort.PortDirection.Output;
            foreach (var port in Ports)
                if (port.Connections.Exists(connection => connection.node_guid == guid))
                    return port.Direction;

            return r;
        }

        public List<int> GetConnectPortIndexes(NarrativeNode node)
        {
            var indexes = new List<int>();
            for (var i = 0; i < Ports.Count; i++)
            {
                var port = Ports[i];
                foreach (var connection in port.Connections)
                    if (connection.node_guid == node.guid)
                    {
                        indexes.Add(i);
                        break;
                    }
            }

            return indexes;
        }
    }
}