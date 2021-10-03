using System;
using System.Collections.Generic;
using ConditionSetting;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace Narrative.Runtime.Scripts.EventConfig
{
    [CreateAssetMenu(menuName = "Narrative/EventConfig/EventConfig", fileName = "New EventConfig")]
    public class EventConfig : ScriptableObject
    {
        [SerializeField] private List<ConditionEvent> events;

        public virtual void Trigger()
        {
            foreach (var @event in events)
            {
                @event.CheckConditionInvokeEvent();
            }
        }

        [Serializable]
        public class ConditionEvent
        {
            [SerializeField] private ConditionSetting.ConditionConfig conditionConfig;
            [SerializeField] private UnityEvent nodeEvent;

            public void CheckConditionInvokeEvent()
            {
                if (conditionConfig == null || conditionConfig.Result())
                {
                    nodeEvent?.Invoke();
                }
            }
        }
    }
}