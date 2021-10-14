//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        /// <summary>
        ///     反序列化可更新模式版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的可更新模式版本资源列表（版本 0）。</returns>
        public static UpdatableVersionList UpdatableVersionListDeserializeCallback_V0(Stream stream)
        {
            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                var encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                var applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                var internalResourceVersion = binaryReader.ReadInt32();
                var assetCount = binaryReader.ReadInt32();
                var assets = assetCount > 0 ? new UpdatableVersionList.Asset[assetCount] : null;
                var resourceCount = binaryReader.ReadInt32();
                var resources = resourceCount > 0 ? new UpdatableVersionList.Resource[resourceCount] : null;
                var resourceToAssetNames = new string[resourceCount][];
                var assetNameToDependencyAssetNames = new List<KeyValuePair<string, string[]>>(assetCount);
                for (var i = 0; i < resourceCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var variant = binaryReader.ReadEncryptedString(encryptBytes);
                    var loadType = binaryReader.ReadByte();
                    var length = binaryReader.ReadInt32();
                    var hashCode = binaryReader.ReadInt32();
                    var compressedLength = binaryReader.ReadInt32();
                    var compressedHashCode = binaryReader.ReadInt32();
                    Utility.Converter.GetBytes(hashCode, s_CachedHashBytes);

                    var assetNameCount = binaryReader.ReadInt32();
                    var assetNames = assetNameCount > 0 ? new string[assetNameCount] : null;
                    for (var j = 0; j < assetNameCount; j++)
                    {
                        assetNames[j] = binaryReader.ReadEncryptedString(s_CachedHashBytes);
                        var dependencyAssetNameCount = binaryReader.ReadInt32();
                        var dependencyAssetNames =
                            dependencyAssetNameCount > 0 ? new string[dependencyAssetNameCount] : null;
                        for (var k = 0; k < dependencyAssetNameCount; k++)
                            dependencyAssetNames[k] = binaryReader.ReadEncryptedString(s_CachedHashBytes);

                        assetNameToDependencyAssetNames.Add(
                            new KeyValuePair<string, string[]>(assetNames[j], dependencyAssetNames));
                    }

                    resourceToAssetNames[i] = assetNames;
                    resources[i] = new UpdatableVersionList.Resource(name, variant, null, loadType, length, hashCode,
                        compressedLength, compressedHashCode, assetNameCount > 0 ? new int[assetNameCount] : null);
                }

                assetNameToDependencyAssetNames.Sort(AssetNameToDependencyAssetNamesComparer);
                Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
                var index = 0;
                foreach (var i in assetNameToDependencyAssetNames)
                    if (i.Value != null)
                    {
                        var dependencyAssetIndexes = new int[i.Value.Length];
                        for (var j = 0; j < i.Value.Length; j++)
                            dependencyAssetIndexes[j] = GetAssetNameIndex(assetNameToDependencyAssetNames, i.Value[j]);

                        assets[index++] = new UpdatableVersionList.Asset(i.Key, dependencyAssetIndexes);
                    }
                    else
                    {
                        assets[index++] = new UpdatableVersionList.Asset(i.Key, null);
                    }

                for (var i = 0; i < resources.Length; i++)
                {
                    var assetIndexes = resources[i].GetAssetIndexes();
                    for (var j = 0; j < assetIndexes.Length; j++)
                        assetIndexes[j] =
                            GetAssetNameIndex(assetNameToDependencyAssetNames, resourceToAssetNames[i][j]);
                }

                var resourceGroupCount = binaryReader.ReadInt32();
                var resourceGroups = resourceGroupCount > 0
                    ? new UpdatableVersionList.ResourceGroup[resourceGroupCount]
                    : null;
                for (var i = 0; i < resourceGroupCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var resourceIndexCount = binaryReader.ReadInt32();
                    var resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (var j = 0; j < resourceIndexCount; j++) resourceIndexes[j] = binaryReader.ReadUInt16();

                    resourceGroups[i] = new UpdatableVersionList.ResourceGroup(name, resourceIndexes);
                }

                return new UpdatableVersionList(applicableGameVersion, internalResourceVersion, assets, resources, null,
                    resourceGroups);
            }
        }

        /// <summary>
        ///     反序列化可更新模式版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的可更新模式版本资源列表（版本 1）。</returns>
        public static UpdatableVersionList UpdatableVersionListDeserializeCallback_V1(Stream stream)
        {
            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                var encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                var applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                var internalResourceVersion = binaryReader.Read7BitEncodedInt32();
                var assetCount = binaryReader.Read7BitEncodedInt32();
                var assets = assetCount > 0 ? new UpdatableVersionList.Asset[assetCount] : null;
                for (var i = 0; i < assetCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var dependencyAssetCount = binaryReader.Read7BitEncodedInt32();
                    var dependencyAssetIndexes = dependencyAssetCount > 0 ? new int[dependencyAssetCount] : null;
                    for (var j = 0; j < dependencyAssetCount; j++)
                        dependencyAssetIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    assets[i] = new UpdatableVersionList.Asset(name, dependencyAssetIndexes);
                }

                var resourceCount = binaryReader.Read7BitEncodedInt32();
                var resources = resourceCount > 0 ? new UpdatableVersionList.Resource[resourceCount] : null;
                for (var i = 0; i < resourceCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var variant = binaryReader.ReadEncryptedString(encryptBytes);
                    var extension = binaryReader.ReadEncryptedString(encryptBytes) ?? DefaultExtension;
                    var loadType = binaryReader.ReadByte();
                    var length = binaryReader.Read7BitEncodedInt32();
                    var hashCode = binaryReader.ReadInt32();
                    var compressedLength = binaryReader.Read7BitEncodedInt32();
                    var compressedHashCode = binaryReader.ReadInt32();
                    var assetIndexCount = binaryReader.Read7BitEncodedInt32();
                    var assetIndexes = assetIndexCount > 0 ? new int[assetIndexCount] : null;
                    for (var j = 0; j < assetIndexCount; j++) assetIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    resources[i] = new UpdatableVersionList.Resource(name, variant, extension, loadType, length,
                        hashCode, compressedLength, compressedHashCode, assetIndexes);
                }

                var resourceGroupCount = binaryReader.Read7BitEncodedInt32();
                var resourceGroups = resourceGroupCount > 0
                    ? new UpdatableVersionList.ResourceGroup[resourceGroupCount]
                    : null;
                for (var i = 0; i < resourceGroupCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                    var resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (var j = 0; j < resourceIndexCount; j++)
                        resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    resourceGroups[i] = new UpdatableVersionList.ResourceGroup(name, resourceIndexes);
                }

                return new UpdatableVersionList(applicableGameVersion, internalResourceVersion, assets, resources, null,
                    resourceGroups);
            }
        }

        /// <summary>
        ///     反序列化可更新模式版本资源列表（版本 2）回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的可更新模式版本资源列表（版本 2）。</returns>
        public static UpdatableVersionList UpdatableVersionListDeserializeCallback_V2(Stream stream)
        {
            using (var binaryReader = new BinaryReader(stream, Encoding.UTF8))
            {
                var encryptBytes = binaryReader.ReadBytes(CachedHashBytesLength);
                var applicableGameVersion = binaryReader.ReadEncryptedString(encryptBytes);
                var internalResourceVersion = binaryReader.Read7BitEncodedInt32();
                var assetCount = binaryReader.Read7BitEncodedInt32();
                var assets = assetCount > 0 ? new UpdatableVersionList.Asset[assetCount] : null;
                for (var i = 0; i < assetCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var dependencyAssetCount = binaryReader.Read7BitEncodedInt32();
                    var dependencyAssetIndexes = dependencyAssetCount > 0 ? new int[dependencyAssetCount] : null;
                    for (var j = 0; j < dependencyAssetCount; j++)
                        dependencyAssetIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    assets[i] = new UpdatableVersionList.Asset(name, dependencyAssetIndexes);
                }

                var resourceCount = binaryReader.Read7BitEncodedInt32();
                var resources = resourceCount > 0 ? new UpdatableVersionList.Resource[resourceCount] : null;
                for (var i = 0; i < resourceCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var variant = binaryReader.ReadEncryptedString(encryptBytes);
                    var extension = binaryReader.ReadEncryptedString(encryptBytes) ?? DefaultExtension;
                    var loadType = binaryReader.ReadByte();
                    var length = binaryReader.Read7BitEncodedInt32();
                    var hashCode = binaryReader.ReadInt32();
                    var compressedLength = binaryReader.Read7BitEncodedInt32();
                    var compressedHashCode = binaryReader.ReadInt32();
                    var assetIndexCount = binaryReader.Read7BitEncodedInt32();
                    var assetIndexes = assetIndexCount > 0 ? new int[assetIndexCount] : null;
                    for (var j = 0; j < assetIndexCount; j++) assetIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    resources[i] = new UpdatableVersionList.Resource(name, variant, extension, loadType, length,
                        hashCode, compressedLength, compressedHashCode, assetIndexes);
                }

                var fileSystemCount = binaryReader.Read7BitEncodedInt32();
                var fileSystems = fileSystemCount > 0 ? new UpdatableVersionList.FileSystem[fileSystemCount] : null;
                for (var i = 0; i < fileSystemCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                    var resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (var j = 0; j < resourceIndexCount; j++)
                        resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    fileSystems[i] = new UpdatableVersionList.FileSystem(name, resourceIndexes);
                }

                var resourceGroupCount = binaryReader.Read7BitEncodedInt32();
                var resourceGroups = resourceGroupCount > 0
                    ? new UpdatableVersionList.ResourceGroup[resourceGroupCount]
                    : null;
                for (var i = 0; i < resourceGroupCount; i++)
                {
                    var name = binaryReader.ReadEncryptedString(encryptBytes);
                    var resourceIndexCount = binaryReader.Read7BitEncodedInt32();
                    var resourceIndexes = resourceIndexCount > 0 ? new int[resourceIndexCount] : null;
                    for (var j = 0; j < resourceIndexCount; j++)
                        resourceIndexes[j] = binaryReader.Read7BitEncodedInt32();

                    resourceGroups[i] = new UpdatableVersionList.ResourceGroup(name, resourceIndexes);
                }

                return new UpdatableVersionList(applicableGameVersion, internalResourceVersion, assets, resources,
                    fileSystems, resourceGroups);
            }
        }
    }
}