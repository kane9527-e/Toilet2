﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using GameFramework;
using GameFramework.Download;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using FileInfo = System.IO.FileInfo;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    ///     编辑器资源组件。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EditorResourceComponent : MonoBehaviour, IResourceManager
    {
        private const int DefaultPriority = 0;
        private static readonly int AssetsStringLength = "Assets".Length;

        [SerializeField] private bool m_EnableCachedAssets = true;

        [SerializeField] private int m_LoadAssetCountPerFrame = 1;

        [SerializeField] private float m_MinLoadAssetRandomDelaySeconds;

        [SerializeField] private float m_MaxLoadAssetRandomDelaySeconds;

        private Dictionary<string, Object> m_CachedAssets;
        private GameFrameworkLinkedList<LoadAssetInfo> m_LoadAssetInfos;
        private GameFrameworkLinkedList<LoadSceneInfo> m_LoadSceneInfos;

        private GameFrameworkLinkedList<UnloadSceneInfo> m_UnloadSceneInfos;

        /// <summary>
        ///     获取等待编辑器加载的资源数量。
        /// </summary>
        public int LoadWaitingAssetCount => m_LoadAssetInfos.Count;

        private void Awake()
        {
            ReadOnlyPath = null;
            ReadWritePath = null;
            m_CachedAssets = new Dictionary<string, Object>(StringComparer.Ordinal);
            m_LoadAssetInfos = new GameFrameworkLinkedList<LoadAssetInfo>();
            m_LoadSceneInfos = new GameFrameworkLinkedList<LoadSceneInfo>();
            m_UnloadSceneInfos = new GameFrameworkLinkedList<UnloadSceneInfo>();

            var baseComponent = GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Error("Can not find base component.");
                return;
            }

            if (baseComponent.EditorResourceMode)
            {
                baseComponent.EditorResourceHelper = this;
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (m_LoadAssetInfos.Count > 0)
            {
                var count = 0;
                var current = m_LoadAssetInfos.First;
                while (current != null && count < m_LoadAssetCountPerFrame)
                {
                    var loadAssetInfo = current.Value;
                    var elapseSeconds = (float)(DateTime.UtcNow - loadAssetInfo.StartTime).TotalSeconds;
                    if (elapseSeconds >= loadAssetInfo.DelaySeconds)
                    {
                        var asset = GetCachedAsset(loadAssetInfo.AssetName);
                        if (asset == null)
                        {
#if UNITY_EDITOR
                            if (loadAssetInfo.AssetType != null)
                                asset = AssetDatabase.LoadAssetAtPath(loadAssetInfo.AssetName, loadAssetInfo.AssetType);
                            else
                                asset = AssetDatabase.LoadMainAssetAtPath(loadAssetInfo.AssetName);

                            if (m_EnableCachedAssets && asset != null)
                                m_CachedAssets.Add(loadAssetInfo.AssetName, asset);
#endif
                        }

                        if (asset != null)
                        {
                            if (loadAssetInfo.LoadAssetCallbacks.LoadAssetSuccessCallback != null)
                                loadAssetInfo.LoadAssetCallbacks.LoadAssetSuccessCallback(loadAssetInfo.AssetName,
                                    asset, elapseSeconds, loadAssetInfo.UserData);
                        }
                        else
                        {
                            if (loadAssetInfo.LoadAssetCallbacks.LoadAssetFailureCallback != null)
                                loadAssetInfo.LoadAssetCallbacks.LoadAssetFailureCallback(loadAssetInfo.AssetName,
                                    LoadResourceStatus.AssetError, "Can not load this asset from asset database.",
                                    loadAssetInfo.UserData);
                        }

                        var next = current.Next;
                        m_LoadAssetInfos.Remove(loadAssetInfo);
                        current = next;
                        count++;
                    }
                    else
                    {
                        if (loadAssetInfo.LoadAssetCallbacks.LoadAssetUpdateCallback != null)
                            loadAssetInfo.LoadAssetCallbacks.LoadAssetUpdateCallback(loadAssetInfo.AssetName,
                                elapseSeconds / loadAssetInfo.DelaySeconds, loadAssetInfo.UserData);

                        current = current.Next;
                    }
                }
            }

            if (m_LoadSceneInfos.Count > 0)
            {
                var current = m_LoadSceneInfos.First;
                while (current != null)
                {
                    var loadSceneInfo = current.Value;
                    if (loadSceneInfo.AsyncOperation.isDone)
                    {
                        if (loadSceneInfo.AsyncOperation.allowSceneActivation)
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadSceneSuccessCallback != null)
                                loadSceneInfo.LoadSceneCallbacks.LoadSceneSuccessCallback(loadSceneInfo.SceneAssetName,
                                    (float)(DateTime.UtcNow - loadSceneInfo.StartTime).TotalSeconds,
                                    loadSceneInfo.UserData);
                        }
                        else
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadSceneFailureCallback != null)
                                loadSceneInfo.LoadSceneCallbacks.LoadSceneFailureCallback(loadSceneInfo.SceneAssetName,
                                    LoadResourceStatus.AssetError, "Can not load this scene from asset database.",
                                    loadSceneInfo.UserData);
                        }

                        var next = current.Next;
                        m_LoadSceneInfos.Remove(loadSceneInfo);
                        current = next;
                    }
                    else
                    {
                        if (loadSceneInfo.LoadSceneCallbacks.LoadSceneUpdateCallback != null)
                            loadSceneInfo.LoadSceneCallbacks.LoadSceneUpdateCallback(loadSceneInfo.SceneAssetName,
                                loadSceneInfo.AsyncOperation.progress, loadSceneInfo.UserData);

                        current = current.Next;
                    }
                }
            }

            if (m_UnloadSceneInfos.Count > 0)
            {
                var current = m_UnloadSceneInfos.First;
                while (current != null)
                {
                    var unloadSceneInfo = current.Value;
                    if (unloadSceneInfo.AsyncOperation.isDone)
                    {
                        if (unloadSceneInfo.AsyncOperation.allowSceneActivation)
                        {
                            if (unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                                unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneSuccessCallback(
                                    unloadSceneInfo.SceneAssetName, unloadSceneInfo.UserData);
                        }
                        else
                        {
                            if (unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneFailureCallback != null)
                                unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneFailureCallback(
                                    unloadSceneInfo.SceneAssetName, unloadSceneInfo.UserData);
                        }

                        var next = current.Next;
                        m_UnloadSceneInfos.Remove(unloadSceneInfo);
                        current = next;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
            }
        }

        /// <summary>
        ///     获取资源只读区路径。
        /// </summary>
        public string ReadOnlyPath { get; private set; }

        /// <summary>
        ///     获取资源读写区路径。
        /// </summary>
        public string ReadWritePath { get; private set; }

        /// <summary>
        ///     获取资源模式。
        /// </summary>
        public ResourceMode ResourceMode => ResourceMode.Unspecified;

        /// <summary>
        ///     获取当前变体。
        /// </summary>
        public string CurrentVariant => null;

        /// <summary>
        ///     获取单机模式版本资源列表序列化器。
        /// </summary>
        public PackageVersionListSerializer PackageVersionListSerializer =>
            throw new NotSupportedException("ReadWriteVersionListSerializer");

        /// <summary>
        ///     获取可更新模式版本资源列表序列化器。
        /// </summary>
        public UpdatableVersionListSerializer UpdatableVersionListSerializer =>
            throw new NotSupportedException("ReadWriteVersionListSerializer");

        /// <summary>
        ///     获取本地只读区版本资源列表序列化器。
        /// </summary>
        public ReadOnlyVersionListSerializer ReadOnlyVersionListSerializer =>
            throw new NotSupportedException("ReadWriteVersionListSerializer");

        /// <summary>
        ///     获取本地读写区版本资源列表序列化器。
        /// </summary>
        public ReadWriteVersionListSerializer ReadWriteVersionListSerializer =>
            throw new NotSupportedException("ReadWriteVersionListSerializer");

        /// <summary>
        ///     获取资源包版本资源列表序列化器。
        /// </summary>
        public ResourcePackVersionListSerializer ResourcePackVersionListSerializer =>
            throw new NotSupportedException("ResourcePackVersionListSerializer");

        /// <summary>
        ///     获取当前资源适用的游戏版本号。
        /// </summary>
        public string ApplicableGameVersion => throw new NotSupportedException("ApplicableGameVersion");

        /// <summary>
        ///     获取当前内部资源版本号。
        /// </summary>
        public int InternalResourceVersion => throw new NotSupportedException("InternalResourceVersion");

        /// <summary>
        ///     获取已准备完毕资源数量。
        /// </summary>
        public int AssetCount => throw new NotSupportedException("AssetCount");

        /// <summary>
        ///     获取已准备完毕资源数量。
        /// </summary>
        public int ResourceCount => throw new NotSupportedException("ResourceCount");

        /// <summary>
        ///     获取资源组个数。
        /// </summary>
        public int ResourceGroupCount => throw new NotSupportedException("ResourceGroupCount");

        /// <summary>
        ///     获取或设置资源更新下载地址。
        /// </summary>
        public string UpdatePrefixUri
        {
            get => throw new NotSupportedException("UpdatePrefixUri");
            set => throw new NotSupportedException("UpdatePrefixUri");
        }

        /// <summary>
        ///     获取或设置每更新多少字节的资源，重新生成一次版本资源列表。
        /// </summary>
        public int GenerateReadWriteVersionListLength
        {
            get => throw new NotSupportedException("GenerateReadWriteVersionListLength");
            set => throw new NotSupportedException("GenerateReadWriteVersionListLength");
        }

        /// <summary>
        ///     获取正在应用的资源包路径。
        /// </summary>
        public string ApplyingResourcePackPath => throw new NotSupportedException("ApplyingResourcePackPath");

        /// <summary>
        ///     获取等待应用资源数量。
        /// </summary>
        public int ApplyWaitingCount => throw new NotSupportedException("ApplyWaitingCount");

        /// <summary>
        ///     获取或设置资源更新重试次数。
        /// </summary>
        public int UpdateRetryCount
        {
            get => throw new NotSupportedException("UpdateRetryCount");
            set => throw new NotSupportedException("UpdateRetryCount");
        }

        /// <summary>
        ///     获取正在更新的资源组。
        /// </summary>
        public IResourceGroup UpdatingResourceGroup => throw new NotSupportedException("UpdatingResourceGroup");

        /// <summary>
        ///     获取等待更新资源个数。
        /// </summary>
        public int UpdateWaitingCount => throw new NotSupportedException("UpdateWaitingCount");

        /// <summary>
        ///     获取使用时下载的等待更新资源数量。
        /// </summary>
        public int UpdateWaitingWhilePlayingCount => throw new NotSupportedException("UpdateWaitingWhilePlayingCount");

        /// <summary>
        ///     获取候选更新资源数量。
        /// </summary>
        public int UpdateCandidateCount => throw new NotSupportedException("UpdateCandidateCount");

        /// <summary>
        ///     获取加载资源代理总个数。
        /// </summary>
        public int LoadTotalAgentCount => throw new NotSupportedException("LoadTotalAgentCount");

        /// <summary>
        ///     获取可用加载资源代理个数。
        /// </summary>
        public int LoadFreeAgentCount => throw new NotSupportedException("LoadFreeAgentCount");

        /// <summary>
        ///     获取工作中加载资源代理个数。
        /// </summary>
        public int LoadWorkingAgentCount => throw new NotSupportedException("LoadWorkingAgentCount");

        /// <summary>
        ///     获取等待加载资源任务个数。
        /// </summary>
        public int LoadWaitingTaskCount => throw new NotSupportedException("LoadWaitingTaskCount");

        /// <summary>
        ///     获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetAutoReleaseInterval
        {
            get => throw new NotSupportedException("AssetAutoReleaseInterval");
            set => throw new NotSupportedException("AssetAutoReleaseInterval");
        }

        /// <summary>
        ///     获取或设置资源对象池的容量。
        /// </summary>
        public int AssetCapacity
        {
            get => throw new NotSupportedException("AssetCapacity");
            set => throw new NotSupportedException("AssetCapacity");
        }

        /// <summary>
        ///     获取或设置资源对象池对象过期秒数。
        /// </summary>
        public float AssetExpireTime
        {
            get => throw new NotSupportedException("AssetExpireTime");
            set => throw new NotSupportedException("AssetExpireTime");
        }

        /// <summary>
        ///     获取或设置资源对象池的优先级。
        /// </summary>
        public int AssetPriority
        {
            get => throw new NotSupportedException("AssetPriority");
            set => throw new NotSupportedException("AssetPriority");
        }

        /// <summary>
        ///     获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float ResourceAutoReleaseInterval
        {
            get => throw new NotSupportedException("ResourceAutoReleaseInterval");
            set => throw new NotSupportedException("ResourceAutoReleaseInterval");
        }

        /// <summary>
        ///     获取或设置资源对象池的容量。
        /// </summary>
        public int ResourceCapacity
        {
            get => throw new NotSupportedException("ResourceCapacity");
            set => throw new NotSupportedException("ResourceCapacity");
        }

        /// <summary>
        ///     获取或设置资源对象池对象过期秒数。
        /// </summary>
        public float ResourceExpireTime
        {
            get => throw new NotSupportedException("ResourceExpireTime");
            set => throw new NotSupportedException("ResourceExpireTime");
        }

        /// <summary>
        ///     获取或设置资源对象池的优先级。
        /// </summary>
        public int ResourcePriority
        {
            get => throw new NotSupportedException("ResourcePriority");
            set => throw new NotSupportedException("ResourcePriority");
        }

        /// <summary>
        ///     设置资源只读区路径。
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径。</param>
        public void SetReadOnlyPath(string readOnlyPath)
        {
            if (string.IsNullOrEmpty(readOnlyPath))
            {
                Log.Error("Read-only path is invalid.");
                return;
            }

            ReadOnlyPath = readOnlyPath;
        }

        /// <summary>
        ///     设置资源读写区路径。
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        public void SetReadWritePath(string readWritePath)
        {
            if (string.IsNullOrEmpty(readWritePath))
            {
                Log.Error("Read-write path is invalid.");
                return;
            }

            ReadWritePath = readWritePath;
        }

        /// <summary>
        ///     设置资源模式。
        /// </summary>
        /// <param name="resourceMode">资源模式。</param>
        public void SetResourceMode(ResourceMode resourceMode)
        {
            throw new NotSupportedException("SetResourceMode");
        }

        /// <summary>
        ///     设置当前变体。
        /// </summary>
        /// <param name="currentVariant">当前变体。</param>
        public void SetCurrentVariant(string currentVariant)
        {
            throw new NotSupportedException("SetCurrentVariant");
        }

        /// <summary>
        ///     设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            throw new NotSupportedException("SetObjectPoolManager");
        }

        /// <summary>
        ///     设置文件系统管理器。
        /// </summary>
        /// <param name="fileSystemManager">文件系统管理器。</param>
        public void SetFileSystemManager(IFileSystemManager fileSystemManager)
        {
            throw new NotSupportedException("SetFileSystemManager");
        }

        /// <summary>
        ///     设置下载管理器。
        /// </summary>
        /// <param name="downloadManager">下载管理器。</param>
        public void SetDownloadManager(IDownloadManager downloadManager)
        {
            throw new NotSupportedException("SetDownloadManager");
        }

        /// <summary>
        ///     设置解密资源回调函数。
        /// </summary>
        /// <param name="decryptResourceCallback">要设置的解密资源回调函数。</param>
        /// <remarks>如果不设置，将使用默认的解密资源回调函数。</remarks>
        public void SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)
        {
            throw new NotSupportedException("SetDecryptResourceCallback");
        }

        /// <summary>
        ///     设置资源辅助器。
        /// </summary>
        /// <param name="resourceHelper">资源辅助器。</param>
        public void SetResourceHelper(IResourceHelper resourceHelper)
        {
            throw new NotSupportedException("SetResourceHelper");
        }

        /// <summary>
        ///     增加加载资源代理辅助器。
        /// </summary>
        /// <param name="loadResourceAgentHelper">要增加的加载资源代理辅助器。</param>
        public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper)
        {
            throw new NotSupportedException("AddLoadResourceAgentHelper");
        }

        /// <summary>
        ///     使用单机模式并初始化资源。
        /// </summary>
        /// <param name="initResourcesCompleteCallback">使用单机模式并初始化资源完成时的回调函数。</param>
        public void InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)
        {
            throw new NotSupportedException("InitResources");
        }

        /// <summary>
        ///     检查版本资源列表。
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号。</param>
        /// <returns>检查版本资源列表结果。</returns>
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {
            throw new NotSupportedException("CheckVersionList");
        }

        /// <summary>
        ///     使用可更新模式并更新版本资源列表。
        /// </summary>
        /// <param name="versionListLength">版本资源列表大小。</param>
        /// <param name="versionListHashCode">版本资源列表哈希值。</param>
        /// <param name="versionListCompressedLength">版本资源列表压缩后大小。</param>
        /// <param name="versionListCompressedHashCode">版本资源列表压缩后哈希值。</param>
        /// <param name="updateVersionListCallbacks">版本资源列表更新回调函数集。</param>
        public void UpdateVersionList(int versionListLength, int versionListHashCode, int versionListCompressedLength,
            int versionListCompressedHashCode, UpdateVersionListCallbacks updateVersionListCallbacks)
        {
            throw new NotSupportedException("UpdateVersionList");
        }

        /// <summary>
        ///     使用可更新模式并检查资源。
        /// </summary>
        /// <param name="ignoreOtherVariant">是否忽略处理其它变体的资源，若不忽略，将会移除其它变体的资源。</param>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成时的回调函数。</param>
        public void CheckResources(bool ignoreOtherVariant,
            CheckResourcesCompleteCallback checkResourcesCompleteCallback)
        {
            throw new NotSupportedException("CheckResources");
        }

        /// <summary>
        ///     使用可更新模式并应用资源包资源。
        /// </summary>
        /// <param name="resourcePackPath">要应用的资源包路径。</param>
        /// <param name="applyResourcesCompleteCallback">使用可更新模式并应用资源包资源完成时的回调函数。</param>
        public void ApplyResources(string resourcePackPath,
            ApplyResourcesCompleteCallback applyResourcesCompleteCallback)
        {
            throw new NotSupportedException("ApplyResources");
        }

        /// <summary>
        ///     使用可更新模式并更新所有资源。
        /// </summary>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新默认资源组完成时的回调函数。</param>
        public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            throw new NotSupportedException("UpdateResources");
        }

        /// <summary>
        ///     使用可更新模式并更新指定资源组的资源。
        /// </summary>
        /// <param name="resourceGroupName">要更新的资源组名称。</param>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新指定资源组完成时的回调函数。</param>
        public void UpdateResources(string resourceGroupName,
            UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            throw new NotSupportedException("UpdateResources");
        }

        /// <summary>
        ///     停止更新资源。
        /// </summary>
        public void StopUpdateResources()
        {
            throw new NotSupportedException("StopUpdateResources");
        }

        /// <summary>
        ///     校验资源包。
        /// </summary>
        /// <param name="resourcePackPath">要校验的资源包路径。</param>
        /// <returns>是否校验资源包成功。</returns>
        public bool VerifyResourcePack(string resourcePackPath)
        {
            throw new NotSupportedException("VerifyResourcePack");
        }

        /// <summary>
        ///     获取所有加载资源任务的信息。
        /// </summary>
        /// <returns>所有加载资源任务的信息。</returns>
        public TaskInfo[] GetAllLoadAssetInfos()
        {
            throw new NotSupportedException("GetAllLoadAssetInfos");
        }

        /// <summary>
        ///     获取所有加载资源任务的信息。
        /// </summary>
        /// <param name="results">所有加载资源任务的信息。</param>
        public void GetAllLoadAssetInfos(List<TaskInfo> results)
        {
            throw new NotSupportedException("GetAllLoadAssetInfos");
        }

        /// <summary>
        ///     检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasAssetResult HasAsset(string assetName)
        {
#if UNITY_EDITOR
            var obj = AssetDatabase.LoadMainAssetAtPath(assetName);
            if (obj == null) return HasAssetResult.NotExist;

            var result = obj.GetType() == typeof(DefaultAsset)
                ? HasAssetResult.BinaryOnDisk
                : HasAssetResult.AssetOnDisk;
            obj = null;
            EditorUtility.UnloadUnusedAssetsImmediate();
            return result;
#else
            return HasAssetResult.NotExist;
#endif
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, null, DefaultPriority, loadAssetCallbacks, null);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, assetType, DefaultPriority, loadAssetCallbacks, null);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, null);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, null, DefaultPriority, loadAssetCallbacks, userData);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, assetType, priority, loadAssetCallbacks, null);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, assetType, DefaultPriority, loadAssetCallbacks, userData);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        ///     异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks,
            object userData)
        {
            if (loadAssetCallbacks == null)
            {
                Log.Error("Load asset callbacks is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(assetName))
            {
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResourceStatus.NotExist,
                        "Asset name is invalid.", userData);

                return;
            }

            if (!assetName.StartsWith("Assets/", StringComparison.Ordinal))
            {
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResourceStatus.NotExist,
                        Utility.Text.Format("Asset name '{0}' is invalid.", assetName), userData);

                return;
            }

            if (!HasFile(assetName))
            {
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, LoadResourceStatus.NotExist,
                        Utility.Text.Format("Asset '{0}' is not exist.", assetName), userData);

                return;
            }

            m_LoadAssetInfos.AddLast(new LoadAssetInfo(assetName, assetType, priority, DateTime.UtcNow,
                m_MinLoadAssetRandomDelaySeconds + (float)Utility.Random.GetRandomDouble() *
                (m_MaxLoadAssetRandomDelaySeconds - m_MinLoadAssetRandomDelaySeconds), loadAssetCallbacks, userData));
        }

        /// <summary>
        ///     卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public void UnloadAsset(object asset)
        {
            // Do nothing in editor resource mode.
        }

        /// <summary>
        ///     异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks)
        {
            LoadScene(sceneAssetName, DefaultPriority, loadSceneCallbacks, null);
        }

        /// <summary>
        ///     异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks)
        {
            LoadScene(sceneAssetName, priority, loadSceneCallbacks, null);
        }

        /// <summary>
        ///     异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, object userData)
        {
            LoadScene(sceneAssetName, DefaultPriority, loadSceneCallbacks, userData);
        }

        /// <summary>
        ///     异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks,
            object userData)
        {
            if (loadSceneCallbacks == null)
            {
                Log.Error("Load scene callbacks is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(sceneAssetName))
            {
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResourceStatus.NotExist,
                        "Scene asset name is invalid.", userData);

                return;
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) ||
                !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResourceStatus.NotExist,
                        Utility.Text.Format("Scene asset name '{0}' is invalid.", sceneAssetName), userData);

                return;
            }

            if (!HasFile(sceneAssetName))
            {
                if (loadSceneCallbacks.LoadSceneFailureCallback != null)
                    loadSceneCallbacks.LoadSceneFailureCallback(sceneAssetName, LoadResourceStatus.NotExist,
                        Utility.Text.Format("Scene '{0}' is not exist.", sceneAssetName), userData);

                return;
            }

#if UNITY_5_5_OR_NEWER
            var asyncOperation = SceneManager.LoadSceneAsync(sceneAssetName, LoadSceneMode.Additive);
#else
            AsyncOperation asyncOperation =
 SceneManager.LoadSceneAsync(SceneComponent.GetSceneName(sceneAssetName), LoadSceneMode.Additive);
#endif
            if (asyncOperation == null) return;

            m_LoadSceneInfos.AddLast(new LoadSceneInfo(asyncOperation, sceneAssetName, priority, DateTime.UtcNow,
                loadSceneCallbacks, userData));
        }

        /// <summary>
        ///     异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            UnloadScene(sceneAssetName, unloadSceneCallbacks, null);
        }

        /// <summary>
        ///     异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/", StringComparison.Ordinal) ||
                !sceneAssetName.EndsWith(".unity", StringComparison.Ordinal))
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }

            if (unloadSceneCallbacks == null)
            {
                Log.Error("Unload scene callbacks is invalid.");
                return;
            }

            if (!HasFile(sceneAssetName))
            {
                Log.Error("Scene '{0}' is not exist.", sceneAssetName);
                return;
            }

#if UNITY_5_5_OR_NEWER
            var asyncOperation = SceneManager.UnloadSceneAsync(sceneAssetName);
            if (asyncOperation == null) return;

            m_UnloadSceneInfos.AddLast(new UnloadSceneInfo(asyncOperation, sceneAssetName, unloadSceneCallbacks,
                userData));
#else
            if (SceneManager.UnloadScene(SceneComponent.GetSceneName(sceneAssetName)))
            {
                if (unloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneSuccessCallback(sceneAssetName, userData);
                }
            }
            else
            {
                if (unloadSceneCallbacks.UnloadSceneFailureCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName, userData);
                }
            }
#endif
        }

        /// <summary>
        ///     获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        public string GetBinaryPath(string binaryAssetName)
        {
            if (!HasFile(binaryAssetName)) return null;

            return Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) +
                   binaryAssetName;
        }

        /// <summary>
        ///     获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <param name="storageInReadOnly">二进制资源是否存储在只读区中。</param>
        /// <param name="storageInFileSystem">二进制资源是否存储在文件系统中。</param>
        /// <param name="relativePath">二进制资源或存储二进制资源的文件系统，相对于只读区或者读写区的相对路径。</param>
        /// <param name="fileName">若二进制资源存储在文件系统中，则指示二进制资源在文件系统中的名称，否则此参数返回空。</param>
        /// <returns>是否获取二进制资源的实际路径成功。</returns>
        public bool GetBinaryPath(string binaryAssetName, out bool storageInReadOnly, out bool storageInFileSystem,
            out string relativePath, out string fileName)
        {
            throw new NotSupportedException("GetBinaryPath");
        }

        /// <summary>
        ///     获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        public int GetBinaryLength(string binaryAssetName)
        {
            var binaryPath = GetBinaryPath(binaryAssetName);
            if (string.IsNullOrEmpty(binaryPath)) return -1;

            return (int)new FileInfo(binaryPath).Length;
        }

        /// <summary>
        ///     异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            LoadBinary(binaryAssetName, loadBinaryCallbacks, null);
        }

        /// <summary>
        ///     异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks, object userData)
        {
            if (loadBinaryCallbacks == null)
            {
                Log.Error("Load binary callbacks is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(binaryAssetName))
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResourceStatus.NotExist,
                        "Binary asset name is invalid.", userData);

                return;
            }

            if (!binaryAssetName.StartsWith("Assets/", StringComparison.Ordinal))
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResourceStatus.NotExist,
                        Utility.Text.Format("Binary asset name '{0}' is invalid.", binaryAssetName), userData);

                return;
            }

            var binaryPath = GetBinaryPath(binaryAssetName);
            if (binaryPath == null)
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResourceStatus.NotExist,
                        Utility.Text.Format("Binary asset '{0}' is not exist.", binaryAssetName), userData);

                return;
            }

            try
            {
                var binaryBytes = File.ReadAllBytes(binaryPath);
                loadBinaryCallbacks.LoadBinarySuccessCallback(binaryAssetName, binaryBytes, 0f, userData);
            }
            catch (Exception exception)
            {
                if (loadBinaryCallbacks.LoadBinaryFailureCallback != null)
                    loadBinaryCallbacks.LoadBinaryFailureCallback(binaryAssetName, LoadResourceStatus.AssetError,
                        exception.ToString(), userData);
            }
        }

        /// <summary>
        ///     从文件系统中加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <returns>存储加载二进制资源的二进制流。</returns>
        public byte[] LoadBinaryFromFileSystem(string binaryAssetName)
        {
            throw new NotSupportedException("LoadBinaryFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="buffer">存储加载二进制资源的二进制流。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer)
        {
            throw new NotSupportedException("LoadBinaryFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="buffer">存储加载二进制资源的二进制流。</param>
        /// <param name="startIndex">存储加载二进制资源的二进制流的起始位置。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex)
        {
            throw new NotSupportedException("LoadBinaryFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="buffer">存储加载二进制资源的二进制流。</param>
        /// <param name="startIndex">存储加载二进制资源的二进制流的起始位置。</param>
        /// <param name="length">存储加载二进制资源的二进制流的长度。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex, int length)
        {
            throw new NotSupportedException("LoadBinaryFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="length">要加载片段的长度。</param>
        /// <returns>存储加载二进制资源片段内容的二进制流。</returns>
        public byte[] LoadBinarySegmentFromFileSystem(string binaryAssetName, int length)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="offset">要加载片段的偏移。</param>
        /// <param name="length">要加载片段的长度。</param>
        /// <returns>存储加载二进制资源片段内容的二进制流。</returns>
        public byte[] LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, int length)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="buffer">存储加载二进制资源片段内容的二进制流。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="buffer">存储加载二进制资源片段内容的二进制流。</param>
        /// <param name="length">要加载片段的长度。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer, int length)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="buffer">存储加载二进制资源片段内容的二进制流。</param>
        /// <param name="startIndex">存储加载二进制资源片段内容的二进制流的起始位置。</param>
        /// <param name="length">要加载片段的长度。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex, int length)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="offset">要加载片段的偏移。</param>
        /// <param name="buffer">存储加载二进制资源片段内容的二进制流。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="offset">要加载片段的偏移。</param>
        /// <param name="buffer">存储加载二进制资源片段内容的二进制流。</param>
        /// <param name="length">要加载片段的长度。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer, int length)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     从文件系统中加载二进制资源的片段。
        /// </summary>
        /// <param name="binaryAssetName">要加载片段的二进制资源的名称。</param>
        /// <param name="offset">要加载片段的偏移。</param>
        /// <param name="buffer">存储加载二进制资源片段内容的二进制流。</param>
        /// <param name="startIndex">存储加载二进制资源片段内容的二进制流的起始位置。</param>
        /// <param name="length">要加载片段的长度。</param>
        /// <returns>实际加载了多少字节。</returns>
        public int LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer, int startIndex,
            int length)
        {
            throw new NotSupportedException("LoadBinarySegmentFromFileSystem");
        }

        /// <summary>
        ///     检查资源组是否存在。
        /// </summary>
        /// <param name="resourceGroupName">要检查资源组的名称。</param>
        /// <returns>资源组是否存在。</returns>
        public bool HasResourceGroup(string resourceGroupName)
        {
            throw new NotSupportedException("HasResourceGroup");
        }

        /// <summary>
        ///     获取默认资源组。
        /// </summary>
        /// <returns>默认资源组。</returns>
        public IResourceGroup GetResourceGroup()
        {
            throw new NotSupportedException("GetResourceGroup");
        }

        /// <summary>
        ///     获取资源组。
        /// </summary>
        /// <param name="resourceGroupName">要获取的资源组名称。</param>
        /// <returns>要获取的资源组。</returns>
        public IResourceGroup GetResourceGroup(string resourceGroupName)
        {
            throw new NotSupportedException("GetResourceGroup");
        }

        /// <summary>
        ///     获取所有资源组。
        /// </summary>
        /// <returns>所有资源组。</returns>
        public IResourceGroup[] GetAllResourceGroups()
        {
            throw new NotSupportedException("GetAllResourceGroups");
        }

        /// <summary>
        ///     获取所有资源组。
        /// </summary>
        /// <param name="results">所有资源组。</param>
        public void GetAllResourceGroups(List<IResourceGroup> results)
        {
            throw new NotSupportedException("GetAllResourceGroups");
        }

        /// <summary>
        ///     获取资源组集合。
        /// </summary>
        /// <param name="resourceGroupNames">要获取的资源组名称的集合。</param>
        /// <returns>要获取的资源组集合。</returns>
        public IResourceGroupCollection GetResourceGroupCollection(params string[] resourceGroupNames)
        {
            throw new NotSupportedException("GetResourceGroupCollection");
        }

        /// <summary>
        ///     获取资源组集合。
        /// </summary>
        /// <param name="resourceGroupNames">要获取的资源组名称的集合。</param>
        /// <returns>要获取的资源组集合。</returns>
        public IResourceGroupCollection GetResourceGroupCollection(List<string> resourceGroupNames)
        {
            throw new NotSupportedException("GetResourceGroupCollection");
        }

        private bool HasFile(string assetName)
        {
            if (string.IsNullOrEmpty(assetName)) return false;

            if (HasCachedAsset(assetName)) return true;

            var assetFullName = Application.dataPath.Substring(0, Application.dataPath.Length - AssetsStringLength) +
                                assetName;
            if (string.IsNullOrEmpty(assetFullName)) return false;

            var splitedAssetFullName = assetFullName.Split('/');
            var currentPath = Path.GetPathRoot(assetFullName);
            for (var i = 1; i < splitedAssetFullName.Length - 1; i++)
            {
                var directoryNames = Directory.GetDirectories(currentPath, splitedAssetFullName[i]);
                if (directoryNames.Length != 1) return false;

                currentPath = directoryNames[0];
            }

            var fileNames = Directory.GetFiles(currentPath, splitedAssetFullName[splitedAssetFullName.Length - 1]);
            if (fileNames.Length != 1) return false;

            var fileFullName = Utility.Path.GetRegularPath(fileNames[0]);
            if (fileFullName == null) return false;

            if (assetFullName != fileFullName)
            {
                if (assetFullName.ToLowerInvariant() == fileFullName.ToLowerInvariant())
                    Log.Warning(
                        "The real path of the specific asset '{0}' is '{1}'. Check the case of letters in the path.",
                        assetName, "Assets" + fileFullName.Substring(Application.dataPath.Length));

                return false;
            }

            return true;
        }

        private bool HasCachedAsset(string assetName)
        {
            if (!m_EnableCachedAssets) return false;

            if (string.IsNullOrEmpty(assetName)) return false;

            return m_CachedAssets.ContainsKey(assetName);
        }

        private Object GetCachedAsset(string assetName)
        {
            if (!m_EnableCachedAssets) return null;

            if (string.IsNullOrEmpty(assetName)) return null;

            Object asset = null;
            if (m_CachedAssets.TryGetValue(assetName, out asset)) return asset;

            return null;
        }

        [StructLayout(LayoutKind.Auto)]
        private struct LoadAssetInfo
        {
            public LoadAssetInfo(string assetName, Type assetType, int priority, DateTime startTime, float delaySeconds,
                LoadAssetCallbacks loadAssetCallbacks, object userData)
            {
                AssetName = assetName;
                AssetType = assetType;
                Priority = priority;
                StartTime = startTime;
                DelaySeconds = delaySeconds;
                LoadAssetCallbacks = loadAssetCallbacks;
                UserData = userData;
            }

            public string AssetName { get; }

            public Type AssetType { get; }

            public int Priority { get; }

            public DateTime StartTime { get; }

            public float DelaySeconds { get; }

            public LoadAssetCallbacks LoadAssetCallbacks { get; }

            public object UserData { get; }
        }

        [StructLayout(LayoutKind.Auto)]
        private struct LoadSceneInfo
        {
            public LoadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName, int priority, DateTime startTime,
                LoadSceneCallbacks loadSceneCallbacks, object userData)
            {
                AsyncOperation = asyncOperation;
                SceneAssetName = sceneAssetName;
                Priority = priority;
                StartTime = startTime;
                LoadSceneCallbacks = loadSceneCallbacks;
                UserData = userData;
            }

            public AsyncOperation AsyncOperation { get; }

            public string SceneAssetName { get; }

            public int Priority { get; }

            public DateTime StartTime { get; }

            public LoadSceneCallbacks LoadSceneCallbacks { get; }

            public object UserData { get; }
        }

        [StructLayout(LayoutKind.Auto)]
        private struct UnloadSceneInfo
        {
            public UnloadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName,
                UnloadSceneCallbacks unloadSceneCallbacks, object userData)
            {
                AsyncOperation = asyncOperation;
                SceneAssetName = sceneAssetName;
                UnloadSceneCallbacks = unloadSceneCallbacks;
                UserData = userData;
            }

            public AsyncOperation AsyncOperation { get; }

            public string SceneAssetName { get; }

            public UnloadSceneCallbacks UnloadSceneCallbacks { get; }

            public object UserData { get; }
        }

#pragma warning disable 0067, 0414

        /// <summary>
        ///     资源应用成功事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceApplySuccessEventArgs> ResourceApplySuccess;

        /// <summary>
        ///     资源应用失败事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceApplyFailureEventArgs> ResourceApplyFailure;

        /// <summary>
        ///     资源更新开始事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateStartEventArgs> ResourceUpdateStart;

        /// <summary>
        ///     资源更新改变事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateChangedEventArgs> ResourceUpdateChanged;

        /// <summary>
        ///     资源更新成功事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateSuccessEventArgs> ResourceUpdateSuccess;

        /// <summary>
        ///     资源更新失败事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateFailureEventArgs> ResourceUpdateFailure;

        /// <summary>
        ///     资源更新全部完成事件。
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateAllCompleteEventArgs> ResourceUpdateAllComplete;

#pragma warning restore 0067, 0414
    }
}