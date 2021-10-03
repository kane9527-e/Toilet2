#if UNITY_EDITOR
using Narrative.Runtime.Scripts.ConditionConfig;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Inspectors
{
    [CustomEditor(typeof(DataConditionConfig))]
    public class DataConditionConfigInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var dataConditionsProperty = serializedObject.FindProperty("dataConditions");
            if (dataConditionsProperty != null)
                EditorGUILayout.PropertyField(dataConditionsProperty);
        }
    }
}
#endif