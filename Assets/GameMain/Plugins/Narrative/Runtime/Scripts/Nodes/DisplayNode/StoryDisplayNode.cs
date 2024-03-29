using System;
using System.Collections.Generic;
using System.Text;
using TextVariable;
using UnityEngine;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.DisplayNode
{
    [NodeName("DisplayNode/Story Node")]
    public class StoryDisplayNode : BaseNode.DisplayNode
    {
        [SerializeField] private List<ConditionText> storyTexts;

        //[TextArea(1, 10)] [SerializeField] private string storyText;
        [HideInInspector] [SerializeField] private Texture2D backgroundImage;

        // ReSharper disable once NotAccessedField.Local
        [HideInInspector] [SerializeField] private AudioClip sound;

        public List<ConditionText> StoryTexts => storyTexts;

        public Texture2D BackgroundImage
        {
            get => backgroundImage;
            set => backgroundImage = value;
        }

        public AudioClip Sound
        {
            get => sound;
            set => sound = value;
        }


        public virtual string GetStoryText()
        {
            var builder = new StringBuilder();
            foreach (var conditionText in storyTexts)
                if (conditionText.ConditionConfig == null || conditionText.ConditionConfig.Result())
                    builder.AppendLine(conditionText.StoryText);

            return TextVariableProcessor.ProcessVariable(builder.ToString());
        }

        public virtual string GetLine(int index)
        {
            if (storyTexts == null || storyTexts.Count <= 0 || index < 0 || index > storyTexts.Count)
                return string.Empty;
            var text = storyTexts[index].StoryText;
            var lines = text.Split('\n');
            if (lines.Length <= 0)
                return string.Empty;
            return lines[0];
        }
    }

    [Serializable]
    public class ConditionText
    {
        [SerializeField] private ConditionSetting.ConditionConfig conditionConfig;
        [TextArea(1, 10)] [SerializeField] private string storyText;
        public string StoryText => storyText;
        public ConditionSetting.ConditionConfig ConditionConfig => conditionConfig;
    }
}