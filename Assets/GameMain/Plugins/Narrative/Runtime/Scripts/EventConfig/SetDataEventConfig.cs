// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using Narrative.Runtime.Scripts.MonoBehaviour;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.Nodes.TriggerNode
{
    [CreateAssetMenu(menuName = "Narrative/EventConfig/SetDataConfig", fileName = "New SetDataEventConfig")]
    public class SetDataEventConfig : EventConfig.EventConfig
    {
        [SerializeField] private List<SetDataEvent> setDataEvents = new List<SetDataEvent>();

        public override void Trigger()
        {
            //base.Trigger();
            foreach (var @event in setDataEvents) @event?.ChangeData();
        }
    }

    [Serializable]
    public class SetDataEvent
    {
        [SerializeField] private string keyName;
        [SerializeField] private SetDataActionType actionType;
        [SerializeField] private DataValueType valueType;
        [HideInInspector] [SerializeField] private int intValue;
        [HideInInspector] [SerializeField] private float floatValue;
        [HideInInspector] [SerializeField] private bool boolValue;
        [HideInInspector] [SerializeField] private string stringValue;

        public void ChangeData()
        {
            ChangeData(keyName);
        }

        public void ChangeData(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var dataBase = NarrativeManager.Instance.DataBase;
            if (!dataBase) return;
            switch (actionType)
            {
                case SetDataActionType.Cover:
                    switch (valueType)
                    {
                        case DataValueType.Int:
                            dataBase.SetInt(key, intValue);
                            break;
                        case DataValueType.Float:
                            dataBase.SetFloat(key, floatValue);
                            break;
                        case DataValueType.Bool:
                            dataBase.SetBool(key, boolValue);
                            break;
                        case DataValueType.String:
                            dataBase.SetString(key, stringValue);
                            break;
                    }

                    break;
                case SetDataActionType.Add:
                    switch (valueType)
                    {
                        case DataValueType.Int:
                            dataBase.SetInt(key, dataBase.GetInt(key) + intValue);
                            break;
                        case DataValueType.Float:
                            dataBase.GetFloat(key, dataBase.GetFloat(key) + floatValue);
                            break;
                        case DataValueType.Bool:
                            var boolValue = dataBase.GetBool(key);
                            var setValue = Convert.ToInt32(boolValue) + Convert.ToInt32(boolValue);
                            setValue = setValue > 1 ? 1 : setValue < 0 ? 0 : setValue;
                            dataBase.SetBool(key, Convert.ToBoolean(setValue));
                            break;
                        case DataValueType.String:
                            dataBase.SetString(key, dataBase.GetString(key) + stringValue);
                            break;
                    }

                    break;
                case SetDataActionType.Less:
                    switch (valueType)
                    {
                        case DataValueType.Int:
                            dataBase.SetInt(key, dataBase.GetInt(key) - intValue);
                            break;
                        case DataValueType.Float:
                            dataBase.GetFloat(key, dataBase.GetFloat(key) - floatValue);
                            break;
                        case DataValueType.Bool:
                            var boolValue = dataBase.GetBool(key);
                            var setValue = Convert.ToInt32(boolValue) - Convert.ToInt32(boolValue);
                            setValue = setValue > 1 ? 1 : setValue < 0 ? 0 : setValue;
                            dataBase.SetBool(key, Convert.ToBoolean(setValue));
                            break;
                        case DataValueType.String:
                            dataBase.SetString(key, dataBase.GetString(key).Replace(stringValue, string.Empty));
                            break;
                    }

                    break;
            }
        }
    }

    public enum SetDataActionType
    {
        Cover,
        Add,
        Less
    }

    public enum DataValueType
    {
        Int,
        Float,
        Bool,
        String
    }
}