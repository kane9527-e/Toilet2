using System;
using Characteristic.Runtime.Scripts.Manager;
using UnityEngine;

namespace Characteristic.Runtime.Scripts.ScriptableObject
{
    [CreateAssetMenu(fileName = "New Characteristic Unit", menuName = "Characteristic/Unit")]
    public class CharacteristicUnit : UnityEngine.ScriptableObject
    {
        [SerializeField] private CharacteristicUnitUIConfig config;
        [SerializeField] private CharacteristicType type;
        [HideInInspector] public int intvalue;
        [HideInInspector] public float floatvalue;
        [HideInInspector] public string stringvalue;
        [HideInInspector] public bool boolvalue;

        [HideInInspector] public int intvalueMax;
        [HideInInspector] public float floatvalueMax;

        [HideInInspector] public int intDefaultValue;
        [HideInInspector] public float floatDefaultValue;
        [HideInInspector] public string stringDefaultValue;
        [HideInInspector] public bool boolDefaultValue;
        public CharacteristicType CharacteristicType => type;
        public CharacteristicUnitUIConfig Config => config;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int IntvalueLast { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public float FloatvalueLast { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string StringvalueLast { get; private set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public bool BoolvalueLast { get; private set; }

        public void Reset()
        {
            intvalue = intDefaultValue;
            floatvalue = floatDefaultValue;
            stringvalue = stringDefaultValue;
            boolvalue = boolDefaultValue;
        }

        public void SetValue(object value)
        {
            IntvalueLast = intvalue;
            FloatvalueLast = floatvalue;
            StringvalueLast = stringvalue;
            BoolvalueLast = boolvalue;

            switch (type)
            {
                case CharacteristicType.Int:
                    intvalue = (int)value;
                    intvalue = Mathf.Clamp(intvalue, 0, intvalueMax);
                    break;
                case CharacteristicType.Float:
                    floatvalue = (float)value;
                    floatvalue = Mathf.Clamp(floatvalue, 0f, floatvalueMax);
                    break;
                case CharacteristicType.String:
                    stringvalue = (string)value;
                    break;
                case CharacteristicType.Boolean:
                    boolvalue = (bool)value;
                    break;
            }
        }

        /// <summary>
        ///     减少
        /// </summary>
        /// <param name="value"></param>
        public void Less(object value)
        {
            IntvalueLast = intvalue;
            FloatvalueLast = floatvalue;
            StringvalueLast = stringvalue;
            BoolvalueLast = boolvalue;

            switch (type)
            {
                case CharacteristicType.Int:
                    intvalue -= Convert.ToInt32(value);
                    if (intvalue < 0) intvalue = 0;
                    intvalue = Mathf.Clamp(intvalue, 0, intvalueMax);
                    break;
                case CharacteristicType.Float:
                    floatvalue -= Convert.ToSingle(value);
                    floatvalue = Mathf.Clamp(floatvalue, 0f, floatvalueMax);
                    break;
            }
        }

        // ReSharper disable once FunctionRecursiveOnAllPaths
        public void LessValue(float value)
        {
            CharacteristicManager.Instance.LessData(this, value);
            //Less(value);
        }

        /// <summary>
        ///     增加
        /// </summary>
        /// <param name="value"></param>
        public void Add(object value)
        {
            IntvalueLast = intvalue;
            FloatvalueLast = floatvalue;
            StringvalueLast = stringvalue;
            BoolvalueLast = boolvalue;
            switch (type)
            {
                case CharacteristicType.Int:
                    intvalue += Convert.ToInt32(value);
                    intvalue = Mathf.Clamp(intvalue, 0, intvalueMax);
                    break;
                case CharacteristicType.Float:
                    floatvalue += Convert.ToSingle(value);
                    floatvalue = Mathf.Clamp(floatvalue, 0f, floatvalueMax);
                    break;
            }
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once FunctionRecursiveOnAllPaths
        public void AddValue(float value)
        {
            CharacteristicManager.Instance.AddData(this, value);
            //Add(value);
        }
    }


    public enum CharacteristicType
    {
        Int,
        Float,
        String,
        Boolean
    }
}