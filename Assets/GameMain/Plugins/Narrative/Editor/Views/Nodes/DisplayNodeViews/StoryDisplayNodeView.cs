using System.Linq;
using System.Reflection;
using Narrative.Editor.Views.Nodes.BaseNodeViews;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Nodes.DisplayNodeViews
{
    [CustomNodeView(typeof(StoryDisplayNode))]
    public class StoryDisplayNodeView : DisplayNodeView
    {
        private StoryDisplayNode _node;

        public override void DrawNode()
        {
            DrawNodeDataView();
            RefreshTitle();
            _node = (StoryDisplayNode)nodeTarget;

            var soundField = new ObjectField();
            soundField.tooltip = "Sound";
            soundField.label = "Sound";
            soundField.objectType = typeof(AudioClip);
            soundField.value = _node.Sound;
            soundField.RegisterValueChangedCallback(callback =>
            {
                _node.Sound = (AudioClip)callback.newValue;
                RefreshPlaySoundButton(soundField);
                PlayCurrentSound();
            });
            RefreshPlaySoundButton(soundField);

            var backGroundImageField = new ObjectField();
            backGroundImageField.tooltip = "BackGroundImage";
            backGroundImageField.label = "BackGroundImage";
            backGroundImageField.objectType = typeof(Texture2D);
            backGroundImageField.value = _node.BackgroundImage;
            backGroundImageField.style.flexDirection = FlexDirection.Column;
            backGroundImageField.RegisterValueChangedCallback(callback =>
            {
                _node.BackgroundImage = (Texture2D)callback.newValue;
                RefreshPreviewImage(backGroundImageField);
            });
            NodeDataView.Add(soundField);
            RefreshPreviewImage(backGroundImageField);
            NodeDataView.Add(backGroundImageField);
            base.DrawNode();
        }

        private void PlayCurrentSound()
        {
            if (_node.Sound)
                PlayClip(_node.Sound);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            RefreshTitle();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            RefreshTitle();
        }

        private void RefreshTitle()
        {
            var node = (StoryDisplayNode)nodeTarget;
            title = string.Format("Story:{0}", node.GetLine(0));
            if (string.IsNullOrWhiteSpace(title))
                title = nameof(StoryDisplayNode);
        }

        private void RefreshPreviewImage(VisualElement element)
        {
            foreach (var child in element.Children().ToArray())
                if (child.GetType() == typeof(Image))
                {
                    element.Remove(child);
                    break;
                }

            if (_node.BackgroundImage != null)
            {
                var previewImage = new Image();
                previewImage.image = _node.BackgroundImage;
                previewImage.style.alignSelf = Align.Center;
                var defaultWidth = default_size.x;
                var defaultHeight = default_size.y;

                var imageWidth = previewImage.image.width;
                var imageHeight = previewImage.image.height;

                var widthScaleRatio = defaultWidth / imageWidth;
                var heightScaleRatio = defaultHeight / imageHeight;

                var ratio = widthScaleRatio < heightScaleRatio ? widthScaleRatio : heightScaleRatio;

                previewImage.style.minWidth = imageWidth * ratio;
                previewImage.style.maxWidth = imageWidth * ratio;
                previewImage.style.minHeight = imageHeight * ratio;
                previewImage.style.maxHeight = imageHeight * ratio;
                element.Add(previewImage);
            }
        }

        private void RefreshPlaySoundButton(VisualElement element)
        {
            foreach (var child in element.Children().ToArray())
                if (child.GetType() == typeof(Button))
                {
                    element.Remove(child);
                    break;
                }

            if (_node.Sound)
            {
                var playSoundButton = new Button();
                playSoundButton.clickable.clicked += PlayCurrentSound;
                playSoundButton.text = "PlaySound";
                element.Add(playSoundButton);
            }
        }

        public static void PlayClip(AudioClip clip)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
#if UNITY_2019
                 "PlayClip",
#elif UNITY_2020_1_OR_NEWER
                "PlayPreviewClip",
#endif
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[]
                {
                    typeof(AudioClip),
                    typeof(int),
                    typeof(bool)
                },
                null
            );

            method.Invoke(
                null,
                new object[]
                {
                    clip,
                    0,
                    false
                }
            );
        }
        // Texture2D ScaleTexture(Texture2D source, float targetWidth, float targetHeight)
        // {
        //     Texture2D result = new Texture2D((int)targetWidth, (int)targetHeight, source.format, false);
        //
        //     float incX = (1.0f / targetWidth);
        //     float incY = (1.0f / targetHeight);
        //
        //     for (int i = 0; i < result.height; ++i)
        //     {
        //         for (int j = 0; j < result.width; ++j)
        //         {
        //             Color newColor =
        //                 source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
        //             result.SetPixel(j, i, newColor);
        //         }
        //     }
        //
        //     result.Apply();
        //     return result;
        // }
    }
}