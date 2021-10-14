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
        /// <summary>
        ///     序列化本地版本资源列表（版本 0）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的本地版本资源列表（版本 0）。</param>
        /// <returns>是否序列化本地版本资源列表（版本 0）成功。</returns>
        public static bool LocalVersionListSerializeCallback_V0(Stream stream, LocalVersionList versionList)
        {
            if (!versionList.IsValid) return false;

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
                var resources = versionList.GetResources();
                binaryWriter.Write(resources.Length);
                foreach (var resource in resources)
                {
                    binaryWriter.WriteEncryptedString(resource.Name, s_CachedHashBytes);
                    binaryWriter.WriteEncryptedString(resource.Variant, s_CachedHashBytes);
                    binaryWriter.Write(resource.LoadType);
                    binaryWriter.Write(resource.Length);
                    binaryWriter.Write(resource.HashCode);
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        ///     序列化本地版本资源列表（版本 1）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的本地版本资源列表（版本 1）。</param>
        /// <returns>是否序列化本地版本资源列表（版本 1）成功。</returns>
        public static bool LocalVersionListSerializeCallback_V1(Stream stream, LocalVersionList versionList)
        {
            if (!versionList.IsValid) return false;

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
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
                }
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }

        /// <summary>
        ///     序列化本地版本资源列表（版本 2）回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="versionList">要序列化的本地版本资源列表（版本 2）。</param>
        /// <returns>是否序列化本地版本资源列表（版本 2）成功。</returns>
        public static bool LocalVersionListSerializeCallback_V2(Stream stream, LocalVersionList versionList)
        {
            if (!versionList.IsValid) return false;

            Utility.Random.GetRandomBytes(s_CachedHashBytes);
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8))
            {
                binaryWriter.Write(s_CachedHashBytes);
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
            }

            Array.Clear(s_CachedHashBytes, 0, CachedHashBytesLength);
            return true;
        }
    }
}