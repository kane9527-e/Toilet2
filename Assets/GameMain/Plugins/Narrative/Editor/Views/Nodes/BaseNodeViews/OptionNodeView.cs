using System.Linq;
using Narrative.Editor.Views.Nodes.BaseNodeViews;
using Narrative.Runtime.Scripts.EventConfig;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes
{
    [CustomNodeView(typeof(OptionNode))]
    public class OptionNodeView :  NarrativeNodeView
    {
        private OptionNode _node;
        public override void DrawNode()
        {
            _node = ((OptionNode)nodeTarget);

            if (NodeDataView == null)
            {
                NodeDataView = new VisualElement();
                NodeDataView.AddToClassList("node_data");
            }


            mainContainer.Add(NodeDataView);
            Toggle onceToggle = new Toggle();
            onceToggle.text = "Once";
            onceToggle.value = _node.Once;
            onceToggle.RegisterValueChangedCallback(evt => _node.Once = evt.newValue);
            
            ObjectField eventConfigField = new ObjectField();
            eventConfigField.style.flexDirection = FlexDirection.Column;
            eventConfigField.tooltip = "EventConfig";
            eventConfigField.objectType = typeof(EventConfig);
            eventConfigField.value = _node.EventConfig;
            eventConfigField.RegisterValueChangedCallback(callback =>
            {
                _node.EventConfig = (EventConfig)callback.newValue;
                //RefreshEventInspectorGUI(eventConfigField,_node.EventConfig);
            });
            //RefreshEventInspectorGUI(eventConfigField,_node.EventConfig);
            NodeDataView.Add(onceToggle);
            NodeDataView.Add(eventConfigField);
        }

        private void RefreshEventInspectorGUI(VisualElement view, EventConfig config)
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
    }
}