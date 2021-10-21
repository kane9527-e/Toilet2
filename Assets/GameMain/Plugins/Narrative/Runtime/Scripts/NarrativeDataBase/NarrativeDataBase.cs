using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.NarrativeDataBase
{
    [CreateAssetMenu(menuName = "Narrative/DataBase", fileName = "New NarrativeDataBase")]
    [Serializable]
    public class NarrativeDataBase : ScriptableObject
    {
        //private DataBaseContainer _dataBaseContainer;
        //[HideInInspector] [SerializeField] private List<string> savedkeys = new List<string>();

        [SerializeField] private List<NarrativeData> datas = new List<NarrativeData>();

        //private readonly string _saveKey = "NarrativeDataBase";

        public Action<string> OndatasChanged;
        //public List<string> SavedKeys => savedkeys;


        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData(string key, object value)
        {
            var data = (datas.Find(item => item.key == key));
            if (data == null)
            {
                data = NarrativeData.Create(key, value);
                datas.Add(data);
            }
            else
            {
                data.value = value.ToString();
            }

            FireDataChangeEvent();
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        /// <param name="key"></param>
        public void ClearData(string key)
        {
            datas.RemoveAll(item => item.key == key);
            FireDataChangeEvent();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            var findData = datas.Find(item => item.key == key);
            if (findData == null) return default;
            return (T)Convert.ChangeType(findData.value, typeof(T));
        }

        #region PublicMethod

        /// <summary>
        /// 初始化所有数据
        /// </summary>
        public void InitData(string dataJson)
        {
            if (!string.IsNullOrWhiteSpace(dataJson))
                datas = JsonUtil.FromJson<List<NarrativeData>>(dataJson);
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void ClearAllData()
        {
            datas.Clear();
            FireDataChangeEvent();
        }

        #endregion

        #region PrivateMethod

        private void FireDataChangeEvent()
        {
            OndatasChanged?.Invoke(JsonUtil.ToJson(datas));
        }

        #endregion
    }

    [Serializable]
    public class NarrativeData
    {
        public string key;
        public string type;
        public string value;

        private NarrativeData(string key, object value)
        {
            this.key = key;
            type = value.GetType().Name;
            this.value = value.ToString();
        }

        public static NarrativeData Create(string key, object value)
        {
            return new NarrativeData(key, value);
        }
    }
}