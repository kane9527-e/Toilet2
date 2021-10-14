//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using GameFramework;
using GameFramework.Resource;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    ///     内置版本资源列表序列化器。
    /// </summary>
    public static partial class BuiltinVersionListSerializer
    {
#if UNITY_EDITOR

        /// <summary>
        ///     序列化可更新模式版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的可更新模式版本资源列表（版本 0）。</param>
        /// <returns>是否序列化可更新模式版本资源列表（版本 0）成功。</returns>
        public static bool UpdatableVersionListSerializeCallback_V0(Stream stream, UpdatableVersionList versionList)
        {
            if (!versionList.IsValid) return false;

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                binaryWriter.WriteEncryptedString(versionList.ApplicableGameVersion, s_CachedHashBytes);
                binaryWriter.Write(versionList.InternalResourceVersion);
                var assets = versionList.GetAssets();
                binaryWriter.Write(assets.Length);
                var resources = versionList.GetResources();
                binaryWriter.Write(resources.Length);
                foreach (var resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(resource.Variant, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                    binaryWriter.Write(resource.CompressedLength);
                    binaryWriter.Write(resource.CompressedHashCode);
                    var assetIndexes = resource.GetAssetIndexes();
                    binaryWriter.Write(assetIndexes.Length);
                    var hashBytes = new byte[CachedHashBytesLength];
                    foreach (var assetIndex in assetIndexes)
                    {
                        Utility.Converter.GetBytes(resource.HashCode, hashBytes);
                        var asset = assets[assetIndex];
                        binaryWriter.WriteEncryptedString(asset.Name, hashBytes);
                        var dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
                        binaryWriter.Write(dependencyAssetIndexes.Length);
                        foreach (var dependencyAssetIndex in dependencyAssetIndexes)
                            binaryWriter.WriteEncryptedString(assets[dependencyAssetIndex].Name, hashBytes);
                    }
                }

                var resourceGroups = versionList.GetResourceGroups();
                binaryWriter.Write(resourceGroups.Length);
                foreach (var resourceGroup in resourceGroups)
                {
                    binaryWriter.WriteEncryptedString(resourceGroup.Name, s_CachedHashBytes);
                    var resourceIndexes = resourceGroup.GetResourceIndexes();
                    binaryWriter.Write(resourceIndexes.Length);
                    foreach (ushort resourceIndex in resourceIndexes) binaryWriter.Write(resourceIndex);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        ///     序列化可更新模式版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的可更新模式版本资源列表（版本 1）。</param>
        /// <returns>是否序列化可更新模式版本资源列表（版本 1）成功。</returns>
        public static bool UpdatableVersionListSerializeCallback_V1(Stream stream, UpdatableVersionList versionList)
        {
            if (!versionList.IsValid) return false;

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                binaryWriter.WriteEncryptedString(versionList.ApplicableGameVersion, s_CachedHashBytes);
                binaryWriter.Write7BitEncodedInt32(versionList.InternalResourceVersion);
                var assets = versionList.GetAssets();
                binaryWriter.Write7BitEncodedInt32(assets.Length);
                foreach (var asset in assets)
                {
                    binaryWriter.WriteEncryptedString(asset.Name, s_CachedHashBytes);
                    var dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
                    binaryWriter.Write7BitEncodedInt32(dependencyAssetIndexes.Length);
                    foreach (var dependencyAssetIndex in dependencyAssetIndexes)
                        binaryWriter.Write7BitEncodedInt32(dependencyAssetIndex);
                }

                var resources = versionList.GetResources();
                binaryWriter.Write7BitEncodedInt32(resources.Length);
                foreach (var resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(resource.Variant, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(
                        resource.Extension != DefaultExtension ? resource.Extension : null, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write7BitEncodedInt32(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                    binaryWriter.Write7BitEncodedInt32(resource.CompressedLength);
                    binaryWriter.Write(resource.CompressedHashCode);
                    var assetIndexes = resource.GetAssetIndexes();
                    binaryWriter.Write7BitEncodedInt32(assetIndexes.Length);
                    foreach (var assetIndex in assetIndexes) binaryWriter.Write7BitEncodedInt32(assetIndex);
                }

                var resourceGroups = versionList.GetResourceGroups();
                binaryWriter.Write7BitEncodedInt32(resourceGroups.Length);
                foreach (var resourceGroup in resourceGroups)
                {
                    binaryWriter.WriteEncryptedString(resourceGroup.Name, s_CachedHashBytes);
                    var resourceIndexes = resourceGroup.GetResourceIndexes();
                    binaryWriter.Write7BitEncodedInt32(resourceIndexes.Length);
                    foreach (var resourceIndex in resourceIndexes) binaryWriter.Write7BitEncodedInt32(resourceIndex);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        ///     序列化可更新模式版本资源列表（版本 2）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的可更新模式版本资源列表（版本 2）。</param>
        /// <returns>是否序列化可更新模式版本资源列表（版本 2）成功。</returns>
        public static bool UpdatableVersionListSerializeCallback_V2(Stream stream, UpdatableVersionList versionList)
        {
            if (!versionList.IsValid) return false;

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                binaryWriter.WriteEncryptedString(versionList.ApplicableGameVersion, s_CachedHashBytes);
                binaryWriter.Write7BitEncodedInt32(versionList.InternalResourceVersion);
                var assets = versionList.GetAssets();
                binaryWriter.Write7BitEncodedInt32(assets.Length);
                foreach (var asset in assets)
                {
                    binaryWriter.WriteEncryptedString(asset.Name, s_CachedHashBytes);
                    var dependencyAssetIndexes = asset.GetDependencyAssetIndexes();
                    binaryWriter.Write7BitEncodedInt32(dependencyAssetIndexes.Length);
                    foreach (var dependencyAssetIndex in dependencyAssetIndexes)
                        binaryWriter.Write7BitEncodedInt32(dependencyAssetIndex);
                }

                var resources = versionList.GetResources();
                binaryWriter.Write7BitEncodedInt32(resources.Length);
                foreach (var resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(resource.Variant, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(
                        resource.Extension != DefaultExtension ? resource.Extension : null, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write7BitEncodedInt32(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                    binaryWriter.Write7BitEncodedInt32(resource.CompressedLength);
                    binaryWriter.Write(resource.CompressedHashCode);
                    var assetIndexes = resource.GetAssetIndexes();
                    binaryWriter.Write7BitEncodedInt32(assetIndexes.Length);
                    foreach (var assetIndex in assetIndexes) binaryWriter.Write7BitEncodedInt32(assetIndex);
                }

                var fileSystems = versionList.GetFileSystems();
                binaryWriter.Write7BitEncodedInt32(fileSystems.Length);
                foreach (var fileSystem in fileSystems)
                {
                    binaryWriter.WriteEncryptedString(fileSystem.Name, s_CachedHashBytes);
                    var resourceIndexes = fileSystem.GetResourceIndexes();
                    binaryWriter.Write7BitEncodedInt32(resourceIndexes.Length);
                    foreach (var resourceIndex in resourceIndexes) binaryWriter.Write7BitEncodedInt32(resourceIndex);
                }

                var resourceGroups = versionList.GetResourceGroups();
                binaryWriter.Write7BitEncodedInt32(resourceGroups.Length);
                foreach (var resourceGroup in resourceGroups)
                {
                    binaryWriter.WriteEncryptedString(resourceGroup.Name, s_CachedHashBytes);
                    var resourceIndexes = resourceGroup.GetResourceIndexes();
                    binaryWriter.Write7BitEncodedInt32(resourceIndexes.Length);
                    foreach (var resourceIndex in resourceIndexes) binaryWriter.Write7BitEncodedInt32(resourceIndex);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

#endif
    }
}