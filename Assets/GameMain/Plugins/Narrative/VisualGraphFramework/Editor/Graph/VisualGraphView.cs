﻿///-------------------------------------------------------------------------------------------------
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
using Object = UnityEngine.Object;

namespace VisualGraphEditor
{
    public class VisualGraphView : GraphView, IEdgeConnectorListener
    {
        public bool activeVisualGraph;
        private VisualGraphEditor editorWindow;
        private Orientation orientation;
        private VisualGraphSearchWindow searchWindow;

        private VisualGraph visualGraph;

        // Runtime Type / Editor Type
        private readonly Dictionary<Type, Type> visualGraphNodeLookup = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Type> visualGraphPortLookup = new Dictionary<Type, Type>();

        public VisualGraphView(VisualGraphEditor editorWindow)
        {
            Initialize(editorWindow);
        }

        public VisualGraph VisualGraph
        {
            get => visualGraph;
            private set { }
        }

        public MiniMap Minimap { get; private set; }
        public BlackboardView BlackboardView { get; private set; }

        public Blackboard Blackboard
        {
            get => BlackboardView.blackboard;
            private set { }
        }

        // ReSharper disable once ParameterHidesMember
        public void Initialize(VisualGraphEditor editorWindow)
        {
            this.editorWindow = editorWindow;

            Undo.undoRedoPerformed += OnUndoRedoCallback;

            graphViewChanged = OnGraphChange;

            styleSheets.Add(Resources.Load<StyleSheet>("VisualGraphStyle"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var nodeAttrib = type.GetCustomAttribute<CustomNodeViewAttribute>();
                    if (nodeAttrib != null && nodeAttrib.type.IsAbstract == false)
                        visualGraphNodeLookup.Add(nodeAttrib.type, type);

                    var portAttrib = type.GetCustomAttribute<CustomPortViewAttribute>();
                    if (portAttrib != null && portAttrib.type.IsAbstract == false)
                        visualGraphPortLookup.Add(portAttrib.type, type);
                }
            }
        }

        public void CreateMinimap(float windowWidth)
        {
            Minimap = new MiniMap { anchored = true };
            Minimap.capabilities &= ~Capabilities.Movable;
            Minimap.SetPosition(new Rect(windowWidth - 210, 30, 200, 140));
            Add(Minimap);
        }

        public void CreateBlackboard()
        {
            BlackboardView = new BlackboardView();
            BlackboardView.visualGraphView = this;
            Blackboard.SetPosition(new Rect(10, 30, 250, 300));
            Add(BlackboardView.blackboard);
        }

        #region View OnGUI/Update

        public void OnGUI()
        {
            if (Minimap != null) Minimap.SetPosition(new Rect(contentRect.width - 210, 30, 200, 140));
            if (Blackboard != null)
                Blackboard.SetPosition(new Rect(10, 30, Blackboard.style.width.value.value,
                    Blackboard.style.height.value.value));
        }

        public void Update()
        {
            nodes.ForEach(nodeView =>
            {
                var node = nodeView.userData as VisualGraphNode;
                if (node != null)
                {
                    if (node.editor_ActiveNode)
                        nodeView.AddToClassList("VisualGraphNodeSelected");
                    else
                        nodeView.RemoveFromClassList("VisualGraphNodeSelected");
                }
            });
        }

        #endregion

        #region Init the Graph

        public void OnUndoRedoCallback()
        {
            SetGraph(visualGraph);
        }

        /// <summary>
        ///     Load the Visual Graph into the Editor Graph View
        /// </summary>
        /// <param name="_graph"></param>
        public void SetGraph(VisualGraph _visualGraph)
        {
            // Set the graph to null and clear the edges and nodes before we get going.
            visualGraph = null;
            DeleteElements(graphElements.ToList());
            DeleteElements(nodes.ToList());
            DeleteElements(edges.ToList());
            activeVisualGraph = false;
            BlackboardView.ClearBlackboard();

            visualGraph = _visualGraph;
            if (visualGraph != null)
            {
                // When the graph is loaded connections need to be remade
                visualGraph.InitializeGraph();

                activeVisualGraph = true;

                var orientationAttrib =
                    visualGraph.GetType().GetCustomAttribute<GraphOrientationAttribute>();
                Debug.Assert(orientationAttrib != null,
                    $"Graph node requires a GraphOrientationAttribute {visualGraph.GetType().Name}");
                orientation = orientationAttrib.GrapOrientation == GraphOrientationAttribute.Orientation.Horizontal
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

                BlackboardView.SetVisualGraph(visualGraph);

                searchWindow = ScriptableObject.CreateInstance<VisualGraphSearchWindow>();
                searchWindow.Configure(editorWindow, this);
                nodeCreationRequest = context =>
                    SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);

                // If the graph doesn't have a start node it's probably the first time we opened it. This means
                // we will create one to get going.
                if (visualGraph.StartingNode == null)
                {
                    var startingNode =
                        Activator.CreateInstance(typeof(VisualGraphStartNode)) as VisualGraphNode;
                    visualGraph.StartingNode = startingNode;
                    startingNode.name = "Start";
                    startingNode.position = new Vector2(270, 30);

                    var graphPort = startingNode.AddPort("Next", VisualGraphPort.PortDirection.Output);
                    graphPort.CanBeRemoved = false;
                    visualGraph.Nodes.Add(startingNode);

                    if (startingNode.name == null || startingNode.name.Trim() == "")
                        startingNode.name = ObjectNames.NicifyVariableName(startingNode.name);
                    if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(visualGraph)))
                        AssetDatabase.AddObjectToAsset(startingNode, visualGraph);

                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(startingNode));
                }

                // Create all other Nodes for the graph
                foreach (var graphNode in visualGraph.Nodes)
                {
                    var node = AddGraphNode(graphNode);
                    var pos = new Vector2(node.style.left.value.value, node.style.top.value.value);
                    ((VisualGraphNode)node.userData).position = pos;
                }

                foreach (var graphNode in visualGraph.Nodes)
                foreach (var graphPort in graphNode.Ports)
                    if (graphPort.Direction == VisualGraphPort.PortDirection.Output)
                    {
                        var port = graphPort.editor_port as Port;
                        foreach (var graph_connection in
                            graphPort.Connections)
                        {
                            var other_port =
                                graph_connection.Node.FindPortByGuid(graph_connection.port_guid);
                            var other_editor_port = other_port.editor_port as Port;
                            AddElement(port.ConnectTo(other_editor_port));
                        }
                    }

                //foreach (var group in visualGraph.Groups)
                //{
                //	AddGroupBlock(group);
                //}
            }
        }

        #endregion

        #region Group Block

        public void CreateGroupBlock(Vector2 position)
        {
            Undo.RecordObject(visualGraph, "Create Node");
            var block = new VisualGraphGroup
            {
                title = "Graph Group",
                position = new Rect(position.x, position.y, 300, 200)
            };
            visualGraph.Groups.Add(block);
            AddGroupBlock(block);
        }

        public void AddGroupBlock(VisualGraphGroup graphGroup)
        {
            var group = new VisualGraphGroupView
            {
                autoUpdateGeometry = true,
                title = graphGroup.title,
                userData = graphGroup
            };
            group.SetPosition(graphGroup.position);
            AddElement(group);

            var nodes = new HashSet<GraphElement>();
            foreach (var node_guid in graphGroup.node_guids)
            {
                var node = visualGraph.FindNodeByGuid(node_guid);
                if (node != null) nodes.Add(node.graphElement as GraphElement);
            }

            group.CollectElements(nodes, null);
        }

        #endregion

        #region Node Creation

        /// <summary>
        ///     Create a node based off the type. Once the node is created it will be added to the Graph and a View
        ///     node will be created and added
        /// </summary>
        /// <param name="position"></param>
        /// <param name="nodeType"></param>
        public void CreateNode(Vector2 position, Type nodeType)
        {
            Undo.RecordObject(visualGraph, "Create Node");
            var graphNode = visualGraph.AddNode(nodeType);
            Undo.RegisterCreatedObjectUndo(graphNode, "Create Node");
            graphNode.position = position;

            if (graphNode.name == null || graphNode.name.Trim() == "")
                graphNode.name = ObjectNames.NicifyVariableName(nodeType.Name);
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(visualGraph)))
                AssetDatabase.AddObjectToAsset(graphNode, visualGraph);

            var node = AddGraphNode(graphNode);

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graphNode));
        }

        /// <summary>
        ///     Add the node from the graph to the view based off property and attribute settings
        /// </summary>
        /// <param name="graphNode"></param>
        /// <returns></returns>
        private Node AddGraphNode(VisualGraphNode graphNode)
        {
            // By default we will create all nodes from VisualGraphNodeView
            var visualNodeType = typeof(VisualGraphNodeView);
            if (visualGraphNodeLookup.ContainsKey(graphNode.GetType()))
                visualNodeType = visualGraphNodeLookup[graphNode.GetType()];

            // Create the Node View based off the type, set the class for styling
            var nodeView = Activator.CreateInstance(visualNodeType) as VisualGraphNodeView;
            nodeView.AddToClassList("VisualGraphNode");
            nodeView.title = GetGraphNodeName(graphNode.GetType());
            nodeView.userData = graphNode;
            nodeView.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            nodeView.Initialize(graphNode);
            // The Editor stores a reference to the graph created;
            graphNode.graphElement = nodeView;

            // If there are extra Styles apply them
            var customStyleAttribs =
                graphNode.GetType().GetCustomAttributes<CustomNodeStyleAttribute>();
            if (customStyleAttribs != null)
                foreach (var customStyleAttrib in customStyleAttribs)
                    try
                    {
                        var styleSheet = Resources.Load<StyleSheet>(customStyleAttrib.style);
                        if (styleSheet != null)
                            nodeView.styleSheets.Add(styleSheet);
                        else throw new Exception();
                    }
#pragma warning disable 168
                    catch (Exception ex)
#pragma warning restore 168
                    {
                        Debug.LogWarning($"Style sheet does not exit: {customStyleAttrib.style}");
                    }

            // Get the Port Dynamics. Base class type already has the attribute so this should never fail
            var dynamicsAttrib =
                graphNode.GetType().GetCustomAttribute<NodePortAggregateAttribute>();
            Debug.Assert(dynamicsAttrib != null,
                $"Graph node requires a NodePortAggregateAttribute {graphNode.GetType().Name}");

            if (dynamicsAttrib.InputPortAggregate == NodePortAggregateAttribute.PortAggregate.Multiple)
            {
                // Button for input ports
                var button = new Button(() => { CreatePort(nodeView, "Input", VisualGraphPort.PortDirection.Input); })
                {
                    text = "Add Input"
                };
                nodeView.titleButtonContainer.Add(button);
            }

            if (dynamicsAttrib.OutputPortAggregate == NodePortAggregateAttribute.PortAggregate.Multiple)
            {
                var button = new Button(() => { CreatePort(nodeView, "Exit", VisualGraphPort.PortDirection.Output); })
                {
                    text = "Add Exit"
                };
                nodeView.titleButtonContainer.Add(button);
            }

            // Set the node capabilites. The default View node can be overriden
            nodeView.capabilities = nodeView.SetCapabilities(nodeView.capabilities);
            nodeView.SetPosition(new Rect(graphNode.position, nodeView.default_size));

            // Add the needed ports
            foreach (var graphPort in graphNode.Ports) AddPort(graphPort, nodeView);

            // If there are custom elements or drawing, let the derived node handle this
            nodeView.DrawNode();

            // If the node wants to hide the properties the user must make a View node and set this to false
            if (nodeView.ShowNodeProperties)
            {
                var divider = new VisualElement();
                divider.style.borderBottomColor = divider.style.borderTopColor =
                    divider.style.borderLeftColor = divider.style.borderRightColor = Color.black;
                divider.style.borderBottomWidth = divider.style.borderTopWidth =
                    divider.style.borderLeftWidth = divider.style.borderRightWidth = 0.5f;
                nodeView.mainContainer.Add(divider);

                var node_data = new VisualElement();
                node_data.AddToClassList("node_data");
                nodeView.mainContainer.Add(node_data);

                var editor = Editor.CreateEditor((VisualGraphNode)nodeView.userData);
                var inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
                node_data.Add(inspectorIMGUI);
            }

            // Finally add the element
            AddElement(nodeView);

            // Refresh the view
            nodeView.RefreshExpandedState();
            nodeView.RefreshPorts();

            return nodeView;
        }

        private string GetGraphNodeName(Type type)
        {
            var display_name = "";
            if (type.GetCustomAttribute<NodeNameAttribute>() != null)
                display_name = type.GetCustomAttribute<NodeNameAttribute>().name;
            else
                display_name = type.Name;

            return display_name;
        }

        #endregion

        #region Port Connections

        /// <summary>
        ///     Create a port for the given node based off the direction. Once the port is created for the node in the graph
        ///     a port will be added to the view node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        public void CreatePort(Node node, string name, VisualGraphPort.PortDirection direction)
        {
            var graphNode = node.userData as VisualGraphNode;
            Undo.RecordObject(graphNode, "Add Port to Node");

            var graphPort = graphNode.AddPort(name, direction);
            AddPort(graphPort, node);

            EditorUtility.SetDirty(visualGraph);
        }

        /// <summary>
        ///     Add a port to the view node
        /// </summary>
        /// <param name="graphPort"></param>
        /// <param name="node"></param>
        private void AddPort(VisualGraphPort graphPort, Node node)
        {
            var graphNode = node.userData as VisualGraphNode;

            // Determine the direction of the port (In/Out)
            var direction = graphPort.Direction == VisualGraphPort.PortDirection.Input
                ? Direction.Input
                : Direction.Output;

            // Get the capacity of the port (how many connections can this port have)
            var capacityAttrib = graphNode.GetType().GetCustomAttribute<PortCapacityAttribute>();
            Debug.Assert(capacityAttrib != null,
                $"Graph node requires a PortCapacityAttribute {graphNode.GetType().Name}");
            var capacity = Port.Capacity.Single;
            if (graphPort.Direction == VisualGraphPort.PortDirection.Input)
                capacity = capacityAttrib.InputPortCapacity == PortCapacityAttribute.Capacity.Single
                    ? Port.Capacity.Single
                    : Port.Capacity.Multi;
            else
                capacity = capacityAttrib.OutputPortCapacity == PortCapacityAttribute.Capacity.Single
                    ? Port.Capacity.Single
                    : Port.Capacity.Multi;

            // Get the data type for the port.
            // TODO: can we optimze/change this to be more dynamic?
            var port_type = graphPort.Direction == VisualGraphPort.PortDirection.Input
                ? graphNode.InputType
                : graphNode.OutputType;

            // Create the port based off supplied information
            var port = node.InstantiatePort(orientation, direction, capacity, port_type);
            port.portName = ""; // Don't set the name this helps with the view.
            port.userData = graphPort;
            graphPort.editor_port = port;


            // Custom View for ports
            var portAggregateAttrib =
                graphNode.GetType().GetCustomAttribute<NodePortAggregateAttribute>();
            var aggregate = NodePortAggregateAttribute.PortAggregate.None;
            if (graphPort.Direction == VisualGraphPort.PortDirection.Input)
                aggregate = portAggregateAttrib.InputPortAggregate == NodePortAggregateAttribute.PortAggregate.Single
                    ? NodePortAggregateAttribute.PortAggregate.Single
                    : NodePortAggregateAttribute.PortAggregate.Multiple;
            else
                aggregate = portAggregateAttrib.OutputPortAggregate == NodePortAggregateAttribute.PortAggregate.Single
                    ? NodePortAggregateAttribute.PortAggregate.Single
                    : NodePortAggregateAttribute.PortAggregate.Multiple;

            VisualGraphPortView graphPortView = null;
            if (aggregate == NodePortAggregateAttribute.PortAggregate.Single)
            {
                graphPortView = Activator.CreateInstance(typeof(VisualGraphLabelPortView)) as VisualGraphLabelPortView;
            }
            else
            {
                Type portViewType = null;
                visualGraphPortLookup.TryGetValue(graphPort.GetType(), out portViewType);
                if (portViewType == null) portViewType = typeof(VisualGraphPortView);

                graphPortView = Activator.CreateInstance(portViewType) as VisualGraphPortView;
            }

            graphPortView.CreateView(graphPort);
            port.Add(graphPortView);

            // If the user can remove a port add a button
            if (graphPort.CanBeRemoved)
            {
                var deleteButton = new Button(() => RemovePort(node, port))
                {
                    text = "X"
                };
                port.Add(deleteButton);
            }

            port.AddManipulator(new EdgeConnector<Edge>(this));

            var portTypeAttribute = graphNode.GetType().GetCustomAttribute<PortTypeAttribute>();

            // Put the port in the proper container for the view
            if (direction == Direction.Input)
            {
                if (portTypeAttribute != null && portTypeAttribute.InputType != null)
                    port.portType = portTypeAttribute.InputType;
                node.inputContainer.Add(port);
            }
            else
            {
                if (portTypeAttribute != null && portTypeAttribute.OutputType != null)
                    port.portType = portTypeAttribute.OutputType;
                node.outputContainer.Add(port);
            }

            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        /// <summary>
        ///     Connect the nodes together through the port Connections
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="edge"></param>
        public void OnDrop(GraphView graphView, Edge edge)
        {
            var graph_input_node = edge.input.node.userData as VisualGraphNode;
            var graph_output_node = edge.output.node.userData as VisualGraphNode;

            Undo.RecordObjects(new Object[] { graph_input_node, graph_output_node }, "Add Port to Node");

            var graph_input_port = edge.input.userData as VisualGraphPort;
            var graph_output_port = edge.output.userData as VisualGraphPort;

            graph_input_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection
            {
                initialized = true,
                Node = edge.output.node.userData as VisualGraphNode,
                port = graph_output_port,
                port_guid = graph_output_port.guid,
                node_guid = graph_output_node.guid
            });
            graph_output_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection
            {
                initialized = true,
                Node = edge.input.node.userData as VisualGraphNode,
                port = graph_input_port,
                port_guid = graph_input_port.guid,
                node_guid = graph_input_node.guid
            });

            EditorUtility.SetDirty(visualGraph);
        }

        /// <summary>
        ///     Not needed for default graphing. May be implemented in future versions
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="position"></param>
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        /// <summary>
        ///     Remove the given port from the node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="socket"></param>
        private void RemovePort(Node node, Port socket)
        {
            var socket_port = socket.userData as VisualGraphPort;
            var edgeList = edges.ToList();
            foreach (var edge in edgeList)
            {
                var graphPort = edge.output.userData as VisualGraphPort;
                if (graphPort.guid.Equals(socket_port.guid))
                {
                    RemoveEdge(edge);
                    edge.input.Disconnect(edge);
                    RemoveElement(edge);
                }
            }

            var graphNode = node.userData as VisualGraphNode;

            Undo.RecordObject(graphNode, "Remove Port");

            graphNode.Ports.Remove(socket_port);

            if (socket.direction == Direction.Input)
                node.inputContainer.Remove(socket);
            else
                node.outputContainer.Remove(socket);

            node.RefreshPorts();
            node.RefreshExpandedState();

            EditorUtility.SetDirty(visualGraph);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach(port =>
            {
                var portView = port;

                var startNode = (VisualGraphNode)startPortView.node.userData;
                var portNode = (VisualGraphNode)portView.node.userData;
                var hasConnectSamNodeAttribute =
                    startNode.GetType().GetCustomAttribute<CanNotConnectSameNodeAttribute>() != null;

                var connectCheckPass =
                    hasConnectSamNodeAttribute && portNode.GetType() != startNode.GetType() ||
                    !hasConnectSamNodeAttribute;

                if (startPortView.direction != portView.direction && startPortView != portView &&
                    startPortView.node != portView.node && startPortView.portType == portView.portType &&
                    connectCheckPass)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #endregion

        #region Graph Changes

        private GraphViewChange OnGraphChange(GraphViewChange change)
        {
            // Return from this if we don't have a Visual Graph. This means we are reseting the
            // graph
            if (visualGraph == null) return change;

            if (change.elementsToRemove != null)
                foreach (var element in change.elementsToRemove)
                    if (typeof(Edge).IsInstanceOfType(element))
                    {
                        var graph_input_node = ((Edge)element).input.node.userData as VisualGraphNode;
                        var graph_output_node = ((Edge)element).output.node.userData as VisualGraphNode;
                        Undo.RecordObjects(new Object[] { graph_input_node, graph_output_node },
                            "Add Port to Node");

                        RemoveEdge((Edge)element);
                    }
                    else if (typeof(Node).IsInstanceOfType(element))
                    {
                        RemoveNode((Node)element);
                    }
                //else if (typeof(Group).IsInstanceOfType(element) == true)
                //{
                //    VisualGraphGroup block = ((Group)element).userData as VisualGraphGroup;
                //    Undo.RecordObjects(new UnityEngine.Object[] { visualGraph }, "Add Port to Node");
                //    visualGraph.Groups.Remove(block);
                //}

            if (change.movedElements != null)
            {
                var movedNodes = new List<VisualGraphNode>();
                foreach (var element in change.movedElements)
                    if (typeof(Node).IsInstanceOfType(element))
                        movedNodes.Add((VisualGraphNode)element.userData);

                Undo.RecordObjects(movedNodes.ToArray(), "Moved VisualGraphNode");

                foreach (var element in change.movedElements)
                    if (typeof(Node).IsInstanceOfType(element))
                    {
                        movedNodes.Add((VisualGraphNode)element.userData);
                        ((VisualGraphNode)element.userData).position = new Vector2(element.style.left.value.value,
                            element.style.top.value.value);
                    }
            }

            EditorUtility.SetDirty(visualGraph);

            return change;
        }

        private void RemoveNode(Node node)
        {
            var graphNode = node.userData as VisualGraphNode;
            Undo.RecordObjects(new Object[] { graphNode, visualGraph }, "Delete Node");
            visualGraph.RemoveNode(graphNode);
            Undo.DestroyObjectImmediate(graphNode);
        }

        private void RemoveEdge(Edge edge)
        {
            var graph_input_port = edge.input.userData as VisualGraphPort;
            var graph_output_port = edge.output.userData as VisualGraphPort;
            graph_input_port.RemoveConnectionByPortGuid(graph_output_port.guid);
            graph_output_port.RemoveConnectionByPortGuid(graph_input_port.guid);
        }

        #endregion
    }
}