using System.Linq;
using Narrative.Editor.Views.Nodes.BaseNodeViews;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.DisplayNodeViews
{
    [CustomNodeView(typeof(ImageDisplayNode))]
    public class ImageDisplayNodeView : DisplayNodeView
    {
        public override bool ShowNodeProperties => false;

        private ImageDisplayNode _node;
        public override void DrawNode()
        {
            DrawNodeDataView();
            _node = (ImageDisplayNode)nodeTarget;
            ObjectField imageField = new ObjectField();
            imageField.tooltip = "Texture";
            imageField.label = "Texture";
            imageField.objectType = typeof(Texture2D);
            imageField.value = _node.Texture2D;
            imageField.style.flexDirection = FlexDirection.Column;
            imageField.RegisterValueChangedCallback(callback =>
            {
                _node.Texture2D = (Texture2D)callback.newValue;
                RefreshPreviewImage(imageField);
                RefreshTitle();
            });
            RefreshPreviewImage(imageField);
            NodeDataView.Add(imageField);
            RefreshTitle();
            base.DrawNode();
        }
        
        private void RefreshTitle()
        {
            title = _node.Texture2D ? string.Format("Image:{0}", _node.Texture2D.name) : "ImageDisplayNode";
        }
        
        private void RefreshPreviewImage(VisualElement element)
        {
            foreach (var child in element.Children().ToArray())
            {
                if (child.GetType() == typeof(Image))
                {
                    element.Remove(child);
                    break;
                }
            }

            if (_node.Texture2D != null)
            {
                Image previewImage = new Image();
                previewImage.image = _node.Texture2D;
                previewImage.style.alignSelf = Align.Center;
                var defaultWidth= default_size.x;
                var defaultHeight= default_size.y;

                var imageWidth = previewImage.image.width;
                var imageHeight = previewImage.image.height;
                
                var widthScaleRatio = defaultWidth / imageWidth;
                var heightScaleRatio = defaultHeight / imageHeight;

                var ratio = widthScaleRatio < heightScaleRatio ? widthScaleRatio : heightScaleRatio;
                
                previewImage.style.minWidth = imageWidth*ratio;
                previewImage.style.maxWidth = imageWidth*ratio;
                previewImage.style.minHeight =  imageHeight*ratio;
                previewImage.style.maxHeight =  imageHeight*ratio;
                element.Add(previewImage);
            }
        }
    }
}
