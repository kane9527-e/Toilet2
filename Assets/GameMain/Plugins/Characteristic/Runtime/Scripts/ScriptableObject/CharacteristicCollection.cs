// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characteristic.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "New Characteristic Collection", menuName = "Characteristic/Collection")]
    public class CharacteristicCollection : UnityEngine.ScriptableObject
    {
        [SerializeField] private List<CharacteristicUnit> collection = new List<CharacteristicUnit>();

        public List<CharacteristicUnit> CharacteristicUnits => collection;

        public bool HasUnit(CharacteristicUnit unit)
        {
            if (collection == null || collection.Count <= 0)
                return false;
            return collection.Contains(unit);
        }

        public bool IsValueLess(CharacteristicUnit data, float value)
        {
            if (!data) return false;
            if (collection == null || !collection.Contains(data)) return false;
            switch (data.CharacteristicType)
            {
                case CharacteristicType.Float:
                    return data.floatvalue < value;
                case CharacteristicType.Int:
                    return data.intvalue < value;
                case CharacteristicType.Boolean:
                    return Convert.ToInt32(data.boolvalue) < value;
                case CharacteristicType.String:
                    return Convert.ToSingle(data.stringvalue) < value;
                default:
                    return false;
            }
        }

        public bool IsValueMore(CharacteristicUnit data, float value)
        {
            if (!data) return false;
            if (collection == null || !collection.Contains(data)) return false;
            switch (data.CharacteristicType)
            {
                case CharacteristicType.Float:
                    return data.floatvalue > value;
                case CharacteristicType.Int:
                    return data.intvalue > value;
                case CharacteristicType.Boolean:
                    return Convert.ToInt32(data.boolvalue) > value;
                case CharacteristicType.String:
                    return Convert.ToSingle(data.stringvalue) > value;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="value"></param>
        public void SetData(CharacteristicUnit unit, object value)
        {
            if (collection == null || collection.Count <= 0)
                return;
            var cUnit = collection.Find(x => x == unit);
            cUnit.SetValue(value);
        }

        public void Add(CharacteristicUnit unit, object value)
        {
            if (collection == null || collection.Count <= 0)
                return;
            var cUnit = collection.Find(x => x == unit);
            cUnit.Add(value);
        }

        public void Less(CharacteristicUnit unit, object value)
        {
            if (collection == null || collection.Count <= 0)
                return;
            var cUnit = collection.Find(x => x == unit);
            cUnit.Less(value);
        }

        public void ResetAllValue()
        {
            foreach (var unit in collection)
                unit.Reset();
        }
    }
}