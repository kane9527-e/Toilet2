using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TabGroup : ToggleGroup
{
    [SerializeField] private List<Tab> tabs = new List<Tab>();

    protected override void Awake()
    {
        base.Awake();
        InitTabs();
    }

    private void InitTabs()
    {
        foreach (var tab in tabs)
            tab.group = this;
    }
}