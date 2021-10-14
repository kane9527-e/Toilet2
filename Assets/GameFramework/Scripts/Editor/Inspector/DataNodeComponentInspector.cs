//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataNode;
using UnityEditor;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(DataNodeComponent))]
    internal sealed class DataNodeComponentInspector : GameFrameworkInspector
    {
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            var t = (DataNodeComponent)target;

            if (IsPrefabInHierarchy(t.gameObject)) DrawDataNode(t.Root);

            Repaint();
        }

        private void DrawDataNode(IDataNode dataNode)
        {
            EditorGUILayout.LabelField(dataNode.FullName, dataNode.ToDataString());
            var child = dataNode.GetAllChild();
            foreach (var c in child) DrawDataNode(c);
        }
    }
}