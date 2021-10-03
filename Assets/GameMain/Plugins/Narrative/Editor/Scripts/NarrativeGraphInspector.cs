using Narrative.Runtime.Scripts.Graph;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using VisualGraphEditor;

namespace Narrative.Editor.Scripts
{
    [CustomEditor(typeof(NarrativeGraph))]
    public class NarrativeGraphInspector : UnityEditor.Editor
    {
#if UNITY_2021
        [OnOpenAsset(0,OnOpenAssetAttributeMode.Execute)]
#elif UNITY_2020
        [OnOpenAsset(0)]
#endif
        public static bool OpenVisualGraph(int instanceID, int line)
        {
            NarrativeGraph graph = EditorUtility.InstanceIDToObject(instanceID) as NarrativeGraph;
            if (graph != null)
            {
                NarrativeGraphEditor.CreateGraphViewWindow(graph, true);
                return true;
            }

            return false;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.EndDisabledGroup();
            DrawDefaultInspector();
            EditorGUI.EndDisabledGroup();
        }
    }
}