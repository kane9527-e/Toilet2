// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using ConditionSetting;
using Narrative.Runtime.Scripts.MonoBehaviour;
using Narrative.Runtime.Scripts.Nodes.TriggerNode;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.ConditionConfig
{
    [CreateAssetMenu(menuName = "Narrative/ConditionConfig/DataCondition", fileName = "New DataConditionConfig")]
    public class DataConditionConfig : ConditionSetting.ConditionConfig
    {
        [SerializeField] private List<DataCondition> dataConditions = new List<DataCondition>();

        public override bool Result()
        {
            var r = false;
            foreach (var condition in dataConditions)
                switch (condition.ConditionType)
                {
                    case ConditionType.And:
                        r = condition.GetResult();
                        if (!r) return false;
                        break;
                    case ConditionType.Or:
                        r = condition.GetResult();
                        if (r) return true;
                        break;
                }

            return r;
        }
    }

    [Serializable]
    public class DataCondition
    {
        [SerializeField] private string keyName;
        [SerializeField] private ConditionType conditionType;
        [SerializeField] private DataValueType dataValueType;
        [SerializeField] private RelationalType relationalType;
        [HideInInspector] [SerializeField] private int intValue;
        [HideInInspector] [SerializeField] private float floatValue;
        [HideInInspector] [SerializeField] private bool boolValue;
        [HideInInspector] [SerializeField] private string stringValue;

        public ConditionType ConditionType => conditionType;

        public bool GetResult()
        {
            return GetResult(keyName);
        }

        public bool GetResult(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            var dataBase = NarrativeManager.Instance.DataBase;
            if (!dataBase) return false;
            switch (relationalType)
            {
                case RelationalType.Equal:
                    switch (dataValueType)
                    {
#pragma warning disable 162
                        case DataValueType.Int:
                            return dataBase.GetData<int>(key) == intValue;
                            break;
                        case DataValueType.Float:
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            return dataBase.GetData<float>(key) == floatValue;
                            break;
                        case DataValueType.Bool:
                            return dataBase.GetData<bool>(key) == boolValue;
                            break;
                        case DataValueType.String:
                            return dataBase.GetData<string>(key) == stringValue;
                            break;
#pragma warning restore 162
                    }

                    break;
                case RelationalType.Greater:
                    switch (dataValueType)
                    {
#pragma warning disable 162
                        case DataValueType.Int:
                            return dataBase.GetData<int>(key) > intValue;
                            break;
                        case DataValueType.Float:
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            return dataBase.GetData<float>(key) > floatValue;
                            break;
                        case DataValueType.Bool:
                            return Convert.ToInt32(dataBase.GetData<bool>(key)) > Convert.ToInt32(boolValue);
                            break;
                        case DataValueType.String:
                            return string.Compare(dataBase.GetData<string>(key), stringValue,
                                StringComparison.Ordinal) > 0;
                            break;
#pragma warning restore 162
                    }

                    break;
                case RelationalType.GreaterEqual:
                    switch (dataValueType)
                    {
#pragma warning disable 162
                        case DataValueType.Int:
                            return dataBase.GetData<int>(key) >= intValue;
                            break;
                        case DataValueType.Float:
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            return dataBase.GetData<float>(key) >= floatValue;
                            break;
                        case DataValueType.Bool:
                            return Convert.ToInt32(dataBase.GetData<bool>(key)) >= Convert.ToInt32(boolValue);
                            break;
                        case DataValueType.String:
                            return string.Compare(dataBase.GetData<string>(key), stringValue,
                                StringComparison.Ordinal) >= 0;
                            break;
#pragma warning restore 162
                    }

                    break;
                case RelationalType.Less:
                    switch (dataValueType)
                    {
#pragma warning disable 162
                        case DataValueType.Int:
                            return dataBase.GetData<int>(key) < intValue;
                            break;
                        case DataValueType.Float:
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            return dataBase.GetData<float>(key) < floatValue;
                            break;
                        case DataValueType.Bool:
                            return Convert.ToInt32(dataBase.GetData<bool>(key)) < Convert.ToInt32(boolValue);
                            break;
                        case DataValueType.String:
                            return string.Compare(dataBase.GetData<string>(key), stringValue,
                                StringComparison.Ordinal) < 0;
                            break;
#pragma warning restore 162
                    }

                    break;
                case RelationalType.LessEqual:
                    switch (dataValueType)
                    {
#pragma warning disable 162
                        case DataValueType.Int:
                            return dataBase.GetData<int>(key) <= intValue;
                            break;
                        case DataValueType.Float:
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            return dataBase.GetData<float>(key) <= floatValue;
                            break;
                        case DataValueType.Bool:
                            return Convert.ToInt32(dataBase.GetData<bool>(key)) <= Convert.ToInt32(boolValue);
                            break;
                        case DataValueType.String:
                            return string.Compare(dataBase.GetData<string>(key), stringValue,
                                StringComparison.Ordinal) <= 0;
                            break;
#pragma warning restore 162
                    }

                    break;
            }

            return false;
        }
    }

    public enum RelationalType
    {
        Equal,
        Greater,
        GreaterEqual,
        Less,
        LessEqual
    }
}