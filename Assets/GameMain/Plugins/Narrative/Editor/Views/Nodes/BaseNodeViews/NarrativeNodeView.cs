using System;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualGraphEditor;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.BaseNodeViews
{
    [CustomNodeView(typeof(NarrativeNode))]
    public class NarrativeNodeView : VisualGraphNodeView
    {
        public VisualElement NodeDataView;


        public bool InputsHasConnect
        {
            get
            {
                foreach (var port in nodeTarget.Inputs)
                foreach (var connection in port.Connections)
                    if (connection.Node)
                        return true;

                return false;
            }
        }


        public bool OutputsHasConnect
        {
            get
            {
                foreach (var port in nodeTarget.Outputs)
                foreach (var connection in port.Connections)
                    if (connection.Node)
                        return true;

                return false;
            }
        }


        public virtual bool CompatiblePortCondition(Direction direction, VisualGraphNode targetPortNode)
        {
            return true;
        }


        public bool OutputsIsExtensionNode()
        {
            foreach (var port in nodeTarget.Outputs)
            foreach (var connection in port.Connections)
                if (connection.Node.GetType().BaseType != typeof(TriggerNode) &&
                    connection.Node.GetType().BaseType != typeof(BranchNode))
                    return false;

            return true;
        }

        public bool OutputsHasType(Type type)
        {
            foreach (var port in nodeTarget.Outputs)
            foreach (var connection in port.Connections)
                if (connection.Node.GetType() == type)
                    return true;

            return false;
        }
    }
}