// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using ConditionSetting;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Inventory.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(menuName = "Inventory/ItemUsableConfig", fileName = "New ItemUsableConfig")]
    public class ItemUsableConfig : UnityEngine.ScriptableObject
    {
        [SerializeField] private List<UsableOption> usableOptions = new List<UsableOption>();
        public List<UsableOption> UsableOptions => usableOptions;

        [Serializable]
        public class UsableOption
        {
            [SerializeField] private string optionName;
            [SerializeField] private ConditionConfig conditionConfig;

            [SerializeField] private UnityEvent action;

            //[SerializeField] private bool reduceAfterUse;
            public string OptionName => optionName;
            public ConditionConfig ConditionConfig => conditionConfig;

            public UnityEvent Action => action;
            //public bool ReduceAfterUse => reduceAfterUse;

            public void Use()
            {
                if (!ConditionConfig.Result()) return;
                Action?.Invoke();
            }
        }
    }
}