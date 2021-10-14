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
    [SerializeField] private Text textUI;
    [SerializeField] private List<DelayCharConfig> delayCharConfigs;
    [SerializeField] private float wrapDelayTime = 0.3f;
    [SerializeField] private float textDelayTime = 0.1f;
    private Coroutine _typeEffectCoroutine;
    public override Type DisplayNodeType => typeof(StoryDisplayNode);

    private float _progressSpeedUp = 0f;
    public void ProgressSpeedUp(float speed) => _progressSpeedUp += speed;

    public override void Display(DisplayNode node)
    {
        _progressSpeedUp = 0;
        var targetNode = (StoryDisplayNode)node;
        if (textUI && targetNode)
        {
            var storyText = targetNode.GetStoryText();
            var speakText = storyText.Replace("\n", ",");
            var lastChar = speakText[speakText.Length - 1];
            if (lastChar == '\n' || lastChar == ',')
            {
                speakText.Remove(speakText.Length - 1, 1);
                speakText += "。";
            }

            //TTSEngineAndroid.Speak(speakText);

            PlayTypeEffect(storyText); //播放打字效果

            // Speaker.Instance.Speak(storyText.Replace("\n", ","),
            //     null,
            //     Speaker.Instance.VoiceForCulture("zh-CN"), true,
            //     1f,
            //     0.5f);
        }
    }


    #region PrivateMethod

    private void PlayTypeEffect(string displayText)
    {
        if (_typeEffectCoroutine != null)
            StopCoroutine(_typeEffectCoroutine);
        _typeEffectCoroutine = StartCoroutine(ShowStoryTextWithTypeEffect(displayText));
    }

    private IEnumerator ShowStoryTextWithTypeEffect(string text)
    {
        var effectBuilder = new StringBuilder();
        for (var i = 0; i < text.Length; i++)
        {
            var storyChar = text[i];
            if (storyChar == '\n')
                yield return new WaitForSecondsRealtime(wrapDelayTime - _progressSpeedUp);

            var findChar = delayCharConfigs.Find(item => item.KeyChar.Equals(storyChar));
            if (findChar != null)
            {
                yield return new WaitForSecondsRealtime(findChar.DelayTime - _progressSpeedUp);
            }

            effectBuilder.Append(storyChar);
            if (textUI)
                textUI.text = effectBuilder.ToString();
            yield return new WaitForSecondsRealtime(textDelayTime - _progressSpeedUp);
            yield return new WaitForEndOfFrame();
        }

        if (_progressSpeedUp > 0)
            yield return new WaitForSecondsRealtime(Mathf.Clamp(_progressSpeedUp,0f,1f));
        ShowOption();
    }

    #endregion

    [Serializable]
    public class DelayCharConfig
    {
        [SerializeField] private char keyChar; //关键字符
        [SerializeField] private float delayTime; //延迟时间

        public char KeyChar => keyChar;
        public float DelayTime => delayTime;
    }
}