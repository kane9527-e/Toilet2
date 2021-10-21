using System;
using System.Collections.Generic;
using GameMain.Scripts.Runtime.Base;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace GameMain.Scripts.Component.Mono
{
    /// <summary>
    ///     资源库组件
    /// </summary>
    [DisallowMultipleComponent]
    public class AssetLibraryManager : MonoBehaviour
    {
        [SerializeField] private List<AssetLibrary.AssetLibrary> assetLibraries = new List<AssetLibrary.AssetLibrary>();

        public List<AssetLibrary.AssetLibrary> AssetLibraries => assetLibraries;

        #region Singleton

        private static AssetLibraryManager _instance;

        public static AssetLibraryManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<AssetLibraryManager>();
                    if (!_instance)
                        _instance = new GameObject(nameof(AssetLibraryManager)).AddComponent<AssetLibraryManager>();
                }

                return _instance;
            }
        }

        #endregion
        
        #region PublicMethod

        public T GetAsset<T>(string assetName) where T : Object
        {
            var assetLibrary =
                assetLibraries.Find(library => library.TypeName.ToLower().Equals(typeof(T).Name.ToLower()));
            if (assetLibrary == null) return default;

            var asset = assetLibrary.Library.Find(asset => asset.name.Equals(assetName));
            if (asset)
                return asset as T;
            return default;
        }

        #endregion
    }
}