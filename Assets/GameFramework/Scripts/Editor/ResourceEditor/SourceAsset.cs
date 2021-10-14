//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFramework.Editor.ResourceTools
{
    public sealed class SourceAsset
    {
        private Texture m_CachedIcon;

        public SourceAsset(string guid, string path, string name, SourceFolder folder)
        {
            if (folder == null) throw new GameFrameworkException("Source asset folder is invalid.");

            Guid = guid;
            Path = path;
            Name = name;
            Folder = folder;
            m_CachedIcon = null;
        }

        public string Guid { get; }

        public string Path { get; }

        public string Name { get; }

        public SourceFolder Folder { get; }

        public string FromRootPath =>
            Folder.Folder == null ? Name : Utility.Text.Format("{0}/{1}", Folder.FromRootPath, Name);

        public int Depth => Folder != null ? Folder.Depth + 1 : 0;

        public Texture Icon
        {
            get
            {
                if (m_CachedIcon == null) m_CachedIcon = AssetDatabase.GetCachedIcon(Path);

                return m_CachedIcon;
            }
        }
    }
}