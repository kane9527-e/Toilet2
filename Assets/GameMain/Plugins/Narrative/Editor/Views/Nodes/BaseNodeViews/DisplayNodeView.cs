using System.Linq;
using Narrative.Runtime.Scripts.EventConfig;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.BaseNodeViews
{
    [CustomNodeView(typeof(DisplayNode))]
    public class DisplayNodeView : NarrativeNodeView
    {
        private DisplayNode _node;

        public override void DrawNode()
        {
            _node = (DisplayNode)nodeTarget;

            DrawNodeDataView();

            var eventConfigField = new ObjectField();
            eventConfigField.tooltip = "EventConfig";
            eventConfigField.objectType = typeof(EventConfig);
            eventConfigField.value = _node.EventConfig;
            eventConfigField.style.flexDirection = FlexDirection.Column;
            eventConfigField.RegisterValueChangedCallback(callback =>
            {
                _node.EventConfig = (EventConfig)callback.newValue;
                //RefreshEventInspectorGUI(eventConfigField,_node.EventConfig);
            });
            //RefreshEventInspectorGUI(eventConfigField,_node.EventConfig);
            NodeDataView.Add(eventConfigField);
            mainContainer.Add(NodeDataView);
        }

        public override bool CompatiblePortCondition(Direction direction, VisualGraphNode targetPortNode)
        {
            if (direction == Direction.Output)
            {
                var targetType = targetPortNode.GetType();
                if (targetType == typeof(TriggerNode) || targetType.BaseType == typeof(TriggerNode)) return true;
                if (targetType == typeof(BranchNode) || targetType.BaseType == typeof(BranchNode)) return true;
                if (!OutputsHasConnect) return true;
                if (OutputsIsExtensionNode()) return true;
                return OutputsHasType(targetType);
            }

            if (direction == Direction.Input)
                //if (!InputsHasConnect) return true;
                if (targetPortNode.GetType().BaseType == typeof(DisplayNode))
                {
                    var displayNode = (DisplayNode)targetPortNode;
                    if (!((NarrativeNodeView)displayNode.graphElement).OutputsHasConnect) return true;
                    return OutputsHasType(GetType());
                }

            return true;
        }


        protected void RefreshEventInspectorGUI(VisualElement view, EventConfig config)
        {
            foreach (var child in view.Children().ToArray())
                if (child is IMGUIContainer)
                    view.Remove(child);

            if (config)
            {
                var editor = UnityEditor.Editor.CreateEditor(config);
                // ReSharper disable once Unity.NoNullPropagation
                var inspectorIMGUI = new IMGUIContainer(() => { editor?.OnInspectorGUI(); });
                view.Add(inspectorIMGUI);
            }
        }

        protected void DrawNodeDataView()
        {
            if (NodeDataView == null)
            {
                NodeDataView = new VisualElement();
                NodeDataView.AddToClassList("node_data");
            }
        }
    }
}