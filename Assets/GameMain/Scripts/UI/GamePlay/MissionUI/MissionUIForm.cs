using System;
using System.Collections.Generic;
using GameMain.Scripts.Runtime.Message;
using MissionSystem.Runtime.Scripts.Manager;
using MissionSystem.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Scripts.UI.GamePlay
{
    public class MissionUIForm : UIFormLogic
    {
        [SerializeField] private RectTransform rootTrans;
        [SerializeField] private GameObject missionDisplayUIPrefab;

        private List<MissionDisplayUI> _displayUis = new List<MissionDisplayUI>();
        private MissionManager _manager;

        private void Awake()
        {
            OnInit(this);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            _manager = MissionManager.Instance;
            
            _manager.OnMissionAdded += OnMissionAddedHandler;
            _manager.OnMissionRemoved += OnMissionRemovedHandler;
            _manager.OnCurrentMissionInit += OnCurrentMissionInitHandler;
            _manager.OnMissionCompleted += OnMissionCompleteHandler;
        }

        


        #region PrivateMethod

        private void UpdateAllMission(List<MissionConfig> missionConfigs)
        {
            foreach (var config in missionConfigs)
            {
                CreateCurrentMissionDisplay(config);
            }
        }


        private void CreateCurrentMissionDisplay(MissionConfig config)
        {
            if (!missionDisplayUIPrefab || !rootTrans) return;
            if (_displayUis.Exists(item => item.Config == config)) return;
            var displayObj = Instantiate(missionDisplayUIPrefab, rootTrans);
            MissionDisplayUI displayUI;
            if (!displayObj.TryGetComponent(out displayUI))
                displayUI = displayObj.AddComponent<MissionDisplayUI>();
            displayUI.Init(config);
            _displayUis.Add(displayUI);
        }

        private void RemoveMissionDisplay(MissionConfig config)
        {
            var display = _displayUis.Find(item => item.Config == config);
            if (!display) return;
            _displayUis.Remove(display);
            Destroy(display.gameObject);
        }

        #endregion

        #region EventHandler
        
        private void OnCurrentMissionInitHandler(MissionConfig config)
        {
            CreateCurrentMissionDisplay(config);
        }

        private void OnMissionAddedHandler(MissionConfig config)
        {
            CreateCurrentMissionDisplay(config);
                MessageSystem.PushMessage(string.Format("<b><size=15>任务</size></b>\n{0}\n{1}",
                    config.MissionTitle,
                    config.MissionIntro));
        }

        private void OnMissionRemovedHandler(MissionConfig config)
        {
            RemoveMissionDisplay(config);
        }


        private void OnMissionCompleteHandler(MissionConfig config)
        {
            MessageSystem.PushMessage(string.Format("<b><size=15>任务完成</size></b>\n{0}\n{1}", config.MissionTitle,
                config.CompleteIntro));
            RemoveMissionDisplay(config);
        }

        #endregion
    }
}