//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;

namespace UnityGameFramework.Editor.ResourceTools
{
    public sealed partial class ResourceBuilderController
    {
        private sealed class ResourceData
        {
            private readonly List<AssetData> m_AssetDatas;
            private readonly List<ResourceCode> m_Codes;
            private readonly string[] m_ResourceGroups;

            public ResourceData(string name, string variant, string fileSystem, LoadType loadType, bool packed,
                string[] resourceGroups)
            {
                Name = name;
                Variant = variant;
                FileSystem = fileSystem;
                LoadType = loadType;
                Packed = packed;
                m_ResourceGroups = resourceGroups;
                m_AssetDatas = new List<AssetData>();
                m_Codes = new List<ResourceCode>();
            }

            public string Name { get; }

            public string Variant { get; }

            public string FileSystem { get; }

            public bool IsLoadFromBinary => LoadType == LoadType.LoadFromBinary ||
                                            LoadType == LoadType.LoadFromBinaryAndQuickDecrypt ||
                                            LoadType == LoadType.LoadFromBinaryAndDecrypt;

            public LoadType LoadType { get; }

            public bool Packed { get; }

            public int AssetCount => m_AssetDatas.Count;

            public string[] GetResourceGroups()
            {
                return m_ResourceGroups;
            }

            public string[] GetAssetGuids()
            {
                var assetGuids = new string[m_AssetDatas.Count];
                for (var i = 0; i < m_AssetDatas.Count; i++) assetGuids[i] = m_AssetDatas[i].Guid;

                return assetGuids;
            }

            public string[] GetAssetNames()
            {
                var assetNames = new string[m_AssetDatas.Count];
                for (var i = 0; i < m_AssetDatas.Count; i++) assetNames[i] = m_AssetDatas[i].Name;

                return assetNames;
            }

            public AssetData[] GetAssetDatas()
            {
                return m_AssetDatas.ToArray();
            }

            public AssetData GetAssetData(string assetName)
            {
                foreach (var assetData in m_AssetDatas)
                    if (assetData.Name == assetName)
                        return assetData;

                return null;
            }

            public void AddAssetData(string guid, string name, int length, int hashCode, string[] dependencyAssetNames)
            {
                m_AssetDatas.Add(new AssetData(guid, name, length, hashCode, dependencyAssetNames));
            }

            public ResourceCode GetCode(Platform platform)
            {
                foreach (var code in m_Codes)
                    if (code.Platform == platform)
                        return code;

                return null;
            }

            public ResourceCode[] GetCodes()
            {
                return m_Codes.ToArray();
            }

            public void AddCode(Platform platform, int length, int hashCode, int compressedLength,
                int compressedHashCode)
            {
                m_Codes.Add(new ResourceCode(platform, length, hashCode, compressedLength, compressedHashCode));
            }
        }
    }
}