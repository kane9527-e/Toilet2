using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class AutoReferenceResolution : MonoBehaviour
{
    private void Awake()
    {
        var _scaler = GetComponent<CanvasScaler>();
        _scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        Destroy(this);
    }
}
