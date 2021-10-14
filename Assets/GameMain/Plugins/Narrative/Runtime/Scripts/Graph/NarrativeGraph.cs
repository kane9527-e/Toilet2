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
            CurrentNode = (DisplayNode)StartingNode.Outputs.First().Connections[0].Node;
            foreach (var state in Nodes)
            {
                var node = state as NarrativeNode;
                if (node != null) node.narrativeGraph = this;
            }

            CurrentNode.OnEnter();
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
            if (!Nodes.Contains(node)) return;
            CurrentNode.OnExit();
            CurrentNode = node;
            CurrentNode.OnEnter();
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
    }
}