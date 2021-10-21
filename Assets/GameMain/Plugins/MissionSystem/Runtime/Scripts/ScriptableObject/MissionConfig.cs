// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using ConditionSetting;
using MissionSystem.Runtime.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MissionSystem.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(menuName = "Mission/Config", fileName = "New MissionConfig")]
    [Serializable]
    public class MissionConfig : UnityEngine.ScriptableObject
    {
        #region Field

        [SerializeField] private string missionTitle; //任务标题
        [TextArea(1, 5)] [SerializeField] private string missionIntro; //任务介绍
        [TextArea(1, 5)] [SerializeField] private string completeIntro; //完成介绍

        [SerializeField] private ConditionConfig compeleteCondition;
        [SerializeField] private UnityEvent onCompleteEvent;
        
        // ReSharper disable once InconsistentNaming
        public Action<MissionConfig> onCompleteCallBack;

        #endregion

        #region Property

        public bool Completed { get; private set; }//是否完成

        public string MissionTitle => missionTitle;
        public string MissionIntro => missionIntro;
        public string CompleteIntro => completeIntro;
        #endregion


        /// <summary>
        /// 是否可以完成
        /// </summary>
        /// <returns></returns>
        public bool CanComplete() => compeleteCondition && compeleteCondition.Result();

        public void Complete()
        {
            if (!CanComplete()||Completed) return;
            onCompleteEvent?.Invoke(); //执行完成事件
            onCompleteCallBack?.Invoke(this); //执行完成回调函数
            Completed = true;
        }

        public void AddThisMission()
        {
            var manager = MissionManager.Instance;
            // ReSharper disable once Unity.NoNullPropagation
            manager?.AddMission(this);
            Completed = false;
        }
    }
}