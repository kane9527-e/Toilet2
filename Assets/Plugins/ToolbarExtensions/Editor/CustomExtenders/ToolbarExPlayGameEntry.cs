using ToolbarExtensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
namespace ToolbarExtensions.Editor.CustomExtenders
{
    [InitializeOnLoad]
    static class ToolbarExPlayGameEntry
    {
        static ToolbarExPlayGameEntry()
        {
#if UNITY_2021_1_OR_NEWER
#else
            ToolbarExtender.LeftToolbarGUI.Add(delegate
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Entry", GUILayout.ExpandWidth(false)))
                    PlayGameEntry();
            });
#endif
            EditorApplication.playModeStateChanged += delegate(PlayModeStateChange change)
            {
                if (change == PlayModeStateChange.ExitingPlayMode)
                {
                    EditorSceneManager.playModeStartScene = null;
                }
            };
        }

        private static void PlayGameEntry()
        {
            if (EditorApplication.isPlaying) return;
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length <= 0) return;
            var scene = scenes[0];
            if (scene == null) return;
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
            //EditorSceneManager.OpenScene(scene.path);
            EditorApplication.EnterPlaymode();
        }
    }
}