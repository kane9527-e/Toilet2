using UnityEditor;
using UnityEngine;

namespace GameMain.Scripts.Editor.AssetLibrary
{
    [CustomEditor(typeof(Component.Mono.AssetLibrary.AssetLibrary))]
    public class AssetLibraryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("LoadAllAssets"))
            {
                LoadAllTypeAssets();
            }

            serializedObject.ApplyModifiedProperties();
        }

#if UNITY_EDITOR
        public void LoadAllTypeAssets()
        {
            var typeName = serializedObject.FindProperty("typeName").stringValue;
            var libraryProperty = serializedObject.FindProperty("library");

            if (libraryProperty == null) return;

            if (!string.IsNullOrWhiteSpace(typeName))
            {
                var assetsGuid = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeName));

                libraryProperty.ClearArray();
                int index = 0;
                foreach (var guid in assetsGuid)
                {
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(
                        UnityEditor.AssetDatabase.GUIDToAssetPath(guid),
                        typeof(Object));
                    if (asset)
                    {
                        libraryProperty.InsertArrayElementAtIndex(index);
                        var assetProperty = libraryProperty.GetArrayElementAtIndex(index);
                        if (assetProperty != null)
                        {
                            assetProperty.objectReferenceValue = asset;
                        }

                        index++;
                    }
                }
            }
        }
#endif
    }
}