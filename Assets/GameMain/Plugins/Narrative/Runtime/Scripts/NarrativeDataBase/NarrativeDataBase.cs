using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.NarrativeDataBase
{
    [CreateAssetMenu(menuName = "Narrative/DataBase", fileName = "New NarrativeDataBase")]
    [Serializable]
    public class NarrativeDataBase : ScriptableObject
    {
        //private DataBaseContainer _dataBaseContainer;

        [HideInInspector] [SerializeField] private List<string> savedkeys = new List<string>();

        public List<string> SavedKeys => savedkeys;

        private readonly string _saveKeysKey = "NarrativeDataBaseKeys";

        public void LoadDataKeys()
        {
            var loadKeys = PlayerPrefs.GetString(_saveKeysKey, String.Empty);
            loadKeys = loadKeys.TrimStart('\n');
            loadKeys = loadKeys.TrimEnd('\n');
            if (!string.IsNullOrWhiteSpace(loadKeys))
                savedkeys = loadKeys.Split('\n').ToList();
        }

        public void SaveDataKeys()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var savedkey in savedkeys)
                builder.AppendLine(savedkey);

            var result = builder.ToString();
            result = result.TrimStart('\n');
            result = result.TrimEnd('\n');
            PlayerPrefs.SetString(_saveKeysKey, result);
        }

        public virtual void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
            SaveKey(key);
        }

        public virtual void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
            SaveKey(key);
        }

        public virtual void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
            SaveKey(key);
        }

        public virtual void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, Convert.ToInt32(value));
            PlayerPrefs.Save();
            SaveKey(key);
        }

        public virtual int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public virtual float GetFloat(string key, float defaultValue = 0.0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public virtual string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public virtual bool GetBool(string key, bool defaultValue = false)
        {
            return Convert.ToBoolean(PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue)));
        }


        protected void SaveKey(string key)
        {
            if (!savedkeys.Contains(key))
                savedkeys.Add(key);
            SaveDataKeys();
        }

        #region PublicMethod

        public void ClearAllData()
        {
            if (savedkeys.Count <= 0)
                LoadDataKeys();
            for (var i = SavedKeys.Count - 1; i >= 0; i--) DeleteKey(SavedKeys[i]);
            PlayerPrefs.DeleteKey(_saveKeysKey);
        }

        public virtual void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            savedkeys.Remove(key);
        }

        #endregion
    }

    // [Serializable]
    // public class DataBaseContainer
    // {
    //     public Dictionary<string, int> IntData = new Dictionary<string, int>();
    //     public Dictionary<string, float> FloatData = new Dictionary<string, float>();
    //     public Dictionary<string, string> StringData = new Dictionary<string, string>();
    //     public Dictionary<string, bool> BoolData = new Dictionary<string, bool>();
    // }
}