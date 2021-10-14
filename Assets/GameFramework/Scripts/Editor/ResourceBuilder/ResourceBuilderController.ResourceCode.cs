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
        private sealed class ResourceCode
        {
            public ResourceCode(Platform platform, int length, int hashCode, int compressedLength,
                int compressedHashCode)
            {
                Platform = platform;
                Length = length;
                HashCode = hashCode;
                CompressedLength = compressedLength;
                CompressedHashCode = compressedHashCode;
            }

            public Platform Platform { get; }

            public int Length { get; }

            public int HashCode { get; }

            public int CompressedLength { get; }

            public int CompressedHashCode { get; }
        }
    }
}