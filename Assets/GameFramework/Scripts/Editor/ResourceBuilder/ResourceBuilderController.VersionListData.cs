//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace UnityGameFramework.Editor.ResourceTools
{
    public sealed partial class ResourceBuilderController
    {
        private sealed class VersionListData
        {
            public VersionListData(string path, int length, int hashCode, int compressedLength, int compressedHashCode)
            {
                Path = path;
                Length = length;
                HashCode = hashCode;
                CompressedLength = compressedLength;
                CompressedHashCode = compressedHashCode;
            }

            public string Path { get; }

            public int Length { get; }

            public int HashCode { get; }

            public int CompressedLength { get; }

            public int CompressedHashCode { get; }
        }
    }
}