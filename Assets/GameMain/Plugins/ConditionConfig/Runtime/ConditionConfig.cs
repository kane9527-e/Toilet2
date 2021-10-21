using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ConditionSetting
{
    [CreateAssetMenu(menuName = "ConditionConfig/Config", fileName = "New ConditionConfig")]
    public class ConditionConfig : ScriptableObject
    {
        [SerializeField] private List<Condition> conditions = new List<Condition>();

        public virtual bool Result()
        {
            var r = false;
            foreach (var condition in conditions)
                switch (condition.Type)
                {
                    case ConditionType.And:
                        r = condition.Result;
                        if (!r) return false;
                        break;
                    case ConditionType.Or:
                        r = condition.Result;
                        if (r) return true;
                        break;
                }

            return r;
        }
    }

    public enum ConditionType
    {
        And,
        Or
    }

    [Serializable]
    public class Condition
    {
        [SerializeField] private ConditionType type;
        [SerializeField] private BooleanCondition callback;
        [SerializeField] private bool value = true;
        public ConditionType Type => type;
        public bool Result => callback.Invoke() == value;
    }

    [Serializable]
    public class BooleanCondition : SerializableCallback<bool>
    {
    }
}