// ReSharper disable once CheckNamespace

using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ConditionSetting.Editor
{
    [CustomEditor(typeof(ConditionConfig))]
    public class ConditionConfigInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Test Result"))
            {
                var resultMsg =
                    new GUIContent(string.Format("Condition Result:{0}", ((ConditionConfig)target).Result()));
                EditorWindow.mouseOverWindow.ShowNotification(resultMsg);
            }
        }
    }
}