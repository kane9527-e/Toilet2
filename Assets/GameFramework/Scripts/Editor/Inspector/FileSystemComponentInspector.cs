﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.FileSystem;
using UnityEditor;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(FileSystemComponent))]
    internal sealed class FileSystemComponentInspector : GameFrameworkInspector
    {
        private readonly HelperInfo<FileSystemHelperBase> m_FileSystemHelperInfo =
            new HelperInfo<FileSystemHelperBase>("FileSystem");

        private void OnEnable()
        {
            m_FileSystemHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var t = (FileSystemComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_FileSystemHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("File System Count", t.Count.ToString());

                var fileSystems = t.GetAllFileSystems();
                foreach (var fileSystem in fileSystems) DrawFileSystem(fileSystem);
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_FileSystemHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawFileSystem(IFileSystem fileSystem)
        {
            EditorGUILayout.LabelField(fileSystem.FullPath,
                Utility.Text.Format("{0}, {1} / {2} Files", fileSystem.Access.ToString(),
                    fileSystem.FileCount.ToString(), fileSystem.MaxFileCount.ToString()));
        }
    }
}