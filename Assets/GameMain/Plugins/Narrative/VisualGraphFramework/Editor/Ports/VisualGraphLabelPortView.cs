using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    public class VisualGraphLabelPortView : VisualGraphPortView
    {
        public override void CreateView(VisualGraphPort port)
        {
            var field = new Label(port.Name);
            Add(field);
        }
        
    }
}