using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(Tab))]
public class TabInspectorEditor : ToggleEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var tabBarButtonProperty = serializedObject.FindProperty("tabBarButton");
        
        var onShowEventProperty = serializedObject.FindProperty("onShow");
        var onHideEventProperty = serializedObject.FindProperty("onHide");
        var fadeSpeedProperty = serializedObject.FindProperty("fadeSpeed");
        
        EditorGUILayout.PropertyField(tabBarButtonProperty);
        EditorGUILayout.PropertyField(onShowEventProperty);
        EditorGUILayout.PropertyField(onHideEventProperty);
        EditorGUILayout.PropertyField(fadeSpeedProperty);
        
        serializedObject.ApplyModifiedProperties();
    }
}