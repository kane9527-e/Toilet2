///-------------------------------------------------------------------------------------------------
// author: William Barry
// date: 2020
// Copyright (c) Bus Stop Studios.
///-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphRuntime;

namespace VisualGraphEditor
{
    public class BlackboardView
    {
        private readonly Dictionary<Type, Type> blackboardFieldTypes = new Dictionary<Type, Type>();
        private VisualGraph visualGraph;
        public VisualGraphView visualGraphView;

        public BlackboardView()
        {
            blackboard = new Blackboard();
            blackboard.scrollable = true;
            blackboard.windowed = false;
            blackboard.Add(new BlackboardSection { title = "Graph Properties" });
            blackboard.addItemRequested = AddItemRequested;
            blackboard.editTextRequested = EditTextRequested;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (type.IsSubclassOf(typeof(BlackboardFieldView)) && type.IsAbstract == false)
                    {
                        var attrib = type.GetCustomAttribute<BlackboardPropertyTypeAttribute>();
                        if (attrib != null) blackboardFieldTypes.Add(attrib.type, type);
                    }
            }
        }

        public Blackboard blackboard { get; }

        public void ClearBlackboard()
        {
            blackboard.Clear();
        }

        public void SetVisualGraph(VisualGraph visualGraph)
        {
            blackboard.Clear();
            this.visualGraph = visualGraph;
            foreach (var property in visualGraph.BlackboardProperties)
                if (blackboardFieldTypes.ContainsKey(property.GetType()))
                    AddBlackboardProperty(blackboardFieldTypes[property.GetType()], property);
        }

        public void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            var field = (BlackboardField)visualElement;
            var property = (AbstractBlackboardProperty)field.userData;

            if (!string.IsNullOrEmpty(newText) && newText != property.Name)
            {
                Undo.RecordObject(visualGraph, "Edit Blackboard Text");

                var count = 0;
                var propertyName = newText;
                foreach (var boardProperty in visualGraph.BlackboardProperties)
                    if (boardProperty.Name == propertyName)
                        count++;
                if (count > 0) propertyName += $"({count})";

                property.Name = propertyName;
                field.text = property.Name;

                EditorUtility.SetDirty(visualGraph);
            }
        }

        private void AddItemRequested(Blackboard blackboard)
        {
            if (visualGraph == null) return;

            var menu = new GenericMenu();
            foreach (var type in blackboardFieldTypes)
            {
                var attrib = type.Value.GetCustomAttribute<BlackboardPropertyTypeAttribute>();
                if (attrib != null)
                    menu.AddItem(new GUIContent(attrib.menuName), false, () => CreateBlackboardProperty(type.Value));
            }

            menu.ShowAsContext();
        }

        private void CreateBlackboardProperty(Type type)
        {
            var attrib = type.GetCustomAttribute<BlackboardPropertyTypeAttribute>();

            Undo.RecordObject(visualGraph, "Add Blackboard Property");

            var count = 0;
            var propertyName = attrib.menuName;
            foreach (var boardProperty in visualGraph.BlackboardProperties)
                if (boardProperty.Name == propertyName)
                    count++;
            if (count > 0) propertyName += $"({count})";

            var propertyType = attrib.type;
            var property = Activator.CreateInstance(propertyType) as AbstractBlackboardProperty;
            property.name = attrib.type.Name;
            property.Name = propertyName;
            property.guid = Guid.NewGuid().ToString();
            visualGraph.BlackboardProperties.Add(property);

            if (property.name == null || property.name.Trim() == "")
                property.name = ObjectNames.NicifyVariableName(property.name);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(visualGraph)))
                AssetDatabase.AddObjectToAsset(property, visualGraph);

            AddBlackboardProperty(type, property);

            EditorUtility.SetDirty(visualGraph);
            AssetDatabase.SaveAssets();
        }

        private void OnRemoveBlackboardProperty(BlackboardFieldView field)
        {
            Undo.RecordObject(visualGraph, "Remove Blackboard Property");

            visualGraph.BlackboardProperties.Remove(field.property);
            blackboard.Remove(field);

            AssetDatabase.RemoveObjectFromAsset(field.property);
            Undo.DestroyObjectImmediate(field.property);

            EditorUtility.SetDirty(visualGraph);
        }

        private void AddBlackboardProperty(Type type, AbstractBlackboardProperty property)
        {
            var propertyView = Activator.CreateInstance(type) as BlackboardFieldView;
            propertyView.blackboardView = this;
            propertyView.CreateView(visualGraph, property);
            propertyView.onRemoveBlackboardProperty += OnRemoveBlackboardProperty;
            blackboard.Add(propertyView);
        }
    }
}