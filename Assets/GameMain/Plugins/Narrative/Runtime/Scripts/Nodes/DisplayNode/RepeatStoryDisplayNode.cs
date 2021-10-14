// ReSharper disable once CheckNamespace

using Narrative.Runtime.Scripts.MonoBehaviour;
using TextVariable;
using UnityEngine;
using VisualGraphRuntime;

namespace Narrative.Runtime.Scripts.Nodes.DisplayNode
{
    [NodeName("DisplayNode/RepeatStory Node")]
    public class RepeatStoryDisplayNode : StoryDisplayNode
    {
        [SerializeField] private string counterDataKeyName;
        [SerializeField] private EventConfig.EventConfig onEndEventConfig;

        public override void OnEnter()
        {
            base.OnEnter();
            var dataBase = NarrativeManager.Instance.DataBase;
            if (dataBase)
                dataBase.SetInt(counterDataKeyName, dataBase.GetInt(counterDataKeyName) + 1);
        }

        public override string GetStoryText()
        {
            var dataBase = NarrativeManager.Instance.DataBase;
            if (!dataBase) return string.Empty;
            var index = dataBase.GetInt(counterDataKeyName) - 1;
            index = Mathf.Clamp(index, 0, StoryTexts.Count - 1);
            return TextVariableProcessor.ProcessVariable(StoryTexts[index].StoryText);
        }

        public override void TriggerEvent()
        {
            var dataBase = NarrativeManager.Instance.DataBase;
            if (!dataBase) return;
            var index = dataBase.GetInt(counterDataKeyName);
            if (index >= StoryTexts.Count - 1)
                // ReSharper disable once Unity.NoNullPropagation
                onEndEventConfig?.Trigger();
        }
    }
}