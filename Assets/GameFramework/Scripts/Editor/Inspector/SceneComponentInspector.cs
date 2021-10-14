//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(SceneComponent))]
    internal sealed class SceneComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_EnableLoadSceneDependencyAssetEvent;
        private SerializedProperty m_EnableLoadSceneUpdateEvent;

        private void OnEnable()
        {
            m_EnableLoadSceneUpdateEvent = serializedObject.FindProperty("m_EnableLoadSceneUpdateEvent");
            m_EnableLoadSceneDependencyAssetEvent =
                serializedObject.FindProperty("m_EnableLoadSceneDependencyAssetEvent");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var t = (SceneComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_EnableLoadSceneUpdateEvent);
                EditorGUILayout.PropertyField(m_EnableLoadSceneDependencyAssetEvent);
            }
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Loaded Scene Asset Names",
                    GetSceneNameString(t.GetLoadedSceneAssetNames()));
                EditorGUILayout.LabelField("Loading Scene Asset Names",
                    GetSceneNameString(t.GetLoadingSceneAssetNames()));
                EditorGUILayout.LabelField("Unloading Scene Asset Names",
                    GetSceneNameString(t.GetUnloadingSceneAssetNames()));
                EditorGUILayout.ObjectField("Main Camera", t.MainCamera, typeof(Camera), true);

                Repaint();
            }
        }

        private string GetSceneNameString(string[] sceneAssetNames)
        {
            if (sceneAssetNames == null || sceneAssetNames.Length <= 0) return "<Empty>";

            var sceneNameString = string.Empty;
            foreach (var sceneAssetName in sceneAssetNames)
            {
                if (!string.IsNullOrEmpty(sceneNameString)) sceneNameString += ", ";

                sceneNameString += SceneComponent.GetSceneName(sceneAssetName);
            }

            return sceneNameString;
        }
    }
}