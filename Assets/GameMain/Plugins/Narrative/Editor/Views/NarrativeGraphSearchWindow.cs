using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BetterSearchWindow;
using Project.NodeSystem.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;
using VisualGraphRuntime;

public class NarrativeGraphSearchWindow : BetterSearchWindow.BetterSearchWindow<NarrativeGraphSearchWindow, Type>,
    ISearchWindowProvider
{
    private EditorWindow window;
    private NarrativeGraphView graphView;
    private List<Type> nodeTypes = new List<Type>();
    private Texture2D indentationIcon;
    public SearchWindowContext Context { get; private set; }

    public void Configure(EditorWindow window, NarrativeGraphView graphView)
    {
        this.window = window;
        this.graphView = graphView;

        var result = new List<System.Type>();
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

        DefaultNodeTypeAttribute typeAttrib =
            graphView.NarrativeGraph.GetType().GetCustomAttribute<DefaultNodeTypeAttribute>();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeAttrib != null && (type.IsAssignableFrom(typeAttrib.type) == true ||
                                           type.IsSubclassOf(typeAttrib.type))
                                       && type.IsSubclassOf(typeof(VisualGraphNode)) == true
                                       && type.IsAbstract == false)
                {
                    nodeTypes.Add(type);
                }
            }
        }

        nodeTypes.Sort((x, y) =>
        {
            return nodeTypes.FindAll(item=>item.BaseType== x.BaseType).Count.CompareTo(nodeTypes.FindAll(item=>item.BaseType==y.BaseType).Count);
        });

        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        indentationIcon.Apply();
    }

    public override List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Context = context;
        // You can just use BetterSearchTree<T> (where T is you payload type) to build your search tree
        var tree = new BetterSearchTree<Type>();


        for (int i = nodeTypes.Count - 1; i >= 0; i--)
        {
            var type = nodeTypes[i];
            string displayName = "";
            if (type.GetCustomAttribute<NodeNameAttribute>() != null)
            {
                displayName = type.GetCustomAttribute<NodeNameAttribute>().name;
            }
            else
            {
                displayName = type.Name;
            }

            if (string.IsNullOrWhiteSpace(displayName)) continue;
            tree.AddLeaf(displayName, type);
        }


        return tree.ToSearchTreeEntries("Create Nodes");
    }

    public override bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
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
                GraphBuilder.CreateGroupBlock(graphView, graphMousePosition);
                return true;
        }

        return false;
    }
}