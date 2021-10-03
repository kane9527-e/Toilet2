using System;
using System.Collections.Generic;
using Narrative.Runtime.Scripts.NarrativeDataBase;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.NarrativeDataBase
{
    [CreateAssetMenu(menuName = "Narrative/DataBase", fileName = "New NarrativeDataBase")]
    [Serializable]
    public class NarrativeDataBase : ScriptableObject
    {
        [HideInInspector] [SerializeField] private List<string> savedkeys = new List<string>();
        public List<string> SavedKeys => savedkeys;

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

        public virtual int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public virtual float GetFloat(string key, float defaultValue = 0.0f) => PlayerPrefs.GetFloat(key, defaultValue);

        public virtual string GetString(string key, string defaultValue = "") =>
            PlayerPrefs.GetString(key, defaultValue);

        public virtual bool GetBool(string key, bool defaultValue = false) =>
            Convert.ToBoolean(PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue)));


        protected void SaveKey(string key)
        {
            if (!savedkeys.Contains(key))
                savedkeys.Add(key);
        }

        #region PublicMethod

        public void ClearAllData()
        {
            for (int i = SavedKeys.Count - 1; i >= 0; i--)
            {
                DeleteKey(SavedKeys[i]);
            }
        }

        public virtual void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
            savedkeys.Remove(key);
        }

        #endregion
    }
}