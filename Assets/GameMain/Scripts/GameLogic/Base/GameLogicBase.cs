using System;
using GameMain.Scripts.Base.Struct;
using GameMain.Scripts.Runtime.Base;

namespace GameMain.Scripts.GameLogic.Base
{
    public class GameLogicBase
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once UnassignedGetOnlyAutoProperty

        protected Progress _progress;
        public virtual Progress Progress { get=> _progress; set => _progress = value; }
        
        public virtual void OnEnter()
        {
            GameEntry.Progress.SetCurrentProgressName(Progress.name);
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnLeave()
        {
        }
    }
}