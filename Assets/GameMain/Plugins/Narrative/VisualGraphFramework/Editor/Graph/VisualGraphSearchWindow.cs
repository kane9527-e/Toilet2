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
    public class VisualGraphSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private VisualGraphView graphView;
        private Texture2D indentationIcon;
        private readonly List<Type> nodeTypes = new List<Type>();
        private EditorWindow window;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
            //tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 1));

            foreach (var type in nodeTypes)
            {
                var display_name = "";
                if (type.GetCustomAttribute<NodeNameAttribute>() != null)
                    display_name = type.GetCustomAttribute<NodeNameAttribute>().name;
                else
                    display_name = type.Name;

                var splitDisplayName = display_name.Split('/');
                var lastTree = tree;
                for (var i = 0; i < splitDisplayName.Length - 1; i++)
                {
                    var displayName = splitDisplayName[i];
                    var findTree = lastTree.Find(item => item.name == displayName);
                    ;
                    if (findTree == null)
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(displayName, indentationIcon))
                        {
                            level = i + 1,
                            userData = displayName
                        });
                }

                tree.Add(new SearchTreeEntry(new GUIContent(splitDisplayName[splitDisplayName.Length - 1],
                    indentationIcon))
                {
                    level = splitDisplayName.Length,
                    userData = type
                });
            }

            tree.Add(new SearchTreeEntry(new GUIContent("Group", indentationIcon))
            {
                level = 1,
                userData = new Group()
            });

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
                context.screenMousePosition - window.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            switch (SearchTreeEntry.userData)
            {
                case Type nodeData:
                {
                    graphView.CreateNode(graphMousePosition, nodeData);
                    return true;
                }
                case Group group:
                    graphView.CreateGroupBlock(graphMousePosition);
                    return true;
            }

            return false;
        }

        public void Configure(EditorWindow window, VisualGraphView graphView)
        {
            this.window = window;
            this.graphView = graphView;

            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var typeAttrib = graphView.VisualGraph.GetType().GetCustomAttribute<DefaultNodeTypeAttribute>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                    if (typeAttrib != null && (type.IsAssignableFrom(typeAttrib.type) ||
                                               type.IsSubclassOf(typeAttrib.type))
                                           && type.IsSubclassOf(typeof(VisualGraphNode))
                                           && type.IsAbstract == false)
                        nodeTypes.Add(type);
            }

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();
        }
    }
}