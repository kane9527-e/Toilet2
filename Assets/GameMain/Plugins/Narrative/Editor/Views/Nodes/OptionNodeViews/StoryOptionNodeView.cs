using Narrative.Runtime.Scripts.Nodes.OptionNode;
using UnityEngine.UIElements;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes
{
    [CustomNodeView(typeof(StoryOptionNode))]
    public class StoryOptionNodeView : OptionNodeView
    {
        private StoryOptionNode _node;
        public override bool ShowNodeProperties => false;

        //public override string title => ((TextOptionNode)nodeTarget).OptionText;

        public override void DrawNode()
        {
            _node = (StoryOptionNode)nodeTarget;
            if (NodeDataView == null)
            {
                NodeDataView = new VisualElement();
                NodeDataView.AddToClassList("node_data");
            }

            RefreshTitle();
            //onceToggle.style.backgroundColor = new StyleColor(Color.black);

            var optionTextField = new TextField();
            optionTextField.multiline = true;
            optionTextField.value = _node.OptionText;
            //optionTextField.style.backgroundColor = new StyleColor(Color.black);
            optionTextField.RegisterValueChangedCallback(evt =>
            {
                _node.OptionText = evt.newValue;
                RefreshTitle();
            });


            NodeDataView.Add(optionTextField);
            base.DrawNode();
        }


        private void RefreshTitle()
        {
            if (_node)
                title = string.Format("Option:{0}", _node.OptionText);
        }
    }
}