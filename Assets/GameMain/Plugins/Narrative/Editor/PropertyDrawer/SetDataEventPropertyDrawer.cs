using Narrative.Runtime.Scripts.Nodes.TriggerNode;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable once CheckNamespace
namespace Narrative.Editor.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(SetDataEvent), true)]
    public class SetDataEventPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element.
            var container = new VisualElement();

            var actionTypeField = new PropertyField(property.FindPropertyRelative("actionType"));

            var valueTypeProperty = property.FindPropertyRelative("valueType");
            var valueTypeField = new PropertyField(valueTypeProperty);
            container.Add(actionTypeField);
            container.Add(valueTypeField);


            var propertyName = string.Empty;
            switch ((DataValueType)valueTypeProperty.enumValueIndex)
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

            var valueField = new PropertyField(property.FindPropertyRelative(propertyName));
            container.Add(valueField);
            return container;
        }

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
                var actionTypeRect = new Rect(position)
                {
                    width = position.width,
                    x = position.x,
                    y = keyNameRect.y + EditorGUIUtility.singleLineHeight + 2
                };
                var valueTypeRect = new Rect(position)
                {
                    width = position.width,
                    x = position.x,
                    y = actionTypeRect.y + EditorGUIUtility.singleLineHeight + 2
                };
                var valueRect = new Rect(position)
                {
                    width = position.width,
                    x = position.x,
                    y = valueTypeRect.y + EditorGUIUtility.singleLineHeight + 2
                };

                var keyNameProperty = property.FindPropertyRelative("keyName");
                EditorGUI.PropertyField(keyNameRect, keyNameProperty);
                var actionTypeProperty = property.FindPropertyRelative("actionType");
                EditorGUI.PropertyField(actionTypeRect, actionTypeProperty);
                var valueTypeProperty = property.FindPropertyRelative("valueType");
                EditorGUI.PropertyField(valueTypeRect, valueTypeProperty);

                var propertyName = string.Empty;
                switch ((DataValueType)valueTypeProperty.enumValueIndex)
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

                var valueProperty = property.FindPropertyRelative(propertyName);
                EditorGUI.PropertyField(valueRect, valueProperty);
            }

            EditorGUI.EndProperty();
        }
    }
}