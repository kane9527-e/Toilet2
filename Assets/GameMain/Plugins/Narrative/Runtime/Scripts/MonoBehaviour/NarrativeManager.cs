using System;
using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
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

        public void StartNarrative()
        {
            Start();
        }

        public void StartNarrative(NarrativeGraph graph)
        {
            Graph = graph;
            StartNarrative();
        }

        public void StartNarrative(NarrativeGraph graph, DisplayNode node)
        {
            if (node == null)
            {
                StartNarrative(graph);
                return;
            }

            Graph = graph;
            Graph.SwitchNode(node);
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

            if (DataBase)
            {
                DataBase.LoadDataKeys();
                DataBase.ClearAllData();
            }
        }

        protected virtual void OnInstanceInit()
        {
        }

        #endregion
    }
}