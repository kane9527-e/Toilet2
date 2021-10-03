// ReSharper disable once CheckNamespace

using Narrative.Runtime.Scripts.ConditionConfig;
using Narrative.Runtime.Scripts.Nodes.TriggerNode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Narrative.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(DataCondition))]
    public class DataConditionPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //Find the number of fields and the space it will take (including the number of items in the list)
            return 80;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            //创建一个属性包装器，用于将常规GUI控件与SerializedProperty一起使用
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                //设置属性名宽度
                EditorGUIUtility.labelWidth = 100;
                //输入框高度，默认一行的高度
                position.height = EditorGUIUtility.singleLineHeight;
                var keyNameRect = new Rect(position)
                {
                    width = position.width,
                    x = position.x
                };
                var conditionTypeRect = new Rect(position)
                {
                    width = position.width,
                    x = position.x,
                    y = keyNameRect.y + EditorGUIUtility.singleLineHeight + 2,
                };
                var dataValueTypeRect = new Rect(position)
                {
                    width = position.width,
                    x = position.x,
                    y = conditionTypeRect.y + EditorGUIUtility.singleLineHeight + 2,
                };
                var relationalRect = new Rect(position)
                {
                    width = position.width / 2,
                    x = position.x,
                    y = dataValueTypeRect.y + EditorGUIUtility.singleLineHeight + 2,
                };
                var valueRect = new Rect(position)
                {
                    width = position.width / 2 - 10,
                    x = relationalRect.width + 56,
                    y = relationalRect.y,
                };
                var keyNameProperty = property.FindPropertyRelative("keyName");
                EditorGUI.PropertyField(keyNameRect, keyNameProperty);
                var conditionTypeProperty = property.FindPropertyRelative("conditionType");
                EditorGUI.PropertyField(conditionTypeRect, conditionTypeProperty);

                var dataValueTypeProperty = property.FindPropertyRelative("dataValueType");
                EditorGUI.PropertyField(dataValueTypeRect, dataValueTypeProperty);

                var relationalProperty = property.FindPropertyRelative("relationalType");
                EditorGUI.PropertyField(relationalRect, relationalProperty, new GUIContent("Relational"));
                var propertyName = string.Empty;
                switch ((DataValueType)dataValueTypeProperty.enumValueIndex)
                {
                    case DataValueType.Int:
                        propertyName = "intValue";
                        break;
                    case DataValueType.Float:
                        propertyName = "floatValue";
                        break;
                    case DataValueType.Bool:
                        propertyName = "boolValue";
                        break;
                    case DataValueType.String:
                        propertyName = "stringValue";
                        break;
                }

                if (!string.IsNullOrWhiteSpace(propertyName))
                {
                    var valueProperty = property.FindPropertyRelative(propertyName);
                    EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}