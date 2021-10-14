//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEditor;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(ConfigComponent))]
    internal sealed class ConfigComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_CachedBytesSize;

        private readonly HelperInfo<ConfigHelperBase> m_ConfigHelperInfo = new HelperInfo<ConfigHelperBase>("Config");
        private SerializedProperty m_EnableLoadConfigDependencyAssetEvent;
        private SerializedProperty m_EnableLoadConfigUpdateEvent;

        private void OnEnable()
        {
            m_EnableLoadConfigUpdateEvent = serializedObject.FindProperty("m_EnableLoadConfigUpdateEvent");
            m_EnableLoadConfigDependencyAssetEvent =
                serializedObject.FindProperty("m_EnableLoadConfigDependencyAssetEvent");
            m_CachedBytesSize = serializedObject.FindProperty("m_CachedBytesSize");

            m_ConfigHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var t = (ConfigComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_EnableLoadConfigUpdateEvent);
                EditorGUILayout.PropertyField(m_EnableLoadConfigDependencyAssetEvent);
                m_ConfigHelperInfo.Draw();
                EditorGUILayout.PropertyField(m_CachedBytesSize);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Config Count", t.Count.ToString());
                EditorGUILayout.LabelField("Cached Bytes Size", t.CachedBytesSize.ToString());
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
            m_ConfigHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
    }
}