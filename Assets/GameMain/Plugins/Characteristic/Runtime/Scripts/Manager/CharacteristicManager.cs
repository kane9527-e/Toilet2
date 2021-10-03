using Characteristic.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characteristic.Runtime.Scripts.Manager
{
    [DisallowMultipleComponent]
    public class CharacteristicManager : MonoBehaviour
    {
        #region Singleton

        private static CharacteristicManager _instance;

        public static CharacteristicManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<CharacteristicManager>();
                    if (!_instance)
                        _instance = new GameObject(nameof(CharacteristicManager)).AddComponent<CharacteristicManager>();
                }

                return _instance;
            }
        }

        #endregion

        #region Field

        [SerializeField] private CharacteristicCollection currentCollection; //当前属性集合
        public static CharacteristicCollection Collection => Instance.currentCollection;
        public bool resetOnAwake;

        #endregion


        #region Event事件编写

        public delegate void CharacteristicEvent(CharacteristicUnit unit);

        public CharacteristicEvent onValueUpdate, onValueLess, onValueAdd;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if (Instance == this)
                DontDestroyOnLoad(this.gameObject);
            if (resetOnAwake && Collection)
                Collection.ResetAllValue();
        }

        #endregion

        #region Method

        public void SetData(CharacteristicUnit unit, object value)
        {
            if (!currentCollection.HasUnit(unit)) return;
            currentCollection.SetData(unit, value);
            onValueUpdate?.Invoke(unit);
        }

        public void LessData(CharacteristicUnit unit, object value)
        {
            if (!currentCollection.HasUnit(unit)) return;
            currentCollection.Less(unit, value);
            onValueLess?.Invoke(unit);
        }

        public void AddData(CharacteristicUnit unit, object value)
        {
            if (!currentCollection.HasUnit(unit)) return;
            currentCollection.Add(unit, value);
            onValueAdd?.Invoke(unit);
        }

        #endregion
    }
}