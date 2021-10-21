using System;
using System.Collections.Generic;
using GameMain.Scripts.Runtime.Base;
using UnityEngine;


namespace GameMain.Scripts.Base.Struct
{
    [Serializable]
    public class Progress
    {
        public string name; //名称
        public int sceneId; //场景Id
        private Dictionary<string, object> _dataDic = new Dictionary<string, object>();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            if (!HasData(key)) return default;
            return (T)_dataDic[key];
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetData(string key, object data)
        {
            if (!HasData(key))
                _dataDic.Add(key, data);
            else
            {
                _dataDic[key] = data;
            }
            Save();
        }

        public bool HasData(string key) => _dataDic.ContainsKey(key);

        private void Save()
        {
            GameEntry.Setting.SetObject(this.name, this);
            GameEntry.Setting.Save();
        }
    }
}