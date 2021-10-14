// ReSharper disable once CheckNamespace

using UnityEngine;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.DisplayNode
{
    [NodeName("DisplayNode/Image Node")]
    [CustomNodeStyle("ImageDisplayNodeStyle")]
    public class ImageDisplayNode : BaseNode.DisplayNode
    {
        [SerializeField] private Texture2D texture2D;

        public Texture2D Texture2D
        {
            get => texture2D;
            set => texture2D = value;
        }
    }
}