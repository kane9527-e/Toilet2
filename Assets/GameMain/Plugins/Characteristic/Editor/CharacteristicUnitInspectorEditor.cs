using Characteristic.Runtime.Scripts.ScriptableObject;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
// ReSharper disable once CheckNamespace
namespace Characteristic.Editor
{
    [CustomEditor(typeof(CharacteristicUnit))]
    public class CharacteristicUnitInspectorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var typeProperty = serializedObject.FindProperty("type");
            if (typeProperty == null) return;
            EditorGUILayout.PropertyField(typeProperty, new GUIContent("CharacteristicType", "Type of Characteristic"));
            var configProperty = serializedObject.FindProperty("config");
            EditorGUILayout.PropertyField(configProperty,
                new GUIContent("CharacteristicUIConfig", "CharacteristicUIConfig"));
            switch ((CharacteristicType)typeProperty.enumValueIndex)
            {
                case CharacteristicType.Int:

                    var intMaxProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.intvalueMax));
                    intMaxProperty.intValue =
                        EditorGUILayout.IntField("Max Int Value", intMaxProperty.intValue);

                    var intProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.intvalue));
                    intProperty.intValue =
                        EditorGUILayout.IntField("Int Value:",
                            Mathf.Clamp(intProperty.intValue, 0, intMaxProperty.intValue));
                    var intdefalutProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.intDefaultValue));
                    intdefalutProperty.intValue =
                        EditorGUILayout.IntField("Default Int Value",
                            Mathf.Clamp(intdefalutProperty.intValue, 0, intMaxProperty.intValue));
                    break;
                case CharacteristicType.Float:
                    var floatMaxProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.floatvalueMax));
                    floatMaxProperty.floatValue =
                        EditorGUILayout.FloatField("Float Max Value", floatMaxProperty.floatValue);

                    var floatProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.floatvalue));
                    floatProperty.floatValue =
                        EditorGUILayout.FloatField("Float Value",
                            Mathf.Clamp(floatProperty.floatValue, 0, floatMaxProperty.floatValue));

                    var floatdefalutProperty =
                        serializedObject.FindProperty(nameof(CharacteristicUnit.floatDefaultValue));
                    floatdefalutProperty.floatValue =
                        EditorGUILayout.FloatField("Float Default Value",
                            Mathf.Clamp(floatdefalutProperty.floatValue, 0, floatMaxProperty.floatValue));
                    break;
                case CharacteristicType.String:
                    var stringProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.stringvalue));
                    stringProperty.stringValue =
                        EditorGUILayout.TextField("String Value", stringProperty.stringValue);
                    var stringdefalutProperty =
                        serializedObject.FindProperty(nameof(CharacteristicUnit.stringDefaultValue));
                    stringdefalutProperty.stringValue =
                        EditorGUILayout.TextArea("String Default Value", stringdefalutProperty.stringValue);
                    break;
                case CharacteristicType.Boolean:
                    var boolProperty = serializedObject.FindProperty(nameof(CharacteristicUnit.boolvalue));
                    boolProperty.boolValue =
                        EditorGUILayout.Toggle("Bool Value", boolProperty.boolValue);

                    var booldefalutProperty =
                        serializedObject.FindProperty(nameof(CharacteristicUnit.boolDefaultValue));
                    booldefalutProperty.boolValue =
                        EditorGUILayout.Toggle("Bool Defalut Value", booldefalutProperty.boolValue);
                    break;
            }

            if (GUILayout.Button("ResetValue"))
            {
                ((CharacteristicUnit)target).Reset();
                Repaint();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif