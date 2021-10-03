using System;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace UI.GamePlay.StoryUI
{
    public abstract class DisplayUI : MonoBehaviour
    {
        public virtual Type DisplayNodeType => typeof(DisplayNode);
        public Action ShowDisplayFinishCallBack;
        public abstract void Display(DisplayNode node);
        
        protected void ShowOption()
        {
            ShowDisplayFinishCallBack?.Invoke();
        }
    }
}