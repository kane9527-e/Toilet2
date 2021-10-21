using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(TargetConstraintAttribute))]
[CustomPropertyDrawer(typeof(SerializableCallbackBase), true)]
public class SerializableCallbackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Without this, you can't edit fields above the SerializedProperty
        property.serializedObject.ApplyModifiedProperties();

        // Indent label
        label.text = " " + label.text;

#if UNITY_2019_1_OR_NEWER
        GUI.Box(position, "");
#else
		GUI.Box(position, "", (GUIStyle)
			"flow overlay box");
#endif
        position.y += 4;
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        property.serializedObject.Update();
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        var pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var targetRect = new Rect(pos.x, pos.y, pos.width, EditorGUIUtility.singleLineHeight);

        // Get target
        var targetProp = property.FindPropertyRelative("_target");
        object target = targetProp.objectReferenceValue;
        if (attribute != null && attribute is TargetConstraintAttribute)
        {
            var targetType = (attribute as TargetConstraintAttribute).targetType;
            EditorGUI.ObjectField(targetRect, targetProp, targetType, GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);
        }

        if (target == null)
        {
            var helpBoxRect = new Rect(position.x + 8, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing,
                position.width - 16, EditorGUIUtility.singleLineHeight);
            var msg = "Call not set. Execution will be slower.";
            EditorGUI.HelpBox(helpBoxRect, msg, MessageType.Warning);
        }
        else if (target is MonoScript)
        {
            var helpBoxRect = new Rect(position.x + 8, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing,
                position.width - 16,
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight +
                EditorGUIUtility.standardVerticalSpacing);
            var msg = "Assign a GameObject, Component or a ScriptableObject, not a script.";
            EditorGUI.HelpBox(helpBoxRect, msg, MessageType.Warning);
        }
        else
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            // Get method name
            var methodProp = property.FindPropertyRelative("_methodName");
            var methodName = methodProp.stringValue;

            // Get args
            var argProps = property.FindPropertyRelative("_args");
            var argTypes = GetArgTypes(argProps);

            // Get dynamic
            var dynamicProp = property.FindPropertyRelative("_dynamic");
            var dynamic = dynamicProp.boolValue;

            // Get active method
            var activeMethod = GetMethod(target, methodName, argTypes);

            var methodlabel = new GUIContent("n/a");
            if (activeMethod != null) methodlabel = new GUIContent(PrettifyMethod(activeMethod));
            else if (!string.IsNullOrEmpty(methodName))
                methodlabel = new GUIContent("Missing (" + PrettifyMethod(methodName, argTypes) + ")");

            var methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing,
                position.width, EditorGUIUtility.singleLineHeight);

            // Method select button
            pos = EditorGUI.PrefixLabel(methodRect, GUIUtility.GetControlID(FocusType.Passive),
                new GUIContent(dynamic ? "Method (dynamic)" : "Method"));
            if (EditorGUI.DropdownButton(pos, methodlabel, FocusType.Keyboard)) MethodSelector(property);

            if (activeMethod != null && !dynamic)
            {
                // Args
                var activeParameters = activeMethod.GetParameters();
                var argRect = new Rect(position.x, methodRect.max.y + EditorGUIUtility.standardVerticalSpacing,
                    position.width, EditorGUIUtility.singleLineHeight);
                var types = new string[argProps.arraySize];
                for (var i = 0; i < types.Length; i++)
                {
                    var argProp = argProps.FindPropertyRelative("Array.data[" + i + "]");
                    var argLabel = new GUIContent(ObjectNames.NicifyVariableName(activeParameters[i].Name));

                    EditorGUI.BeginChangeCheck();
                    switch ((Arg.ArgType)argProp.FindPropertyRelative("argType").enumValueIndex)
                    {
                        case Arg.ArgType.Bool:
                            EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("boolValue"), argLabel);
                            break;
                        case Arg.ArgType.Int:
                            EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("intValue"), argLabel);
                            break;
                        case Arg.ArgType.Float:
                            EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("floatValue"), argLabel);
                            break;
                        case Arg.ArgType.String:
                            EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("stringValue"), argLabel);
                            break;
                        case Arg.ArgType.Object:
                            var typeProp = argProp.FindPropertyRelative("_typeName");
                            var objProp = argProp.FindPropertyRelative("objectValue");

                            if (typeProp != null)
                            {
                                var objType = Type.GetType(typeProp.stringValue, false);
                                EditorGUI.BeginChangeCheck();
                                var obj = objProp.objectReferenceValue;
                                obj = EditorGUI.ObjectField(argRect, argLabel, obj, objType, true);
                                if (EditorGUI.EndChangeCheck()) objProp.objectReferenceValue = obj;
                            }
                            else
                            {
                                EditorGUI.PropertyField(argRect, objProp, argLabel);
                            }

                            break;
                    }

                    if (EditorGUI.EndChangeCheck()) property.FindPropertyRelative("dirty").boolValue = true;
                    argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            EditorGUI.indentLevel = indent;
        }

        // Set indent back to what it was
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }

    private void MethodSelector(SerializedProperty property)
    {
        // Return type constraint
        Type returnType = null;
        // Arg type constraint
        var argTypes = new Type[0];

        // Get return type and argument constraints
        var dummy = GetDummyFunction(property);
        var genericTypes = dummy.GetType().BaseType.GetGenericArguments();
        // SerializableEventBase is always void return type
        if (dummy is SerializableEventBase)
        {
            returnType = typeof(void);
            if (genericTypes.Length > 0)
            {
                argTypes = new Type[genericTypes.Length];
                Array.Copy(genericTypes, argTypes, genericTypes.Length);
            }
        }
        else
        {
            if (genericTypes != null && genericTypes.Length > 0)
            {
                // The last generic argument is the return type
                returnType = genericTypes[genericTypes.Length - 1];
                if (genericTypes.Length > 1)
                {
                    argTypes = new Type[genericTypes.Length - 1];
                    Array.Copy(genericTypes, argTypes, genericTypes.Length - 1);
                }
            }
        }

        var targetProp = property.FindPropertyRelative("_target");

        var dynamicItems = new List<MenuItem>();
        var staticItems = new List<MenuItem>();

        var targets = new List<Object> { targetProp.objectReferenceValue };

        if (targets[0] is Component)
        {
            targets = (targets[0] as Component).gameObject.GetComponents<Component>().ToList<Object>();
            targets.Add((targetProp.objectReferenceValue as Component).gameObject);
        }
        else if (targets[0] is GameObject)
        {
            targets = (targets[0] as GameObject).GetComponents<Component>().ToList<Object>();
            targets.Add(targetProp.objectReferenceValue as GameObject);
        }

        for (var c = 0; c < targets.Count; c++)
        {
            var t = targets[c];
            var methods = targets[c].GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];

                // Skip methods with wrong return type
                if (returnType != null && method.ReturnType != returnType) continue;
                // Skip methods with null return type
                // if (method.ReturnType == typeof(void)) continue;
                // Skip generic methods
                if (method.IsGenericMethod) continue;

                var parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

                // Skip methods with more than 4 args
                if (parms.Length > 4) continue;
                // Skip methods with unsupported args
                if (parms.Any(x => !Arg.IsSupported(x))) continue;

                var methodPrettyName = PrettifyMethod(methods[i]);
                staticItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name,
                    methodPrettyName, () => SetMethod(property, t, method, false)));

                // Skip methods with wrong constrained args
                if (argTypes.Length == 0 || !argTypes.SequenceEqual(parms)) continue;

                dynamicItems.Add(new MenuItem(targets[c].GetType().Name + "/" + methods[i].DeclaringType.Name,
                    methods[i].Name, () => SetMethod(property, t, method, true)));
            }
        }

        // Construct and display context menu
        var menu = new GenericMenu();
        if (dynamicItems.Count > 0)
        {
            var paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
            foreach (var path in paths)
                menu.AddItem(new GUIContent(path + "/Dynamic " + PrettifyTypes(argTypes)), false, null);
            for (var i = 0; i < dynamicItems.Count; i++)
                menu.AddItem(dynamicItems[i].label, false, dynamicItems[i].action);
            foreach (var path in paths)
            {
                menu.AddItem(new GUIContent(path + "/  "), false, null);
                menu.AddItem(new GUIContent(path + "/Static parameters"), false, null);
            }
        }

        for (var i = 0; i < staticItems.Count; i++) menu.AddItem(staticItems[i].label, false, staticItems[i].action);
        if (menu.GetItemCount() == 0)
            menu.AddDisabledItem(new GUIContent("No methods with return type '" + GetTypeName(returnType) + "'"));
        menu.ShowAsContext();
    }

    private string PrettifyMethod(string methodName, Type[] parmTypes)
    {
        var parmnames = PrettifyTypes(parmTypes);
        return methodName + "(" + parmnames + ")";
    }

    private string PrettifyMethod(MethodInfo methodInfo)
    {
        if (methodInfo == null) throw new ArgumentNullException("methodInfo");
        var parms = methodInfo.GetParameters();
        var parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
        return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + parmnames + ")";
    }

    private string PrettifyTypes(Type[] types)
    {
        if (types == null) throw new ArgumentNullException("types");
        return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
    }

    private MethodInfo GetMethod(object target, string methodName, Type[] types)
    {
        var activeMethod = target.GetType().GetMethod(methodName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, CallingConventions.Any, types,
            null);
        return activeMethod;
    }

    private Type[] GetArgTypes(SerializedProperty argsProp)
    {
        var types = new Type[argsProp.arraySize];
        for (var i = 0; i < argsProp.arraySize; i++)
        {
            var argProp = argsProp.GetArrayElementAtIndex(i);
            var typeNameProp = argProp.FindPropertyRelative("_typeName");
            if (typeNameProp != null) types[i] = Type.GetType(typeNameProp.stringValue, false);
            if (types[i] == null)
                types[i] = Arg.RealType((Arg.ArgType)argProp.FindPropertyRelative("argType").enumValueIndex);
        }

        return types;
    }

    private void SetMethod(SerializedProperty property, Object target, MethodInfo methodInfo, bool dynamic)
    {
        var targetProp = property.FindPropertyRelative("_target");
        targetProp.objectReferenceValue = target;
        var methodProp = property.FindPropertyRelative("_methodName");
        methodProp.stringValue = methodInfo.Name;
        var dynamicProp = property.FindPropertyRelative("_dynamic");
        dynamicProp.boolValue = dynamic;
        var argProp = property.FindPropertyRelative("_args");
        var parameters = methodInfo.GetParameters();
        argProp.arraySize = parameters.Length;
        for (var i = 0; i < parameters.Length; i++)
        {
            argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex =
                (int)Arg.FromRealType(parameters[i].ParameterType);
            argProp.FindPropertyRelative("Array.data[" + i + "]._typeName").stringValue =
                parameters[i].ParameterType.AssemblyQualifiedName;
        }

        property.FindPropertyRelative("dirty").boolValue = true;
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }

    private static string GetTypeName(Type t)
    {
        if (t == typeof(int)) return "int";
        if (t == typeof(float)) return "float";
        if (t == typeof(string)) return "string";
        if (t == typeof(bool)) return "bool";
        if (t == typeof(void)) return "void";
        return t.Name;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
        var targetProp = property.FindPropertyRelative("_target");
        var argProps = property.FindPropertyRelative("_args");
        var dynamicProp = property.FindPropertyRelative("_dynamic");
        var height = lineheight + lineheight;
        if (targetProp.objectReferenceValue != null && targetProp.objectReferenceValue is MonoScript)
            height += lineheight;
        else if (targetProp.objectReferenceValue != null && !dynamicProp.boolValue)
            height += argProps.arraySize * lineheight;
        height += 8;
        return height;
    }

    private static SerializableCallbackBase GetDummyFunction(SerializedProperty prop)
    {
        var stringValue = prop.FindPropertyRelative("_typeName").stringValue;
        var type = Type.GetType(stringValue, false);
        SerializableCallbackBase result;
        if (type == null)
            return null;
        result = Activator.CreateInstance(type) as SerializableCallbackBase;
        return result;
    }

    private class MenuItem
    {
        public readonly GenericMenu.MenuFunction action;
        public readonly GUIContent label;
        public readonly string path;

        public MenuItem(string path, string name, GenericMenu.MenuFunction action)
        {
            this.action = action;
            label = new GUIContent(path + '/' + name);
            this.path = path;
        }
    }
}