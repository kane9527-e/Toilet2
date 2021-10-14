//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Runtime.InteropServices;

namespace UnityGameFramework.Editor.ResourceTools
{
    public sealed partial class ResourceAnalyzerController
    {
        [StructLayout(LayoutKind.Auto)]
        private struct Stamp
        {
            public Stamp(string hostAssetName, string dependencyAssetName)
            {
                HostAssetName = hostAssetName;
                DependencyAssetName = dependencyAssetName;
            }

            public string HostAssetName { get; }

            public string DependencyAssetName { get; }
        }
    }
}