using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    public abstract class VisualGraphPortView : GraphElement
    {
        public abstract void CreateView(VisualGraphPort port);

        
    }
}