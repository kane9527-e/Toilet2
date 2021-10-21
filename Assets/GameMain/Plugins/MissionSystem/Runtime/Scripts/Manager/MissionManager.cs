using System;
using System.Collections.Generic;
using GameMain.Scripts.Runtime.Message;
using MissionSystem.Runtime.Scripts.ScriptableObject;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace MissionSystem.Runtime.Scripts.Manager
{
    [DisallowMultipleComponent]
    public class MissionManager : MonoBehaviour
    {
        #region Singleton

        private static MissionManager _instance;

        public static MissionManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<MissionManager>();
                    if (!_instance)
                        _instance = new GameObject(nameof(MissionManager)).AddComponent<MissionManager>();
                }

                return _instance;
            }
        }

        #endregion

        #region Field

        private MissionLine _missionLine = new MissionLine();

        public MissionLine MissionLine => _missionLine;
        //private List<MissionConfig> _missionList = new List<MissionConfig>(); //任务队列

        public Action<MissionConfig> OnCompleteMissionInit;
        public Action<MissionConfig> OnCurrentMissionInit;

        public Action<MissionConfig> OnMissionAdded;
        public Action<MissionConfig> OnMissionCompleted;
        public Action<MissionConfig> OnMissionRemoved;

        #endregion

        #region Property

        public int CurrentMissionCount => _missionLine.currentMissions.Count;
        public int CompleteMissionCount => _missionLine.completeMissions.Count;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if (Instance == this)
                DontDestroyOnLoad(gameObject);
        }

        private void LateUpdate()
        {
            CheckAllCurrentMission();
        }

        #endregion

        #region PublicMethod

        /// <summary>
        /// 初始化完成任务
        /// </summary>
        /// <param name="missionConfig"></param>
        public void InitCompleteMission(MissionConfig missionConfig)
        {
            if (!_missionLine.completeMissions.Contains(missionConfig))
            {
                _missionLine.completeMissions.Add(missionConfig);
                OnCompleteMissionInit?.Invoke(missionConfig);
            }
        }

        /// <summary>
        /// 增加任务
        /// </summary>
        /// <param name="missionConfig"></param>
        public void InitCurrentMission(MissionConfig missionConfig)
        {
            if (!missionConfig) return;
            if (!_missionLine.currentMissions.Contains(missionConfig))
            {
                missionConfig.onCompleteCallBack += MissionCompletedHandler;
                _missionLine.currentMissions.Add(missionConfig);
                OnCurrentMissionInit?.Invoke(missionConfig);
            }
        }


        /// <summary>
        /// 增加任务
        /// </summary>
        /// <param name="missionConfig"></param>
        public void AddMission(MissionConfig missionConfig)
        {
            //TODO 增加延迟添加任务，需要等待Mission初始化完毕以后再添加

            if (_missionLine.completeMissions.Contains(missionConfig)) return; //如果任务已经完成则不再添加
            if (!_missionLine.currentMissions.Contains(missionConfig))
            {
                missionConfig.onCompleteCallBack += MissionCompletedHandler;
                _missionLine.currentMissions.Add(missionConfig);
                OnMissionAdded?.Invoke(missionConfig);
            }
        }

        /// <summary>
        /// 检查所有任务
        /// </summary>
        public void CheckAllCurrentMission()
        {
            if (_missionLine == null) return;

            for (int i = _missionLine.currentMissions.Count - 1; i >= 0; i--)
            {
                var mission = _missionLine.currentMissions[i];
                if (mission)
                {
                    if (!mission.Completed && mission.CanComplete())
                        mission.Complete();
                }
            }
        }

        #endregion

        #region PrivateMethod

        private void RemoveCurrentMission(MissionConfig missionConfig)
        {
            if (_missionLine.currentMissions.Contains(missionConfig))
                _missionLine.currentMissions.Remove(missionConfig);

            OnMissionRemoved?.Invoke(missionConfig);
        }

        private void MissionCompletedHandler(MissionConfig config)
        {
            _missionLine.currentMissions.Remove(config);

            if (!_missionLine.completeMissions.Contains(config))
                _missionLine.completeMissions.Add(config);


            OnMissionCompleted?.Invoke(config);
        }

        #endregion
    }
}