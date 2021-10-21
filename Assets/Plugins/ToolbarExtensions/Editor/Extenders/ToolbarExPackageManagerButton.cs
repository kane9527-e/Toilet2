using ToolbarExtensions;
using UnityEditor;
using UnityEngine;

namespace Extensions.ToolbarExtensions.Editor.Extenders
{
    [InitializeOnLoad]
    static class ToolbarExPackageManagerButton
    {
        static ToolbarExPackageManagerButton()
        {
#if UNITY_2021_1_OR_NEWER
            var button = ToolbarExtender.CreateToolbarButton("Package Manager", ShowPackageManager);
            ToolbarElement element = ToolbarElement.Create(button, ExtenderType.Right);
            ToolbarExtender.ToolbarExtend(element);
#else
            ToolbarExtender.RightToolbarGUI.Add(delegate
            {
                GUILayout.FlexibleSpace();
                var icon = EditorGUIUtility.FindTexture("Package Manager");
                if (GUILayout.Button(icon, GUILayout.ExpandWidth(false)))
                    ShowPackageManager();
            });
#endif
        }

        private static void ShowPackageManager()
        {
            UnityEditor.PackageManager.UI.Window.Open("");
        }
    }
}