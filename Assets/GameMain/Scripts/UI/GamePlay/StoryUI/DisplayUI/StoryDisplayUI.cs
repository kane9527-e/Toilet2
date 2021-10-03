using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Narrative.Runtime.Scripts.Nodes.DisplayNode;
using UI.GamePlay.StoryUI;
using UnityEngine;
using UnityEngine.UI;

public class StoryDisplayUI : DisplayUI
{
    public override Type DisplayNodeType => typeof(StoryDisplayNode);
    [SerializeField] private Text textUI;

    private Coroutine _typeEffectCoroutine;

    public override void Display(DisplayNode node)
    {
        var targetNode = (StoryDisplayNode)node;
        if (textUI && targetNode)
        {
            PlayTypeEffect(targetNode.GetStoryText());
        }
    }

    private void PlayTypeEffect(string displayText)
    {
        if (_typeEffectCoroutine != null)
            StopCoroutine(_typeEffectCoroutine);
        _typeEffectCoroutine = StartCoroutine(ShowStoryTextWithTypeEffect(displayText));
    }

    IEnumerator ShowStoryTextWithTypeEffect(string text)
    {
        StringBuilder effectBuilder = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            effectBuilder.Append(text[i]);
            if (textUI)
                textUI.text = effectBuilder.ToString();
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
        }
        ShowOption();
    }
}