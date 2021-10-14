using Narrative.Editor.Views.Nodes.BaseNodeViews;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Video;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.DisplayNodeViews
{
    [CustomNodeView(typeof(VideoDisplayNode))]
    public class VideoDisplayNodeView : DisplayNodeView
    {
        private VideoDisplayNode _node;
        public override bool ShowNodeProperties => false;

        public override void DrawNode()
        {
            DrawNodeDataView();
            _node = (VideoDisplayNode)nodeTarget;
            var videoField = new ObjectField();
            videoField.tooltip = "VideoClip";
            videoField.label = "VideoClip";
            videoField.objectType = typeof(VideoClip);
            videoField.value = _node.VideoClip;
            videoField.RegisterValueChangedCallback(callback =>
            {
                _node.VideoClip = (VideoClip)callback.newValue;
                RefreshTitle();
            });
            RefreshTitle();
            NodeDataView.Add(videoField);
            base.DrawNode();
        }

        private void RefreshTitle()
        {
            title = _node.VideoClip ? string.Format("Video:{0}", _node.VideoClip.name) : "VideoDisplayNode";
        }
    }
}