//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using GameFramework;
using UnityEditor;

namespace UnityGameFramework.Editor.ResourceTools
{
    public sealed class ResourceSyncToolsController
    {
        public event GameFrameworkAction<int, int> OnLoadingResource;

        public event GameFrameworkAction<int, int> OnLoadingAsset;

        public event GameFrameworkAction OnCompleted;

        public event GameFrameworkAction<int, int, string> OnResourceDataChanged;

        public string[] GetAllAssetBundleNames()
        {
            return AssetDatabase.GetAllAssetBundleNames();
        }

        public string[] GetUsedAssetBundleNames()
        {
            var hashSet = new HashSet<string>(GetAllAssetBundleNames());
            hashSet.ExceptWith(GetUnusedAssetBundleNames());
            return hashSet.ToArray();
        }

        public string[] GetUnusedAssetBundleNames()
        {
            return AssetDatabase.GetUnusedAssetBundleNames();
        }

        public string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
            return AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
        }

        public string[] GetAssetPathsFromAssetBundleAndAssetName(string assetBundleName, string assetName)
        {
            return AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
        }

        public bool RemoveAssetBundleName(string assetBundleName, bool forceRemove)
        {
            return AssetDatabase.RemoveAssetBundleName(assetBundleName, forceRemove);
        }

        public void RemoveUnusedAssetBundleNames()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        public bool RemoveAllAssetBundleNames()
        {
            var allAssetNames = new HashSet<string>();
            var assetBundleNames = GetUsedAssetBundleNames();
            foreach (var assetBundleName in assetBundleNames)
            {
                var assetNames = GetAssetPathsFromAssetBundle(assetBundleName);
                foreach (var assetName in assetNames) allAssetNames.Add(assetName);
            }

            var assetIndex = 0;
            var assetCount = allAssetNames.Count;
            foreach (var assetName in allAssetNames)
            {
                var assetImporter = AssetImporter.GetAtPath(assetName);
                if (assetImporter == null)
                {
                    if (OnCompleted != null) OnCompleted();

                    return false;
                }

                assetImporter.assetBundleVariant = null;
                assetImporter.assetBundleName = null;
                assetImporter.SaveAndReimport();

                if (OnResourceDataChanged != null) OnResourceDataChanged(++assetIndex, assetCount, assetName);
            }

            RemoveUnusedAssetBundleNames();

            if (OnCompleted != null) OnCompleted();

            return true;
        }

        public bool SyncToProject()
        {
            var resourceCollection = new ResourceCollection();

            resourceCollection.OnLoadingResource += delegate(int index, int count)
            {
                if (OnLoadingResource != null) OnLoadingResource(index, count);
            };

            resourceCollection.OnLoadingAsset += delegate(int index, int count)
            {
                if (OnLoadingAsset != null) OnLoadingAsset(index, count);
            };

            resourceCollection.OnLoadCompleted += delegate
            {
                if (OnCompleted != null) OnCompleted();
            };

            if (!resourceCollection.Load()) return false;

            var assetIndex = 0;
            var assetCount = resourceCollection.AssetCount;
            var resources = resourceCollection.GetResources();
            foreach (var resource in resources)
            {
                if (resource.IsLoadFromBinary) continue;

                var assets = resource.GetAssets();
                foreach (var asset in assets)
                {
                    var assetImporter = AssetImporter.GetAtPath(asset.Name);
                    if (assetImporter == null)
                    {
                        if (OnCompleted != null) OnCompleted();

                        return false;
                    }

                    assetImporter.assetBundleName = resource.Name;
                    assetImporter.assetBundleVariant = resource.Variant;
                    assetImporter.SaveAndReimport();

                    if (OnResourceDataChanged != null) OnResourceDataChanged(++assetIndex, assetCount, asset.Name);
                }
            }

            if (OnCompleted != null) OnCompleted();

            return true;
        }

        public bool SyncFromProject()
        {
            var resourceCollection = new ResourceCollection();
            var assetBundleNames = GetUsedAssetBundleNames();
            foreach (var assetBundleName in assetBundleNames)
            {
                var name = assetBundleName;
                string variant = null;
                var dotPosition = assetBundleName.LastIndexOf('.');
                if (dotPosition > 0 && dotPosition < assetBundleName.Length - 1)
                {
                    name = assetBundleName.Substring(0, dotPosition);
                    variant = assetBundleName.Substring(dotPosition + 1);
                }

                if (!resourceCollection.AddResource(name, variant, null, LoadType.LoadFromFile, false)) return false;

                var assetNames = GetAssetPathsFromAssetBundle(assetBundleName);
                foreach (var assetName in assetNames)
                {
                    var guid = AssetDatabase.AssetPathToGUID(assetName);
                    if (string.IsNullOrEmpty(guid)) return false;

                    if (!resourceCollection.AssignAsset(guid, name, variant)) return false;
                }
            }

            return resourceCollection.Save();
        }
    }
}