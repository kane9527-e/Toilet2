using System;
using System.Collections.Generic;
using GameMain.Scripts.Base.Struct;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Scripts.Runtime.Base.GameEntry;

namespace GameMain.Scripts.Component.GameFramework
{
    /// <summary>
    ///     进度组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Progress")]
    public class ProgressComponent : GameFrameworkComponent
    {
        public string SaveProgressName { get; private set; }
        private readonly string _saveNamekey = "GameProgressName";


        private Dictionary<string, Progress> _progresses = new Dictionary<string, Progress>();

        public Progress GetProgress(string key) => _progresses[key];

        public void Init()
        {
            if (GameEntry.Setting.HasSetting(_saveNamekey))
                SaveProgressName = GameEntry.Setting.GetString(_saveNamekey);
        }

        /// <summary>
        /// 保存进度
        /// </summary>
        public void SaveProgress(Progress progress)
        {
            GameEntry.Setting.SetObject(progress.name, progress);
            GameEntry.Setting.Save();
        }

        /// <summary>
        /// 加载进度
        /// </summary>
        public Progress LoadProgress(string name)
        {
            if (!HasProgress(name))
                return null;
            var progress = GameEntry.Setting.GetObject<Progress>(name);
            if (progress == null) return null;

            if (!_progresses.ContainsKey(progress.name))
                _progresses.Add(progress.name, progress);
            return progress;
        }

        /// <summary>
        /// 设置当前进度名称
        /// </summary>
        /// <param name="name"></param>
        public void SetCurrentProgressName(string name)
        {
            SaveProgressName = name;
        }

        public bool HasProgress(string name) => GameEntry.Setting.HasSetting(name);

        // private void OnApplicationQuit()
        // {
        //     OnApplicationPause(true);
        // }
        //
        // private void OnApplicationPause(bool pauseStatus)
        // {
        //     GameEntry.Setting.SetString(_saveNamekey, SaveProgressName);
        //     foreach (var valuePair in _progresses)
        //         SaveProgress(valuePair.Value);
        // }
    }
}