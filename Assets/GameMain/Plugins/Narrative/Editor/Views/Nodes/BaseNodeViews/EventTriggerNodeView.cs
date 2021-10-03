using System.Linq;
using Narrative.Editor.Views.Nodes.BaseNodeViews;
using Narrative.Runtime.Scripts.EventConfig;
using Narrative.Runtime.Scripts.Nodes.TriggerNode;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VisualGraphEditor;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes
{
    [CustomNodeView(typeof(EventTriggerNode))]
    public class EventTriggerNodeView : TriggerNodeView
    {
        public override bool ShowNodeProperties => false;

        public override void DrawNode()
        {
            base.DrawNode();
            NodeDataView = new VisualElement();
            NodeDataView.AddToClassList("node_data");
            mainContainer.Add(NodeDataView);

            EventTriggerNode node = (EventTriggerNode)nodeTarget;
            ObjectField eventField = new ObjectField();
            eventField.objectType = typeof(EventConfig);
            eventField.value = node.Config;
            eventField.RegisterValueChangedCallback(callback =>
            {
                node.Config = (EventConfig)callback.newValue;
                RefreshEventInspectorGUI(node.Config);
            });
            NodeDataView.Add(eventField);
            RefreshEventInspectorGUI(node.Config);
        }

        private void RefreshEventInspectorGUI(EventConfig config)
        {
            foreach (var child in NodeDataView.Children().ToArray())
            {
                if (child is IMGUIContainer)
                    NodeDataView.Remove(child);
            }

            if (config)
            {
                var editor = UnityEditor.Editor.CreateEditor(config);
                // ReSharper disable once Unity.NoNullPropagation
                var inspectorIMGUI = new IMGUIContainer(() => { editor?.OnInspectorGUI(); });
                NodeDataView.Add(inspectorIMGUI);
            }
        }
    }
}