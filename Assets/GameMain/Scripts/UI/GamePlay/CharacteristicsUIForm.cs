using Characteristic.Runtime.Scripts.Manager;
using Characteristic.Runtime.Scripts.ScriptableObject;
using GameMain.Scripts.Runtime.Message;
using UnityEngine;
using UnityGameFramework.Runtime;

public class CharacteristicsUIForm : UIFormLogic
{
    [SerializeField] private RectTransform root;
    [SerializeField] private GameObject unitPrefab;

    private void Awake()
    {
        //事件注册
        //初始化属性UI
        InitializationUI();
    }

    private void InitializationUI()
    {
        var collection = CharacteristicManager.Collection;
        if (!collection) return;
        if (!root) return;
        foreach (var unit in collection.CharacteristicUnits) CreateCharacteristicUI(unit);

        CharacteristicManager.Instance.onValueAdd += OnValueAddedHandler;
        CharacteristicManager.Instance.onValueLess += OnValueLessHandler;
    }


    private void CreateCharacteristicUI(CharacteristicUnit unit)
    {
        if (!root) return;
        if (!unitPrefab) return;
        var unitUI = Instantiate(unitPrefab, root).GetComponent<CharacteristicUnitValueUI>();
        unitUI.Init(unit);
    }

    private void OnValueAddedHandler(CharacteristicUnit unit)
    {
        if (unit.CharacteristicType == CharacteristicType.Int || unit.CharacteristicType == CharacteristicType.Float)
        {
            var addValue = 0f;

            if (unit.CharacteristicType == CharacteristicType.Int)
                addValue = unit.intvalue - unit.IntvalueLast;

            if (unit.CharacteristicType == CharacteristicType.Float)
                addValue = unit.floatvalue - unit.FloatvalueLast;

            var msg = string.Format("{0}增加了\n{1}点", unit.name, addValue);
            if (addValue == 0)
                msg = string.Format("{0}\n已满", unit.name);
            MessageSystem.PushMessage(msg);
        }
    }

    private void OnValueLessHandler(CharacteristicUnit unit)
    {
        if (unit.CharacteristicType == CharacteristicType.Int || unit.CharacteristicType == CharacteristicType.Float)
        {
            var lessValue = 0f;
            if (unit.CharacteristicType == CharacteristicType.Int)
                lessValue = unit.IntvalueLast - unit.intvalue;
            if (unit.CharacteristicType == CharacteristicType.Float)
                lessValue = unit.FloatvalueLast - unit.floatvalue;
            var msg = string.Format("{0}减少了\n{1}点", unit.name, lessValue);
            if (lessValue == 0)
                msg = string.Format("{0}\n为零", unit.name);
            MessageSystem.PushMessage(msg);
        }
    }
}