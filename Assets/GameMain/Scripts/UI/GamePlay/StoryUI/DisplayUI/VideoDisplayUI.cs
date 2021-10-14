using System;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UI.GamePlay.StoryUI;
using UnityEngine;
using UnityEngine.Video;

public class VideoDisplayUI : DisplayUI
{
    [SerializeField] private VideoPlayer videoPlayer;
    public override Type DisplayNodeType => typeof(VideoDisplayNode);

    public override void Display(DisplayNode node)
    {
        if (!node || !videoPlayer) return;
        var targetNode = (VideoDisplayNode)node;

        if (videoPlayer.isPlaying)
            videoPlayer.Stop();
        videoPlayer.clip = targetNode.VideoClip;
        videoPlayer.loopPointReached += VideoFinish;
        videoPlayer.Play();
    }

    private void VideoFinish(VideoPlayer source)
    {
        ShowOption();
    }
}