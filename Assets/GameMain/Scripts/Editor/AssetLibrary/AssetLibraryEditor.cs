using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

namespace GameMain.Scripts.Editor.AssetLibrary
{
    [CustomEditor(typeof(Component.Mono.AssetLibrary.AssetLibrary))]
    public class AssetLibraryEditor : UnityEditor.Editor
    {
        [DidReloadScripts]
        static void OnScriptsEditOver()//代码编译完成时调用
        {
            //注册打包发布的事件；unity在打包发布的时候会判断buildPlayerHandler 是不是为null，为空就执行默认打包方法，不为空就执行注册的事件
            BuildPlayerWindow.RegisterBuildPlayerHandler(OverridesBuildPlayer);
        }

        private static void OverridesBuildPlayer(BuildPlayerOptions BPOption)
        {
            LoadAllLibraryTypeAssets();
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(BPOption);
        }


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

        [MenuItem("Tools/AssetLibrary/Load All Library's TypeAssets")]
        public static void LoadAllLibraryTypeAssets()
        {
            var assetsGuid = AssetDatabase.FindAssets("t:AssetLibrary");
            foreach (var guid in assetsGuid)
            {
                Component.Mono.AssetLibrary.AssetLibrary libraryAsset =
                    UnityEditor.AssetDatabase.LoadAssetAtPath<Component.Mono.AssetLibrary.AssetLibrary>(
                        UnityEditor.AssetDatabase.GUIDToAssetPath(guid));

                if (!libraryAsset) continue;
                if (!string.IsNullOrWhiteSpace(libraryAsset.TypeName))
                {
                    var typeAssetsGuids =
                        UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", libraryAsset.TypeName));
                    libraryAsset.Library.Clear();
                    foreach (var typeAssetsGuid in typeAssetsGuids)
                    {
                        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath(
                            UnityEditor.AssetDatabase.GUIDToAssetPath(typeAssetsGuid),
                            typeof(Object));
                        if (asset && !libraryAsset.Library.Contains(asset))
                            libraryAsset.Library.Add(asset);
                    }
                }
            }
        }
    }
}