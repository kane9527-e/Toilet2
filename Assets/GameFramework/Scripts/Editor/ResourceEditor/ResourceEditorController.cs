//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor.ResourceTools
{
    public sealed class ResourceEditorController
    {
        private const string DefaultSourceAssetRootPath = "Assets";

        private readonly string m_ConfigurationPath;
        private readonly ResourceCollection m_ResourceCollection;
        private readonly Dictionary<string, SourceAsset> m_SourceAssets;
        private readonly List<string> m_SourceAssetSearchPaths;
        private readonly List<string> m_SourceAssetSearchRelativePaths;
        private AssetSorterType m_AssetSorter;
        private string m_SourceAssetExceptLabelFilter;
        private string m_SourceAssetExceptTypeFilter;
        private string m_SourceAssetRootPath;
        private string m_SourceAssetUnionLabelFilter;
        private string m_SourceAssetUnionTypeFilter;

        public ResourceEditorController()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<ResourceEditorConfigPathAttribute>() ??
                                  Utility.Path.GetRegularPath(Path.Combine(Application.dataPath,
                                      "GameFramework/Configs/ResourceEditor.xml"));
            m_ResourceCollection = new ResourceCollection();
            m_ResourceCollection.OnLoadingResource += delegate(int index, int count)
            {
                if (OnLoadingResource != null) OnLoadingResource(index, count);
            };

            m_ResourceCollection.OnLoadingAsset += delegate(int index, int count)
            {
                if (OnLoadingAsset != null) OnLoadingAsset(index, count);
            };

            m_ResourceCollection.OnLoadCompleted += delegate
            {
                if (OnLoadCompleted != null) OnLoadCompleted();
            };

            m_SourceAssetSearchPaths = new List<string>();
            m_SourceAssetSearchRelativePaths = new List<string>();
            m_SourceAssets = new Dictionary<string, SourceAsset>(StringComparer.Ordinal);
            SourceAssetRoot = null;
            m_SourceAssetRootPath = null;
            m_SourceAssetUnionTypeFilter = null;
            m_SourceAssetUnionLabelFilter = null;
            m_SourceAssetExceptTypeFilter = null;
            m_SourceAssetExceptLabelFilter = null;
            m_AssetSorter = AssetSorterType.Path;

            SourceAssetRootPath = DefaultSourceAssetRootPath;
        }

        public int ResourceCount => m_ResourceCollection.ResourceCount;

        public int AssetCount => m_ResourceCollection.AssetCount;

        public SourceFolder SourceAssetRoot { get; private set; }

        public string SourceAssetRootPath
        {
            get => m_SourceAssetRootPath;
            set
            {
                if (m_SourceAssetRootPath == value) return;

                m_SourceAssetRootPath = value.Replace('\\', '/');
                SourceAssetRoot = new SourceFolder(m_SourceAssetRootPath, null);
                RefreshSourceAssetSearchPaths();
            }
        }

        public string SourceAssetUnionTypeFilter
        {
            get => m_SourceAssetUnionTypeFilter;
            set
            {
                if (m_SourceAssetUnionTypeFilter == value) return;

                m_SourceAssetUnionTypeFilter = value;
            }
        }

        public string SourceAssetUnionLabelFilter
        {
            get => m_SourceAssetUnionLabelFilter;
            set
            {
                if (m_SourceAssetUnionLabelFilter == value) return;

                m_SourceAssetUnionLabelFilter = value;
            }
        }

        public string SourceAssetExceptTypeFilter
        {
            get => m_SourceAssetExceptTypeFilter;
            set
            {
                if (m_SourceAssetExceptTypeFilter == value) return;

                m_SourceAssetExceptTypeFilter = value;
            }
        }

        public string SourceAssetExceptLabelFilter
        {
            get => m_SourceAssetExceptLabelFilter;
            set
            {
                if (m_SourceAssetExceptLabelFilter == value) return;

                m_SourceAssetExceptLabelFilter = value;
            }
        }

        public AssetSorterType AssetSorter
        {
            get => m_AssetSorter;
            set
            {
                if (m_AssetSorter == value) return;

                m_AssetSorter = value;
            }
        }

        public event GameFrameworkAction<int, int> OnLoadingResource;

        public event GameFrameworkAction<int, int> OnLoadingAsset;

        public event GameFrameworkAction OnLoadCompleted;

        public event GameFrameworkAction<SourceAsset[]> OnAssetAssigned;

        public event GameFrameworkAction<SourceAsset[]> OnAssetUnassigned;

        public bool Load()
        {
            if (!File.Exists(m_ConfigurationPath)) return false;

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                var xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                var xmlEditor = xmlRoot.SelectSingleNode("ResourceEditor");
                var xmlSettings = xmlEditor.SelectSingleNode("Settings");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;

                xmlNodeList = xmlSettings.ChildNodes;
                for (var i = 0; i < xmlNodeList.Count; i++)
                {
                    xmlNode = xmlNodeList.Item(i);
                    switch (xmlNode.Name)
                    {
                        case "SourceAssetRootPath":
                            SourceAssetRootPath = xmlNode.InnerText;
                            break;

                        case "SourceAssetSearchPaths":
                            m_SourceAssetSearchRelativePaths.Clear();
                            var xmlNodeListInner = xmlNode.ChildNodes;
                            XmlNode xmlNodeInner = null;
                            for (var j = 0; j < xmlNodeListInner.Count; j++)
                            {
                                xmlNodeInner = xmlNodeListInner.Item(j);
                                if (xmlNodeInner.Name != "SourceAssetSearchPath") continue;

                                m_SourceAssetSearchRelativePaths.Add(xmlNodeInner.Attributes
                                    .GetNamedItem("RelativePath").Value);
                            }

                            break;

                        case "SourceAssetUnionTypeFilter":
                            SourceAssetUnionTypeFilter = xmlNode.InnerText;
                            break;

                        case "SourceAssetUnionLabelFilter":
                            SourceAssetUnionLabelFilter = xmlNode.InnerText;
                            break;

                        case "SourceAssetExceptTypeFilter":
                            SourceAssetExceptTypeFilter = xmlNode.InnerText;
                            break;

                        case "SourceAssetExceptLabelFilter":
                            SourceAssetExceptLabelFilter = xmlNode.InnerText;
                            break;

                        case "AssetSorter":
                            AssetSorter = (AssetSorterType)Enum.Parse(typeof(AssetSorterType), xmlNode.InnerText);
                            break;
                    }
                }

                RefreshSourceAssetSearchPaths();
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                return false;
            }

            ScanSourceAssets();

            m_ResourceCollection.Load();

            return true;
        }

        public bool Save()
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                var xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                var xmlEditor = xmlDocument.CreateElement("ResourceEditor");
                xmlRoot.AppendChild(xmlEditor);

                var xmlSettings = xmlDocument.CreateElement("Settings");
                xmlEditor.AppendChild(xmlSettings);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                xmlElement = xmlDocument.CreateElement("SourceAssetRootPath");
                xmlElement.InnerText = SourceAssetRootPath;
                xmlSettings.AppendChild(xmlElement);

                xmlElement = xmlDocument.CreateElement("SourceAssetSearchPaths");
                xmlSettings.AppendChild(xmlElement);

                foreach (var sourceAssetSearchRelativePath in m_SourceAssetSearchRelativePaths)
                {
                    var xmlElementInner = xmlDocument.CreateElement("SourceAssetSearchPath");
                    xmlAttribute = xmlDocument.CreateAttribute("RelativePath");
                    xmlAttribute.Value = sourceAssetSearchRelativePath;
                    xmlElementInner.Attributes.SetNamedItem(xmlAttribute);
                    xmlElement.AppendChild(xmlElementInner);
                }

                xmlElement = xmlDocument.CreateElement("SourceAssetUnionTypeFilter");
                xmlElement.InnerText = SourceAssetUnionTypeFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetUnionLabelFilter");
                xmlElement.InnerText = SourceAssetUnionLabelFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetExceptTypeFilter");
                xmlElement.InnerText = SourceAssetExceptTypeFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetExceptLabelFilter");
                xmlElement.InnerText = SourceAssetExceptLabelFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AssetSorter");
                xmlElement.InnerText = AssetSorter.ToString();
                xmlSettings.AppendChild(xmlElement);

                var configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                    Directory.CreateDirectory(configurationDirectoryName);

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath)) File.Delete(m_ConfigurationPath);

                return false;
            }

            return m_ResourceCollection.Save();
        }

        public Resource[] GetResources()
        {
            return m_ResourceCollection.GetResources();
        }

        public Resource GetResource(string name, string variant)
        {
            return m_ResourceCollection.GetResource(name, variant);
        }

        public bool HasResource(string name, string variant)
        {
            return m_ResourceCollection.HasResource(name, variant);
        }

        public bool AddResource(string name, string variant, string fileSystem, LoadType loadType, bool packed)
        {
            return m_ResourceCollection.AddResource(name, variant, fileSystem, loadType, packed);
        }

        public bool RenameResource(string oldName, string oldVariant, string newName, string newVariant)
        {
            return m_ResourceCollection.RenameResource(oldName, oldVariant, newName, newVariant);
        }

        public bool RemoveResource(string name, string variant)
        {
            var assetsToRemove = m_ResourceCollection.GetAssets(name, variant);
            if (m_ResourceCollection.RemoveResource(name, variant))
            {
                var unassignedSourceAssets = new List<SourceAsset>();
                foreach (var asset in assetsToRemove)
                {
                    var sourceAsset = GetSourceAsset(asset.Guid);
                    if (sourceAsset != null) unassignedSourceAssets.Add(sourceAsset);
                }

                if (OnAssetUnassigned != null) OnAssetUnassigned(unassignedSourceAssets.ToArray());

                return true;
            }

            return false;
        }

        public bool SetResourceLoadType(string name, string variant, LoadType loadType)
        {
            return m_ResourceCollection.SetResourceLoadType(name, variant, loadType);
        }

        public bool SetResourcePacked(string name, string variant, bool packed)
        {
            return m_ResourceCollection.SetResourcePacked(name, variant, packed);
        }

        public int RemoveUnusedResources()
        {
            var resources = new List<Resource>(m_ResourceCollection.GetResources());
            var removeResources = resources.FindAll(resource => GetAssets(resource.Name, resource.Variant).Length <= 0);
            foreach (var removeResource in removeResources)
                m_ResourceCollection.RemoveResource(removeResource.Name, removeResource.Variant);

            return removeResources.Count;
        }

        public Asset[] GetAssets(string name, string variant)
        {
            var assets = new List<Asset>(m_ResourceCollection.GetAssets(name, variant));
            switch (AssetSorter)
            {
                case AssetSorterType.Path:
                    assets.Sort(AssetPathComparer);
                    break;

                case AssetSorterType.Name:
                    assets.Sort(AssetNameComparer);
                    break;

                case AssetSorterType.Guid:
                    assets.Sort(AssetGuidComparer);
                    break;
            }

            return assets.ToArray();
        }

        public Asset GetAsset(string guid)
        {
            return m_ResourceCollection.GetAsset(guid);
        }

        public bool AssignAsset(string guid, string name, string variant)
        {
            if (m_ResourceCollection.AssignAsset(guid, name, variant))
            {
                if (OnAssetAssigned != null) OnAssetAssigned(new[] { GetSourceAsset(guid) });

                return true;
            }

            return false;
        }

        public bool UnassignAsset(string guid)
        {
            if (m_ResourceCollection.UnassignAsset(guid))
            {
                var sourceAsset = GetSourceAsset(guid);
                if (sourceAsset != null)
                    if (OnAssetUnassigned != null)
                        OnAssetUnassigned(new[] { sourceAsset });

                return true;
            }

            return false;
        }

        public int RemoveUnknownAssets()
        {
            var assets = new List<Asset>(m_ResourceCollection.GetAssets());
            var removeAssets = assets.FindAll(asset => GetSourceAsset(asset.Guid) == null);
            foreach (var asset in removeAssets) m_ResourceCollection.UnassignAsset(asset.Guid);

            return removeAssets.Count;
        }

        public SourceAsset[] GetSourceAssets()
        {
            var count = 0;
            var sourceAssets = new SourceAsset[m_SourceAssets.Count];
            foreach (var sourceAsset in m_SourceAssets) sourceAssets[count++] = sourceAsset.Value;

            return sourceAssets;
        }

        public SourceAsset GetSourceAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return null;

            SourceAsset sourceAsset = null;
            if (m_SourceAssets.TryGetValue(guid, out sourceAsset)) return sourceAsset;

            return null;
        }

        public void ScanSourceAssets()
        {
            m_SourceAssets.Clear();
            SourceAssetRoot.Clear();

            var sourceAssetSearchPaths = m_SourceAssetSearchPaths.ToArray();
            var tempGuids = new HashSet<string>();
            tempGuids.UnionWith(AssetDatabase.FindAssets(SourceAssetUnionTypeFilter, sourceAssetSearchPaths));
            tempGuids.UnionWith(AssetDatabase.FindAssets(SourceAssetUnionLabelFilter, sourceAssetSearchPaths));
            tempGuids.ExceptWith(AssetDatabase.FindAssets(SourceAssetExceptTypeFilter, sourceAssetSearchPaths));
            tempGuids.ExceptWith(AssetDatabase.FindAssets(SourceAssetExceptLabelFilter, sourceAssetSearchPaths));

            var guids = new List<string>(tempGuids).ToArray();
            foreach (var guid in guids)
            {
                var fullPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(fullPath))
                    // Skip folder.
                    continue;

                var assetPath = fullPath.Substring(SourceAssetRootPath.Length + 1);
                var splitedPath = assetPath.Split('/');
                var folder = SourceAssetRoot;
                for (var i = 0; i < splitedPath.Length - 1; i++)
                {
                    var subFolder = folder.GetFolder(splitedPath[i]);
                    folder = subFolder == null ? folder.AddFolder(splitedPath[i]) : subFolder;
                }

                var asset = folder.AddAsset(guid, fullPath, splitedPath[splitedPath.Length - 1]);
                m_SourceAssets.Add(asset.Guid, asset);
            }
        }

        private void RefreshSourceAssetSearchPaths()
        {
            m_SourceAssetSearchPaths.Clear();

            if (string.IsNullOrEmpty(m_SourceAssetRootPath)) SourceAssetRootPath = DefaultSourceAssetRootPath;

            if (m_SourceAssetSearchRelativePaths.Count > 0)
                foreach (var sourceAssetSearchRelativePath in m_SourceAssetSearchRelativePaths)
                    m_SourceAssetSearchPaths.Add(
                        Utility.Path.GetRegularPath(Path.Combine(m_SourceAssetRootPath,
                            sourceAssetSearchRelativePath)));
            else
                m_SourceAssetSearchPaths.Add(m_SourceAssetRootPath);
        }

        private int AssetPathComparer(Asset a, Asset b)
        {
            var sourceAssetA = GetSourceAsset(a.Guid);
            var sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null) return sourceAssetA.Path.CompareTo(sourceAssetB.Path);

            if (sourceAssetA == null && sourceAssetB == null) return a.Guid.CompareTo(b.Guid);

            if (sourceAssetA == null) return -1;

            if (sourceAssetB == null) return 1;

            return 0;
        }

        private int AssetNameComparer(Asset a, Asset b)
        {
            var sourceAssetA = GetSourceAsset(a.Guid);
            var sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null) return sourceAssetA.Name.CompareTo(sourceAssetB.Name);

            if (sourceAssetA == null && sourceAssetB == null) return a.Guid.CompareTo(b.Guid);

            if (sourceAssetA == null) return -1;

            if (sourceAssetB == null) return 1;

            return 0;
        }

        private int AssetGuidComparer(Asset a, Asset b)
        {
            var sourceAssetA = GetSourceAsset(a.Guid);
            var sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null || sourceAssetA == null && sourceAssetB == null)
                return a.Guid.CompareTo(b.Guid);

            if (sourceAssetA == null) return -1;

            if (sourceAssetB == null) return 1;

            return 0;
        }
    }
}