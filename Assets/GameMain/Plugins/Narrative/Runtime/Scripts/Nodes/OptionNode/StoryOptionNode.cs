using UnityEngine;
using VisualGraphRuntime;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.OptionNode
{
    [NodeName("Options/StoryOption")]
    public class StoryOptionNode : global::OptionNode
    {
        [TextArea(1, 3)] [SerializeField] private string optionText;

        public string OptionText
        {
            get => optionText;
            set => optionText = value;
        }
    }
}