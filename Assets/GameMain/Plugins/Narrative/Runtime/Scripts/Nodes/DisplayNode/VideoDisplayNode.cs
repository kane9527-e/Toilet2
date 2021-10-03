// ReSharper disable once CheckNamespace

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Video;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.DisplayNode
{
    [CustomNodeStyle("VideoDisplayNodeStyle")]
    [NodeName("DisplayNode/Video Node")]
    public class VideoDisplayNode : BaseNode.DisplayNode
    {
        [SerializeField] private VideoClip videoClip;

        public VideoClip VideoClip
        {
            get => videoClip;
            set => videoClip = value;
        }
    }
}
