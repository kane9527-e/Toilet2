using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Characteristic.Runtime.Scripts.Manager;
using Characteristic.Runtime.Scripts.ScriptableObject;
using UnityEngine;
using UnityEngine.UI;

public class CharacteristicUnitValueUI : MonoBehaviour
{
    [SerializeField] private Slider valueSlider;
    [SerializeField] private Text valueText;
    [SerializeField] private Image backGroundImage;
    private CharacteristicUnit _characteristicUnitunit;

    public void Init(CharacteristicUnit unit)
    {
        _characteristicUnitunit = unit;
        CharacteristicManager.Instance.onValueAdd += OnValueAddHandler;
        CharacteristicManager.Instance.onValueLess += OnValueLessHandler;
        CharacteristicManager.Instance.onValueUpdate += onValueUpdateHandler;
        OnInit();
    }

    private void onValueUpdateHandler(CharacteristicUnit unit)
    {
        if (!_characteristicUnitunit || unit != _characteristicUnitunit) return;
        UpdateValueUI();
    }

    private void OnValueLessHandler(CharacteristicUnit unit)
    {
        if (!_characteristicUnitunit || unit != _characteristicUnitunit) return;
        UpdateValueUI();
    }

    private void OnValueAddHandler(CharacteristicUnit unit)
    {
        if (!_characteristicUnitunit || unit != _characteristicUnitunit) return;
        UpdateValueUI();
    }

    private void OnInit()
    {
        if (!_characteristicUnitunit) return;
        switch (_characteristicUnitunit.CharacteristicType)
        {
            case CharacteristicType.Int:
                valueSlider.maxValue = _characteristicUnitunit.intvalueMax;
                break;
            case CharacteristicType.Float:
                valueSlider.maxValue = _characteristicUnitunit.floatvalueMax;
                break;
        }

        UpdateUIConfig();
        UpdateValueUI();
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    public void UpdateValueUI()
    {
        if (!valueSlider) return;
        if (!_characteristicUnitunit) return;
        switch (_characteristicUnitunit.CharacteristicType)
        {
            case CharacteristicType.Int:
                valueSlider.value = _characteristicUnitunit.intvalue;
                break;
            case CharacteristicType.Float:
                valueSlider.value = _characteristicUnitunit.floatvalue;
                break;
        }

        if (valueText)
            valueText.text = string.Format("{0}/{1}", valueSlider.value, valueSlider.maxValue);
    }

    public void UpdateUIConfig()
    {
        var iconGraph = valueSlider.targetGraphic as Image;
        if (iconGraph)
        {
            var iconTex = _characteristicUnitunit.Config.Icon;
            if (iconTex)
                iconGraph.sprite = Sprite.Create(iconTex, new Rect(0, 0, iconTex.width, iconTex.height), Vector2.zero);
            iconGraph.color = _characteristicUnitunit.Config.IconColor;
        }


        var fillImage = valueSlider.fillRect.GetComponent<Image>();
        if (fillImage)
        {
            var valueTex = _characteristicUnitunit.Config.ValueTexture2D;
            if (valueTex)
                fillImage.sprite =
                    Sprite.Create(valueTex, new Rect(0, 0, valueTex.width, valueTex.height), Vector2.zero);
            fillImage.color = _characteristicUnitunit.Config.ValueColor;
        }

        if (backGroundImage)
        {
            var backTex = _characteristicUnitunit.Config.BackTexture2D;
            if (backTex)
                backGroundImage.sprite =
                    Sprite.Create(backTex, new Rect(0, 0, backTex.width, backTex.height), Vector2.zero);
            backGroundImage.color = _characteristicUnitunit.Config.BackColor;
        }
    }
}