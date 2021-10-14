using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    [CustomPortView(typeof(VisualGraphPort))]
    public sealed class VisualGraphDefaultPortView : VisualGraphPortView
    {
        public override void CreateView(VisualGraphPort port)
        {
            var leftField = new TextField();
            leftField.value = port.Name;
            leftField.style.width = 100;
            leftField.RegisterCallback<ChangeEvent<string>>(
                evt =>
                {
                    if (string.IsNullOrEmpty(evt.newValue) == false) port.Name = evt.newValue;
                }
            );
            Add(leftField);
        }
        
    }
}