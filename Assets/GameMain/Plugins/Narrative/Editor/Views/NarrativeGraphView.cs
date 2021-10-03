using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Narrative.Editor.Views.Nodes.BaseNodeViews;
using Narrative.Runtime.Scripts.Graph;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using Project.NodeSystem.Editor;
using VisualGraphRuntime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using VisualGraphEditor;
using Button = UnityEngine.UIElements.Button;

public class NarrativeGraphView : GraphView, IEdgeConnectorListener
{
    public NarrativeGraph NarrativeGraph => narrativeGraph;

    public MiniMap Minimap { get; private set; }
    // public BlackboardView BlackboardView { get; private set; }

    // public Blackboard Blackboard
    // {
    //     get { return BlackboardView.blackboard; }
    //     private set { }
    // }

    private NarrativeGraph narrativeGraph;
    private NarrativeGraphSearchWindow searchWindow;
    private NarrativeGraphEditor editorWindow;
    private Orientation orientation;

    // Runtime Type / Editor Type
    private Dictionary<Type, Type> visualGraphNodeLookup = new Dictionary<Type, Type>();
    private Dictionary<Type, Type> visualGraphPortLookup = new Dictionary<Type, Type>();

    public bool activeVisualGraph = false;
    private Vector2 mousePosition;


    public NarrativeGraphView(NarrativeGraphEditor editorWindow)
    {
        Initialize(editorWindow);
    }

    // ReSharper disable once ParameterHidesMember
    public void Initialize(NarrativeGraphEditor editorWindow)
    {
        this.editorWindow = editorWindow;

        Undo.undoRedoPerformed += OnUndoRedoCallback;

        graphViewChanged = OnGraphChange;

        styleSheets.Add(Resources.Load<StyleSheet>("VisualGraphStyle"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        AddManipulator();
        RegisterCallback<MouseMoveEvent>(ONMouseMoveEventHandler);

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        serializeGraphElements = SerializeGraphElementsCallback;
        canPasteSerializedData = CanPasteSerializedDataCallback;
        unserializeAndPaste += OnPaste;

        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                CustomNodeViewAttribute nodeAttrib = type.GetCustomAttribute<CustomNodeViewAttribute>();
                if (nodeAttrib != null && nodeAttrib.type.IsAbstract == false)
                {
                    visualGraphNodeLookup.Add(nodeAttrib.type, type);
                }

                CustomPortViewAttribute portAttrib = type.GetCustomAttribute<CustomPortViewAttribute>();
                if (portAttrib != null && portAttrib.type.IsAbstract == false)
                {
                    visualGraphPortLookup.Add(portAttrib.type, type);
                }
            }
        }
    }

    #region EventHandler

    private void ONMouseMoveEventHandler(MouseMoveEvent evt)
    {
        mousePosition = GetMousePosition(evt);
    }



    #endregion

    private void AddManipulator()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(new FreehandSelector());
        // this.AddManipulator(GraphBuilder.AddGroupContextualMenu(this));
        // this.AddManipulator(GraphBuilder.AddStickyNoteContextualMenu(this));
    }

    #region CopyAndPaste

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        mousePosition = GetMousePosition(evt);
        if (evt.target is UnityEditor.Experimental.GraphView.GraphView && this.nodeCreationRequest != null)
        {
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Create Group",
                action => this.CreateGroupBlock(mousePosition));
            evt.menu.AppendAction("Create StickyNote",
                action => this.CreateStickyNote(mousePosition));
        }

        //mousePosition = GetLocalMousePosition(mousePosition);
    }

    private void OnPaste(string operationname, string data)
    {
        //Debug.Log(data);
        CopyPasteHelper helper = (CopyPasteHelper)JsonUtility.FromJson(data, typeof(CopyPasteHelper));
        List<NarrativeNode> originNodes = new List<NarrativeNode>();
        List<NarrativeNode> copyNodes = new List<NarrativeNode>();

        foreach (var jsonElement in helper.nodeclipboard)
        {
            var originNode = JsonSerializer.DeserializeNode(jsonElement);
            // var graphMousePosition = GetLocalMousePosition();
            if (originNode)
            {
                var originNarrativeNode = (NarrativeNode)originNode;
                originNodes.Add(originNarrativeNode);
                var offset = originNarrativeNode.position - originNodes[0].position;
                var copyNode = CopyNode(originNode, mousePosition + offset); //originNode.position
                copyNodes.Add(copyNode);
            }
        }

        GraphBuilder.CopyNodesConnect(this, originNodes, copyNodes);

        foreach (var jsonElement in helper.groupclipboard)
        {
            var group = JsonSerializer.Deserialize<VisualGraphGroup>(jsonElement);
            if (group != null)
            {
                var copyGroup = CopyGroup(group, mousePosition); //group.position.position
                GraphBuilder.CopyGroupedNodes(group, copyGroup, originNodes, copyNodes);
                GraphBuilder.AddGroupBlock(this, copyGroup);
            }
        }

        NarativeStickyNote firstStickyNote = null;
        foreach (var jsonElement in helper.stickyNoteclipboard)
        {
            var stickyNote = JsonSerializer.Deserialize<NarativeStickyNote>(jsonElement);
            if (firstStickyNote == null) firstStickyNote = stickyNote;
            if (stickyNote != null)
            {
                var offset = stickyNote.position.position - firstStickyNote.position.position;
                CopyStickyNote(stickyNote, mousePosition + offset); //stickyNote.position.position
            }
        }

        // SetGraph(narrativeGraph); //Refresh
    }

    public NarrativeNode CopyNode(VisualGraphNode originNode, Vector2 position)
    {
        var createNode = CreateNode(position, originNode.GetType());
        GraphBuilder.CopyFields(originNode, createNode);
        var graview= (NarrativeNodeView)createNode.graphElement;
        graview.NodeDataView.Clear();
        graview.DrawNode();
        
        var inputCount = originNode.Inputs.Count();
        while (createNode.Inputs.Count() < inputCount)
        {
            for (int i = 0; i < inputCount; i++)
            {
                CreatePort((VisualGraphNodeView)createNode.graphElement, "Input", VisualGraphPort.PortDirection.Input);
            }
        }

        var outputCount = originNode.Outputs.Count();
        while (createNode.Outputs.Count() < outputCount)
        {
            for (int i = 0; i < outputCount; i++)
                CreatePort((VisualGraphNodeView)createNode.graphElement, "Exit", VisualGraphPort.PortDirection.Output);
        }


        for (int i = 0; i < originNode.Ports.Count; i++)
        {
            var orginPort = ((NarrativePort)originNode.Ports[i]);
            if (orginPort.conditionConfig)
                ((NarrativePort)createNode.Ports[i]).conditionConfig = orginPort.conditionConfig;
        }


        return createNode;
    }

    public VisualGraphGroup CopyGroup(VisualGraphGroup graphGroup, Vector2 position)
    {
        var createGroup = GraphBuilder.JustGetCreateGroupBlock(this, position);
        createGroup.title = graphGroup.title;
        return createGroup;
    }

    public void CopyStickyNote(NarativeStickyNote stickyNote, Vector2 position)
    {
        var createStickyNote = GraphBuilder.JustGetCreateStickyNote(this, position);
        createStickyNote.contents = stickyNote.contents;
        createStickyNote.title = stickyNote.title;
        createStickyNote.theme = stickyNote.theme;
        createStickyNote.fontSize = stickyNote.fontSize;
        GraphBuilder.AddStickyNote(this, createStickyNote);
    }

    string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
    {
        var data = new CopyPasteHelper();

        foreach (VisualGraphNodeView nodeView in elements.Where(e => e is VisualGraphNodeView))
        {
            if (nodeView.nodeTarget.GetType() != typeof(VisualGraphStartNode))
                data.nodeclipboard.Add(JsonSerializer.Serialize(nodeView.nodeTarget));
        }

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (VisualGraphGroupView groupView in elements.Where(e => e is VisualGraphGroupView))
            data.groupclipboard.Add(JsonSerializer.Serialize(groupView.group));

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (StickyNote stickyNote in elements.Where(e => e is StickyNote))
            data.stickyNoteclipboard.Add(JsonSerializer.Serialize(stickyNote.userData));

        ClearSelection();

        return JsonUtility.ToJson(data, true);
    }

    bool CanPasteSerializedDataCallback(string serializedData)
    {
        try
        {
            return JsonUtility.FromJson(serializedData, typeof(CopyPasteHelper)) != null;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    public void CreateMinimap(float windowWidth)
    {
        Minimap = new MiniMap { anchored = true };
        Minimap.capabilities &= ~Capabilities.Movable;
        Minimap.SetPosition(new Rect(windowWidth - 210, 30, 200, 140));
        Add(Minimap);
    }

    // public void CreateBlackboard()
    // {
    //     BlackboardView = new BlackboardView();
    //     BlackboardView.visualGraphView = this;
    //     Blackboard.SetPosition(new Rect(10, 30, 250, 300));
    //     Add(BlackboardView.blackboard);
    // }

    #region View OnGUI/Update

    public void OnGUI()
    {
        if (Minimap != null) Minimap.SetPosition(new Rect(contentRect.width - 210, 30, 200, 140));
        // if (Blackboard != null)
        //     Blackboard.SetPosition(new Rect(10, 30, Blackboard.style.width.value.value,
        //         Blackboard.style.height.value.value));
    }

    public void Update()
    {
        nodes.ForEach(nodeView =>
        {
            VisualGraphNode node = nodeView.userData as VisualGraphNode;
            if (node != null)
            {
                if (node.editor_ActiveNode)
                {
                    nodeView.AddToClassList("VisualGraphNodeSelected");
                }
                else
                {
                    nodeView.RemoveFromClassList("VisualGraphNodeSelected");
                }
            }
        });
    }

    #endregion

    #region Init the Graph

    public void OnUndoRedoCallback()
    {
        SetGraph(narrativeGraph);
    }

    /// <summary>
    /// Load the Visual Graph into the Editor Graph View
    /// </summary>
    /// <param name="_graph"></param>
    public void SetGraph(NarrativeGraph _visualGraph)
    {
        // Set the graph to null and clear the edges and nodes before we get going.
        narrativeGraph = null;
        DeleteElements(graphElements.ToList());
        DeleteElements(nodes.ToList());
        DeleteElements(edges.ToList());
        activeVisualGraph = false;
        //BlackboardView.ClearBlackboard();

        narrativeGraph = _visualGraph;
        if (narrativeGraph != null)
        {
            // When the graph is loaded connections need to be remade
            narrativeGraph.InitializeGraph();

            activeVisualGraph = true;

            GraphOrientationAttribute orientationAttrib =
                narrativeGraph.GetType().GetCustomAttribute<GraphOrientationAttribute>();
            Debug.Assert(orientationAttrib != null,
                $"Graph node requires a GraphOrientationAttribute {narrativeGraph.GetType().Name}");
            orientation = (orientationAttrib.GrapOrientation == GraphOrientationAttribute.Orientation.Horizontal)
                ? Orientation.Horizontal
                : Orientation.Vertical;

            //BlackboardView.SetVisualGraph(visualGraph);

            searchWindow = ScriptableObject.CreateInstance<NarrativeGraphSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);

            // If the graph doesn't have a start node it's probably the first time we opened it. This means
            // we will create one to get going.
            if (narrativeGraph.StartingNode == null)
            {
                VisualGraphNode startingNode =
                    Activator.CreateInstance(typeof(VisualGraphStartNode)) as VisualGraphNode;
                narrativeGraph.StartingNode = startingNode;
                startingNode.name = "Start";
                startingNode.position = new Vector2(270, 30);

                VisualGraphPort graphPort = startingNode.AddPort("Next", VisualGraphPort.PortDirection.Output);
                graphPort.CanBeRemoved = false;
                narrativeGraph.Nodes.Add(startingNode);

                if (startingNode.name == null || startingNode.name.Trim() == "")
                    startingNode.name = UnityEditor.ObjectNames.NicifyVariableName(startingNode.name);
                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(narrativeGraph)))
                    AssetDatabase.AddObjectToAsset(startingNode, narrativeGraph);

                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(startingNode));
            }

            // Create all other Nodes for the graph
            foreach (var graphNode in narrativeGraph.Nodes)
            {
                Node node = AddGraphNode((VisualGraphNode)graphNode);
                Vector2 pos = new Vector2(node.style.left.value.value, node.style.top.value.value);
                ((VisualGraphNode)node.userData).position = pos;
            }

            foreach (VisualGraphNode graphNode in narrativeGraph.Nodes)
            {
                foreach (VisualGraphPort graphPort in graphNode.Ports)
                {
                    if (graphPort.Direction == VisualGraphPort.PortDirection.Output)
                    {
                        Port port = graphPort.editor_port as Port;
                        foreach (VisualGraphPort.VisualGraphPortConnection graph_connection in
                            graphPort.Connections)
                        {
                            VisualGraphPort other_port =
                                graph_connection.Node.FindPortByGuid(graph_connection.port_guid);
                            Port other_editor_port = other_port.editor_port as Port;
                            AddElement(port.ConnectTo(other_editor_port));
                        }
                    }
                }
            }

            foreach (var group in narrativeGraph.Groups)
            {
                GraphBuilder.AddGroupBlock(this, group);
            }

            foreach (var stickyNote in narrativeGraph.StickyNotes)
            {
                GraphBuilder.AddStickyNote(this, stickyNote);
            }
        }
    }
    

    #endregion

    #region Node Creation

    /// <summary>
    /// Create a node based off the type. Once the node is created it will be added to the Graph and a View
    /// node will be created and added
    /// </summary>
    /// <param name="position"></param>
    /// <param name="nodeType"></param>
    public NarrativeNode CreateNode(Vector2 position, Type nodeType)
    {
        Undo.RecordObject(narrativeGraph, "Create Node");
        NarrativeNode graphNode = (NarrativeNode)narrativeGraph.AddNode(nodeType);
        Undo.RegisterCreatedObjectUndo(graphNode, "Create Node");
        graphNode.position = position;

        if (graphNode.name == null || graphNode.name.Trim() == "")
            graphNode.name = UnityEditor.ObjectNames.NicifyVariableName(nodeType.Name);
        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(narrativeGraph)))
            AssetDatabase.AddObjectToAsset(graphNode, narrativeGraph);
        Node node = AddGraphNode((NarrativeNode)graphNode);
        return graphNode;
    }

    /// <summary>
    /// Add the node from the graph to the view based off property and attribute settings
    /// </summary>
    /// <param name="graphNode"></param>
    /// <returns></returns>
    private Node AddGraphNode(VisualGraphNode graphNode)
    {
        // By default we will create all nodes from VisualGraphNodeView
        Type visualNodeType = typeof(VisualGraphNodeView);
        if (visualGraphNodeLookup.ContainsKey(graphNode.GetType()) == true)
        {
            visualNodeType = visualGraphNodeLookup[graphNode.GetType()];
        }

        // Create the Node View based off the type, set the class for styling
        VisualGraphNodeView nodeView = Activator.CreateInstance(visualNodeType) as VisualGraphNodeView;
        nodeView.AddToClassList("VisualGraphNode");

        // nodeView.title = string.IsNullOrWhiteSpace(graphNode.NodeTitleName)
        //     ? GetGraphNodeName(graphNode.GetType())
        //     : graphNode.NodeTitleName;
        nodeView.title = GetGraphNodeName(graphNode.GetType());

        nodeView.userData = graphNode;
        nodeView.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        nodeView.Initialize(graphNode);
        // The Editor stores a reference to the graph created;
        graphNode.graphElement = nodeView;

        // If there are extra Styles apply them
        IEnumerable<CustomNodeStyleAttribute> customStyleAttribs =
            graphNode.GetType().GetCustomAttributes<CustomNodeStyleAttribute>();
        if (customStyleAttribs != null)
        {
            foreach (var customStyleAttrib in customStyleAttribs)
            {
                try
                {
                    StyleSheet styleSheet = Resources.Load<StyleSheet>(customStyleAttrib.style);
                    if (styleSheet != null)
                    {
                        nodeView.styleSheets.Add(styleSheet);
                    }
                    else throw new Exception();
                }
#pragma warning disable 168
                catch (Exception ex)
#pragma warning restore 168
                {
                    Debug.LogWarning($"Style sheet does not exit: {customStyleAttrib.style}");
                }
            }
        }

        // Get the Port Dynamics. Base class type already has the attribute so this should never fail
        NodePortAggregateAttribute dynamicsAttrib =
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
        foreach (var graphPort in graphNode.Ports)
        {
            AddPort(graphPort, nodeView);
        }

        // If there are custom elements or drawing, let the derived node handle this


        // If the node wants to hide the properties the user must make a View node and set this to false
        if (nodeView.ShowNodeProperties)
        {
            VisualElement divider = new VisualElement();
            divider.style.borderBottomColor = divider.style.borderTopColor =
                divider.style.borderLeftColor = divider.style.borderRightColor = Color.black;
            divider.style.borderBottomWidth = divider.style.borderTopWidth =
                divider.style.borderLeftWidth = divider.style.borderRightWidth = 0.5f;
            nodeView.mainContainer.Add(divider);

            VisualElement node_data = new VisualElement();
            node_data.AddToClassList("node_data");
            nodeView.mainContainer.Add(node_data);

            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor((VisualGraphNode)nodeView.userData);
            IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            node_data.Add(inspectorIMGUI);
        }

        nodeView.DrawNode();
        // Finally add the element
        AddElement(nodeView);

        // Refresh the view
        nodeView.RefreshExpandedState();
        nodeView.RefreshPorts();

        return nodeView;
    }

    private string GetGraphNodeName(Type type)
    {
        string display_name = "";
        if (type.GetCustomAttribute<NodeNameAttribute>() != null)
        {
            display_name = type.GetCustomAttribute<NodeNameAttribute>().name;
        }
        else
        {
            display_name = type.Name;
        }

        return display_name;
    }

    #endregion

    #region Port Connections

    /// <summary>
    /// Create a port for the given node based off the direction. Once the port is created for the node in the graph
    /// a port will be added to the view node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    /// <param name="direction"></param>
    public VisualGraphPort CreatePort(Node node, string name, VisualGraphPort.PortDirection direction)
    {
        VisualGraphNode graphNode = node.userData as VisualGraphNode;
        Undo.RecordObject(graphNode, "Add Port to Node");

        VisualGraphPort graphPort = graphNode.AddPort(name, direction);
        AddPort(graphPort, node);

        EditorUtility.SetDirty(narrativeGraph);
        return graphPort;
    }

    /// <summary>
    /// Add a port to the view node
    /// </summary>
    /// <param name="graphPort"></param>
    /// <param name="node"></param>
    private void AddPort(VisualGraphPort graphPort, Node node)
    {
        VisualGraphNode graphNode = node.userData as VisualGraphNode;

        // Determine the direction of the port (In/Out)
        Direction direction = (graphPort.Direction == VisualGraphPort.PortDirection.Input)
            ? Direction.Input
            : Direction.Output;

        // Get the capacity of the port (how many connections can this port have)
        PortCapacityAttribute capacityAttrib = graphNode.GetType().GetCustomAttribute<PortCapacityAttribute>();
        Debug.Assert(capacityAttrib != null,
            $"Graph node requires a PortCapacityAttribute {graphNode.GetType().Name}");
        Port.Capacity capacity = Port.Capacity.Single;
        if (graphPort.Direction == VisualGraphPort.PortDirection.Input)
        {
            capacity = (capacityAttrib.InputPortCapacity == PortCapacityAttribute.Capacity.Single)
                ? Port.Capacity.Single
                : Port.Capacity.Multi;
        }
        else
        {
            capacity = (capacityAttrib.OutputPortCapacity == PortCapacityAttribute.Capacity.Single)
                ? Port.Capacity.Single
                : Port.Capacity.Multi;
        }

        // Get the data type for the port.
        // TODO: can we optimze/change this to be more dynamic?
        Type port_type = (graphPort.Direction == VisualGraphPort.PortDirection.Input)
            ? graphNode.InputType
            : graphNode.OutputType;

        // Create the port based off supplied information
        var port = node.InstantiatePort(orientation, direction, capacity, port_type);
        port.portName = ""; // Don't set the name this helps with the view.
        port.userData = graphPort;
        graphPort.editor_port = port;


        // Custom View for ports
        NodePortAggregateAttribute portAggregateAttrib =
            graphNode.GetType().GetCustomAttribute<NodePortAggregateAttribute>();
        NodePortAggregateAttribute.PortAggregate aggregate = NodePortAggregateAttribute.PortAggregate.None;
        if (graphPort.Direction == VisualGraphPort.PortDirection.Input)
        {
            aggregate = (portAggregateAttrib.InputPortAggregate == NodePortAggregateAttribute.PortAggregate.Single)
                ? NodePortAggregateAttribute.PortAggregate.Single
                : NodePortAggregateAttribute.PortAggregate.Multiple;
        }
        else
        {
            aggregate = (portAggregateAttrib.OutputPortAggregate == NodePortAggregateAttribute.PortAggregate.Single)
                ? NodePortAggregateAttribute.PortAggregate.Single
                : NodePortAggregateAttribute.PortAggregate.Multiple;
        }

        VisualGraphPortView graphPortView = null;
        if (aggregate == NodePortAggregateAttribute.PortAggregate.Single)
        {
            graphPortView = Activator.CreateInstance(typeof(VisualGraphLabelPortView)) as VisualGraphLabelPortView;
        }
        else
        {
            Type portViewType = null;
            visualGraphPortLookup.TryGetValue(graphPort.GetType(), out portViewType);
            if (portViewType == null)
            {
                portViewType = typeof(VisualGraphPortView);
            }

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
            //deleteButton.AddToClassList("RemovePortButton");
            deleteButton.name = "RemovePortButton";
            port.Add(deleteButton);
        }

        port.AddManipulator(new EdgeConnector<Edge>(this));

        PortTypeAttribute portTypeAttribute = graphNode.GetType().GetCustomAttribute<PortTypeAttribute>();

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
    /// Connect the nodes together through the port Connections
    /// </summary>
    /// <param name="graphView"></param>
    /// <param name="edge"></param>
    public void OnDrop(GraphView graphView, Edge edge)
    {
        VisualGraphNode graph_input_node = edge.input.node.userData as VisualGraphNode;
        VisualGraphNode graph_output_node = edge.output.node.userData as VisualGraphNode;

        Undo.RecordObjects(new UnityEngine.Object[] { graph_input_node, graph_output_node }, "Add Port to Node");

        VisualGraphPort graph_input_port = edge.input.userData as VisualGraphPort;
        VisualGraphPort graph_output_port = edge.output.userData as VisualGraphPort;

        graph_input_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection()
        {
            initialized = true,
            Node = edge.output.node.userData as VisualGraphNode,
            port = graph_output_port,
            port_guid = graph_output_port.guid,
            node_guid = graph_output_node.guid
        });
        graph_output_port.Connections.Add(new VisualGraphPort.VisualGraphPortConnection()
        {
            initialized = true,
            Node = edge.input.node.userData as VisualGraphNode,
            port = graph_input_port,
            port_guid = graph_input_port.guid,
            node_guid = graph_input_node.guid
        });

        EditorUtility.SetDirty(narrativeGraph);
    }

    /// <summary>
    /// Not needed for default graphing. May be implemented in future versions
    /// </summary>
    /// <param name="edge"></param>
    /// <param name="position"></param>
    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
    }

    /// <summary>
    /// Remove the given port from the node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="socket"></param>
    private void RemovePort(Node node, Port socket)
    {
        VisualGraphPort socket_port = socket.userData as VisualGraphPort;
        List<Edge> edgeList = edges.ToList();
        foreach (var edge in edgeList)
        {
            VisualGraphPort graphPort = edge.output.userData as VisualGraphPort;
            if (graphPort.guid.Equals(socket_port.guid))
            {
                RemoveEdge(edge);
                edge.input.Disconnect(edge);
                RemoveElement(edge);
            }
        }

        VisualGraphNode graphNode = node.userData as VisualGraphNode;

        Undo.RecordObject(graphNode, "Remove Port");

        graphNode.Ports.Remove(socket_port);

        if (socket.direction == Direction.Input)
        {
            node.inputContainer.Remove(socket);
        }
        else
        {
            node.outputContainer.Remove(socket);
        }

        node.RefreshPorts();
        node.RefreshExpandedState();

        EditorUtility.SetDirty(narrativeGraph);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        var startPortView = startPort;

        ports.ForEach((port) =>
        {
            var portView = port;

            var startNode = (VisualGraphNode)startPortView.node.userData;
            var portNode = (VisualGraphNode)portView.node.userData;
            var hasConnectSamNodeAttribute =
                startNode.GetType().GetCustomAttribute<CanNotConnectSameNodeAttribute>() != null;
            var connectSameTypeNodeCheck =
                (hasConnectSamNodeAttribute && portNode.GetType() != startNode.GetType()) ||
                !hasConnectSamNodeAttribute;

            if (startPortView.direction != portView.direction //Direction is different
                && startPortView != portView //PortViewDifferent
                && startPortView.node != portView.node //nodeDifferent
                && (startPortView.portType == portView.portType) //PortTypeSame
                && connectSameTypeNodeCheck) //ConnectSameTypeNodeCheck 
            {
                if (typeof(NarrativeNode).IsInstanceOfType(startNode))
                {
                    if (((NarrativeNode)startNode).CompatiblePortCondition(startPortView.direction, portNode))
                        compatiblePorts.Add(port);
                }
                else
                {
                    compatiblePorts.Add(port);
                }
            }
        });

        return compatiblePorts;
    }

    #endregion

    #region Graph Changes

    private GraphViewChange OnGraphChange(GraphViewChange change)
    {
        // Return from this if we don't have a Visual Graph. This means we are reseting the
        // graph
        if (narrativeGraph == null) return change;

        if (change.elementsToRemove != null)
        {
            foreach (GraphElement element in change.elementsToRemove)
            {
                if (typeof(Edge).IsInstanceOfType(element) == true)
                {
                    VisualGraphNode graph_input_node = ((Edge)element).input.node.userData as VisualGraphNode;
                    VisualGraphNode graph_output_node = ((Edge)element).output.node.userData as VisualGraphNode;
                    Undo.RecordObjects(new UnityEngine.Object[] { graph_input_node, graph_output_node },
                        "Add Port to Node");

                    RemoveEdge((Edge)element);
                }
                else if (typeof(Node).IsInstanceOfType(element) == true)
                {
                    RemoveNode((Node)element);
                }
                else if (typeof(Group).IsInstanceOfType(element) == true)
                {
                    VisualGraphGroup block = ((Group)element).userData as VisualGraphGroup;
                    Undo.RecordObjects(new UnityEngine.Object[] { narrativeGraph }, "Add Port to Node");
                    narrativeGraph.Groups.Remove(block);
                }
                else if (typeof(StickyNote).IsInstanceOfType(element) == true)
                {
                    NarativeStickyNote stickyNote = ((StickyNote)element).userData as NarativeStickyNote;
                    Undo.RecordObjects(new UnityEngine.Object[] { narrativeGraph }, "Remove StickyNote");
                    narrativeGraph.StickyNotes.Remove(stickyNote);
                }
            }
        }

        if (change.movedElements != null)
        {
            List<VisualGraphNode> movedNodes = new List<VisualGraphNode>();
            foreach (GraphElement element in change.movedElements)
            {
                if (typeof(Node).IsInstanceOfType(element) == true)
                {
                    movedNodes.Add((VisualGraphNode)element.userData);
                }
            }

            Undo.RecordObjects(movedNodes.ToArray(), "Moved VisualGraphNode");

            foreach (GraphElement element in change.movedElements)
            {
                if (typeof(Node).IsInstanceOfType(element) == true)
                {
                    movedNodes.Add((VisualGraphNode)element.userData);
                    ((VisualGraphNode)element.userData).position = new Vector2(element.style.left.value.value,
                        element.style.top.value.value);
                }
                else if (typeof(VisualGraphGroupView).IsInstanceOfType(element) == true)
                {
                    var position = new Vector2(element.style.left.value.value,
                        element.style.top.value.value);
                    var size = element.GetPosition();
                    ((VisualGraphGroup)element.userData).position =
                        new Rect(position, new Vector2(size.width, size.height));
                    VisualGraphGroupView groupView = (VisualGraphGroupView)element;
                    foreach (var graphElement in groupView.containedElements)
                    {
                        if (typeof(Node).IsInstanceOfType(graphElement) == true)
                        {
                            ((VisualGraphNode)graphElement.userData).position = new Vector2(
                                graphElement.style.left.value.value,
                                graphElement.style.top.value.value);
                        }
                    }
                }
                else if (typeof(StickyNote).IsInstanceOfType(element) == true)
                {
                    var position = new Vector2(element.style.left.value.value,
                        element.style.top.value.value);
                    var size = element.GetPosition();
                    ((NarativeStickyNote)element.userData).position =
                        new Rect(position, new Vector2(size.width, size.height));
                }

                //Undo.RecordObjects(movedGroups.ToArray(), "Moved VisualGraphNode");
            }
        }

        EditorUtility.SetDirty(narrativeGraph);

        return change;
    }

    private void RemoveNode(Node node)
    {
        VisualGraphNode graphNode = node.userData as VisualGraphNode;
        Undo.RecordObjects(new UnityEngine.Object[] { graphNode, narrativeGraph }, "Delete Node");
        narrativeGraph.RemoveNode(graphNode);
        Undo.DestroyObjectImmediate(graphNode);
        //RefreshGraphAsset();
    }

    private void RemoveEdge(Edge edge)
    {
        VisualGraphPort graph_input_port = edge.input.userData as VisualGraphPort;
        VisualGraphPort graph_output_port = edge.output.userData as VisualGraphPort;
        graph_input_port.RemoveConnectionByPortGuid(graph_output_port.guid);
        graph_output_port.RemoveConnectionByPortGuid(graph_input_port.guid);
    }

    #endregion

    #region PrivateMethod

    public void RefreshGraphAsset()
    {
        var assetPath = AssetDatabase.GetAssetPath(narrativeGraph);
        if (!string.IsNullOrWhiteSpace(assetPath))
        {
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    #endregion

    #region Utilities

    public Vector2 GetLocalMousePosition(Vector2 localMousePosition)
    {
        //Convertit localMousePosition pour l'adapter au Rect de la fen�tre
        Vector2 graphMousePos = contentViewContainer.WorldToLocal(localMousePosition);

        return graphMousePos;
    }

    public Vector2 GetMousePosition(IMouseEvent evt)
    {
        VisualElement container = ElementAt(1);
        Vector3 screenMousePosition = evt.localMousePosition;
        var position = screenMousePosition - container.transform.position;
        position *= 1 / container.transform.scale.x;
        return position;
    }
    #endregion
}