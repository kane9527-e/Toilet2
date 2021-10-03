using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine.UIElements;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.BaseNodeViews
{
    [CustomNodeView(typeof(NarrativeNode))]
    public class NarrativeNodeView : VisualGraphNodeView
    {
        public VisualElement NodeDataView;
        
    }
}
