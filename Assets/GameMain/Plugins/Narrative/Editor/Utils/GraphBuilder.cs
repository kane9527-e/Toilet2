using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Narrative.Editor.Views;
using Narrative.Runtime.Scripts.Nodes.BaseNode;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;
using VisualGraphRuntime;
using Object = UnityEngine.Object;

namespace Project.NodeSystem.Editor
{
    public static class GraphBuilder
    {
        #region Manipulators

        /// <summary>
        /// Ajoute un onglet "Add Group" dans le menu contextuel (clic droit)
        /// </summary>
        /// <returns></returns>
        public static IManipulator AddGroupContextualMenu(NarrativeGraphView graphView)
        {
            ContextualMenuManipulator cmm = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add Group ",
                    actionEvent => CreateGroupBlock(graphView,
                        graphView.GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );
            return cmm;
        }


        /// <summary>
        /// Ajoute un onglet "Add Sticky Node" dans le menu contextuel (clic droit)
        /// </summary>
        /// <returns></returns>
        public static IManipulator AddStickyNoteContextualMenu(NarrativeGraphView graphView)
        {
            ContextualMenuManipulator cmm = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add Sticky Note ",
                    actionEvent => CreateStickyNote(graphView,
                        graphView.GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );
            cmm.target = graphView;

            return cmm;
        }

        #endregion


        #region Group Block

        public static VisualGraphGroup CreateGroupBlock(this NarrativeGraphView view, Vector2 position)
        {
            var narrativeGraph = view.NarrativeGraph;
            if (!narrativeGraph) return null;
            Undo.RecordObject(narrativeGraph, "Create Node");
            VisualGraphGroup block = new VisualGraphGroup()
            {
                title = "New Graph Group",
                position = new Rect(position, new Vector2(200, 300))
            };
            if (narrativeGraph)
                narrativeGraph.Groups.Add(block);
            AddGroupBlock(view, block);
            return block;
        }

        public static VisualGraphGroup JustGetCreateGroupBlock(this NarrativeGraphView view, Vector2 position)
        {
            var narrativeGraph = view.NarrativeGraph;
            if (!narrativeGraph) return null;
            Undo.RecordObject(narrativeGraph, "Create Node");
            VisualGraphGroup block = new VisualGraphGroup()
            {
                title = "New Graph Group",
                position = new Rect(position, new Vector2(200, 300))
            };
            if (narrativeGraph)
                narrativeGraph.Groups.Add(block);
            return block;
        }


        public static void AddGroupBlock(this NarrativeGraphView view, VisualGraphGroup graphGroup)
        {
            var graphGroupView = new VisualGraphGroupView
            {
                autoUpdateGeometry = true,
                title = graphGroup.title,
                userData = graphGroup
            };

            //graphGroupView.SetPosition(graphGroup.position);
            RefreshGroupBlockNodes(view, graphGroupView, graphGroup);
            var size = graphGroupView.GetPosition();
            graphGroupView.SetPosition(new Rect(graphGroup.position.position, new Vector2(size.width, size.height)));
            view.AddElement(graphGroupView);
        }

        public static void RefreshGroupBlockNodes(NarrativeGraphView graphView, VisualGraphGroupView groupView,
            VisualGraphGroup graphGroup)
        {
            HashSet<GraphElement> nodes = new HashSet<GraphElement>();
            foreach (var node_guid in graphGroup.node_guids)
            {
                VisualGraphNode node = graphView.NarrativeGraph.FindNodeByGuid(node_guid);
                if (node != null)
                    nodes.Add(node.graphElement as GraphElement);
            }

            groupView.AddElements(nodes);
            groupView.CollectElements(nodes, null);
        }

        #endregion

        #region StickyNote

        /// <summary>
        /// Cr�e une StickyNote dans la GraphView
        /// </summary>
        /// <param name="title"></param>
        /// <param name="localMousePosition"></param>
        /// <returns></returns>
        public static NarativeStickyNote CreateStickyNote(this NarrativeGraphView view, Vector2 localMousePosition)
        {
            var narrativeGraph = view.NarrativeGraph;
            if (!narrativeGraph) return null;
            NarativeStickyNote note = new NarativeStickyNote
            {
                title = "Note",
                theme = StickyNoteTheme.Classic,
                contents = "<i>Write your text here...</i>",
                position = new Rect(localMousePosition, new Vector2(200, 200)),
                fontSize = StickyNoteFontSize.Medium
            };

            //Change la couleur du groupe dans la Minimap
            //note.ChangeColorInMinimap("#CCAA33");
            //note.AddStyle("Note");
            if (view.NarrativeGraph)
                view.NarrativeGraph.StickyNotes.Add(note);
            AddStickyNote(view, note);
            return note;
        }

        /// <summary>
        /// Cr�e une StickyNote dans la GraphView
        /// </summary>
        /// <param name="title"></param>
        /// <param name="localMousePosition"></param>
        /// <returns></returns>
        public static NarativeStickyNote JustGetCreateStickyNote(this NarrativeGraphView view,
            Vector2 localMousePosition)
        {
            var narrativeGraph = view.NarrativeGraph;
            if (!narrativeGraph) return null;
            NarativeStickyNote note = new NarativeStickyNote
            {
                title = "Note",
                theme = StickyNoteTheme.Classic,
                contents = "<i>Write your text here...</i>",
                position = new Rect(localMousePosition, new Vector2(200, 200)),
                fontSize = StickyNoteFontSize.Medium
            };

            //Change la couleur du groupe dans la Minimap
            //note.ChangeColorInMinimap("#CCAA33");
            //note.AddStyle("Note");
            if (view.NarrativeGraph)
                view.NarrativeGraph.StickyNotes.Add(note);
            return note;
        }

        public static void AddStickyNote(this NarrativeGraphView view, NarativeStickyNote stickyNote)
        {
            StickyNote noteView = new StickyNote()
            {
                title = stickyNote.title,
                contents = stickyNote.contents,
                theme = stickyNote.theme,
                fontSize = stickyNote.fontSize,
                userData = stickyNote
            };
            // TextField titleField = (TextField)noteView.ElementAt(0).ElementAt(1);
            // titleField.RegisterValueChangedCallback(ect => stickyNote.title = ect.newValue);
            //TextField contentsField = (TextField)noteView.ElementAt(0).ElementAt(2).ElementAt(0);
            // contentsField.RegisterValueChangedCallback(ect => stickyNote.contents = ect.newValue);
            // noteView.RegisterCallback<ChangeEvent<StickyNoteTheme>>(evt => stickyNote.theme = evt.newValue);
            noteView.RegisterCallback<StickyNoteChangeEvent>(evt =>
                {
                    var targetNote = ((StickyNote)evt.target);
                    stickyNote.position = targetNote.GetPosition();
                    stickyNote.title = targetNote.title;
                    stickyNote.contents = targetNote.contents;
                    stickyNote.theme = targetNote.theme;
                    stickyNote.fontSize = targetNote.fontSize;
                }
            );
            noteView.SetPosition(stickyNote.position);

            view.AddElement(noteView);
        }

        #endregion

        #region Utilities

        public static Vector2 GetLocalMousePosition(this NarrativeGraphView graphView, Vector2 localMousePosition)
        {
            //Convertit localMousePosition pour l'adapter au Rect de la fen�tre
            Vector2 graphMousePos = graphView.contentViewContainer.WorldToLocal(localMousePosition);

            return graphMousePos;
        }

        public static void CopyFields(Object origin, Object target)
        {
            Type type = origin.GetType();
            var fieldInfos = type.GetRuntimeFields();

            foreach (var info in fieldInfos)
            {
                if ((info.IsDefined(typeof(SerializeField), true)))
                {
                    var originValue = info.GetValue(origin);
                    info.SetValue(target, originValue);
                }
            }

            Type basetype = type.BaseType;
            var basefieldInfos = basetype.GetRuntimeFields();
            foreach (var info in basefieldInfos)
            {
                if ((info.IsDefined(typeof(SerializeField), true)))
                {
                    var originValue = info.GetValue(origin);
                    info.SetValue(target, originValue);
                }
            }
        }


        /// <summary>
        /// CopyNodesConnect
        /// </summary>
        /// <param name="originNodes"></param>
        /// <param name="copyNodes"></param>
        public static void CopyNodesConnect(NarrativeGraphView view, List<NarrativeNode> originNodes,
            List<NarrativeNode> copyNodes)
        {
            for (int i = 0; i < originNodes.Count; i++)
            {
                var oNode = originNodes[i];

                for (int j = 0; j < originNodes.Count; j++)
                {
                    if (i == j) continue;
                    var connectPortIndexes = oNode.GetConnectPortIndexes(originNodes[j]);
                    foreach (var portIndex in connectPortIndexes)
                    {
                        var cNode = copyNodes[i];
                        var connection = cNode.Connect((NarrativePort)cNode.Ports[portIndex], copyNodes[j]);
                    }
                }
            }

            foreach (VisualGraphNode graphNode in copyNodes)
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
                            view.AddElement(port.ConnectTo(other_editor_port));
                        }
                    }
                }
            }
        }

        public static void CopyGroupedNodes(VisualGraphGroup originGroup, VisualGraphGroup copyGroup,
            List<NarrativeNode> originNodes, List<NarrativeNode> copyNodes)
        {
            foreach (var guid in originGroup.node_guids)
            {
                var index = originNodes.FindIndex(node => node.guid == guid);
                if (index >= 0 && index < copyNodes.Count)
                {
                    var offset = originNodes[index].position - originGroup.position.position;
                    copyGroup.node_guids.Add(copyNodes[index].guid);
                    copyNodes[index].position = copyGroup.position.position + offset;
                }
            }
        }

        #endregion
    }
}