using UnityEditor.Experimental.GraphView;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [CustomNodeView(typeof(VisualGraphStartNode))]
    public sealed class VisualGraphStartNodeView : VisualGraphNodeView
    {
        public override bool ShowNodeProperties => false;

        public override Capabilities SetCapabilities(Capabilities capabilities)
        {
            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;
            return capabilities;
        }
    }
}