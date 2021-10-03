using System;
using System.Collections;
using System.Collections.Generic;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UI.GamePlay.StoryUI;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplayUI : DisplayUI
{
    [SerializeField] private RawImage imageUI;

    public override Type DisplayNodeType => typeof(ImageDisplayNode);

    public override void Display(DisplayNode node)
    {
        if (!node || !imageUI) return;
        var targetNode = (ImageDisplayNode)node;
        imageUI.texture = targetNode.Texture2D;
        ShowOption();
    }
}