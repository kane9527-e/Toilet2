using Narrative.Runtime.Scripts.Nodes.BaseNode;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes
{
    [CustomNodeView(typeof(BranchNode))]
    public class BranchNodeView : VisualGraphNodeView
    {
        public override bool ShowNodeProperties => false;
    }
}
