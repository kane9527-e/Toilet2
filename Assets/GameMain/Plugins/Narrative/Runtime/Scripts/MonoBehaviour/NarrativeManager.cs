using System;
using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.MonoBehaviour
{
    [DisallowMultipleComponent]
    public class NarrativeManager : NarrativeBehaviour
    {
        public Action<DisplayNode> OnNarrativeNodeChangeEvent;
        
        public void SwitchNextDefaultNode()
        {
            if (Graph.CurrentNode)
                Graph.CurrentNode.SwitchNextDefaultNode();
        }

        
        public void PlayNarrative(NarrativeGraph graph)
        {
            SetNarrativeGraph(graph);
        }

        public void PlayNarrative(NarrativeGraph graph, DisplayNode node)
        {
            SetNarrativeGraph(graph,node);
        }
        
        public void PlayNarrative(NarrativeGraph graph, int index)
        {
            PlayNarrative(graph, graph.GetNodeByIndex(index));
        }

        protected override void OnNarrativeNodeChange(DisplayNode node)
        {
            OnNarrativeNodeChangeEvent?.Invoke(node);
        }

        #region Singleton

        private static NarrativeManager _instance;

        public static NarrativeManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType(typeof(NarrativeManager)) as NarrativeManager;

                    if (!_instance)
                    {
                        var obj = new GameObject(nameof(NarrativeManager));
                        _instance = obj.AddComponent<NarrativeManager>();
                    }
                    else
                    {
                        _instance.gameObject.name = nameof(NarrativeManager);
                    }

                    _instance.OnInstanceInit();
                }

                return _instance;
            }
        }


        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(_instance.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }


        protected virtual void OnInstanceInit()
        {
        }

        #endregion
    }
}