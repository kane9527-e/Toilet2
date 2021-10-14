using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    public class VisualGraphGroupView : Group
    {
        public VisualGraphGroup group { get; private set; }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            base.OnElementsAdded(elements);
            group = userData as VisualGraphGroup;
            foreach (var element in elements)
            {
                var node = element.userData as VisualGraphNode;
                if (group != null && node is { } && !group.node_guids.Contains(node.guid))
                    group.node_guids.Add(node.guid);
            }
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            base.OnElementsRemoved(elements);
            group = userData as VisualGraphGroup;
            foreach (var element in elements)
            {
                var node = element.userData as VisualGraphNode;
                if (group != null) group.node_guids.RemoveAll(guid => node is { } && guid == node.guid);
            }
        }

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            base.OnGroupRenamed(oldName, newName);

            group = userData as VisualGraphGroup;
            group.title = newName;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            foreach (var element in containedElements)
                element.selected = true;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            foreach (var element in containedElements)
                element.selected = false;
        }
    }
}