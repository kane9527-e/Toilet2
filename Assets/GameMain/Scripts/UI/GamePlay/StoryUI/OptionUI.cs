using System;
using Narrative.Runtime.Scripts.Nodes.OptionNode;
using UnityEngine.UI;

public class OptionUI : Button
{
    private Action<int> makeOptionCalBack;
    private OptionNode optionNode;

    protected override void Awake()
    {
        base.Awake();
        onClick.AddListener(MakeOption);
    }

    private void MakeOption()
    {
        if (optionNode)
            optionNode.SwitchNextDefaultNode();
    }


    public void UpdateOption(OptionNode node)
    {
        optionNode = node;
        var textUI = (Text)targetGraphic;
        if (textUI && optionNode is StoryOptionNode) textUI.text = ((StoryOptionNode)optionNode).OptionText;
    }
}