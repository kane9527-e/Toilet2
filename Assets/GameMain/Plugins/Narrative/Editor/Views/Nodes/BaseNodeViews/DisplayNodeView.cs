using System.Linq;
using Narrative.Runtime.Scripts.EventConfig;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.BaseNodeViews
{
    [CustomNodeView(typeof(DisplayNode))]
    public class DisplayNodeView : NarrativeNodeView
    {
        private DisplayNode _node;
        
        public override void DrawNode()
        {
            _node = ((DisplayNode)nodeTarget);

            DrawNodeDataView();

            ObjectField eventConfigField = new ObjectField();
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

        protected void RefreshEventInspectorGUI(VisualElement view,EventConfig config)
        {
            foreach (var child in view.Children().ToArray())
            {
                if (child is IMGUIContainer)
                    view.Remove(child);
            }

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