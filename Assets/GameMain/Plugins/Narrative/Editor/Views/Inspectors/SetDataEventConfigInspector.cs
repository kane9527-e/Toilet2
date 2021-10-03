#if UNITY_EDITOR
using Narrative.Runtime.Scripts.Nodes.TriggerNode;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.Views.Inspectors
{
    [CustomEditor(typeof(SetDataEventConfig))]
    public class SetDataEventConfigInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var setDataEventsProperty = serializedObject.FindProperty("setDataEvents");
            if (setDataEventsProperty != null)
                EditorGUILayout.PropertyField(setDataEventsProperty);
        }
    }
}
#endif