using System;
using System.Collections.Generic;
using System.Linq;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine;
using VisualGraphRuntime;

namespace Narrative.Runtime.Scripts.Graph
{
    [CreateAssetMenu(menuName = "Narrative/NarrativeGraph")]
    [DefaultNodeType(typeof(NarrativeNode))]
    public class NarrativeGraph : VisualGraph
    {
#if UNITY_EDITOR
        /// <summary>
        ///     StickyNotes.
        /// </summary>
        [HideInInspector] [SerializeField] public List<NarativeStickyNote> StickyNotes = new List<NarativeStickyNote>();
#endif

        private DisplayNode _currentNode;

        public Action<DisplayNode> OnNodeChangedEvent;

        public DisplayNode LastNode { get; private set; }

        public DisplayNode CurrentNode
        {
            get => _currentNode;
            private set
            {
                _currentNode = value;
                OnNodeChangedEvent?.Invoke(_currentNode);
            }
        }

        public override void Init()
        {
            Debug.Assert(StartingNode.Outputs.First().Connections.Count != 0, "Starting node needs a connection");
            if (!(StartingNode.Outputs.First().Connections[0].Node is DisplayNode)) return; //检测是否为显示节点

            foreach (var state in Nodes)
            {
                var node = state as NarrativeNode;
                if (node != null) node.narrativeGraph = this;
            }

            if (CurrentNode == null)
            {
                CurrentNode = (DisplayNode)StartingNode.Outputs.First().Connections[0].Node;
                CurrentNode.OnEnter(CurrentNode);
            }
        }

        public void Update()
        {
            if (CurrentNode != null) CurrentNode.OnUpdate();
        }


        /// <summary>
        ///     SwitchNode
        /// </summary>
        /// <param name="node"></param>
        public void SwitchNode(DisplayNode node)
        {
            if (!node) return;
            if (!Nodes.Contains(node)) return;

            LastNode = CurrentNode;
            if (CurrentNode) //如果上一个有节点
                CurrentNode.OnExit(node);
            CurrentNode = node;
            CurrentNode.OnEnter(LastNode);
        }


        public void SwitchNode(int index)
        {
            if (index < 0 || index >= Nodes.Count) return;
            SwitchNode(GetNodeByIndex(index));
        }

        // public void GoToState(NarrativePort state)
        // {
        //     if (currentState != null)
        //     {
        //         foreach (var port in currentState.Outputs)
        //         {
        //             NarrativePort narrativePort = (NarrativePort)port;
        //             // We assume only one connection based off settings
        //             if (port.Connections.Count >= 0)
        //             {
        //                 currentState.OnExit();
        //                 currentState = (NarrativeState)port.Connections[0].Node;
        //                 currentState.OnEnter();
        //             }
        //         }
        //     }
        // }

        public int GetIndexByNode(DisplayNode node) => Nodes.FindIndex(item => item == node);

        public DisplayNode GetNodeByIndex(int index)
        {
            if (index < 0 || index >= Nodes.Count) return null;
            return Nodes[index] as DisplayNode;
        }
    }
}