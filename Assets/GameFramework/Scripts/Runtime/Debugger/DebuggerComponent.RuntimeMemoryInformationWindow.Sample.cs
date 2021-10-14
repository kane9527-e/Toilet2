//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public sealed partial class DebuggerComponent : GameFrameworkComponent
    {
        private sealed partial class RuntimeMemoryInformationWindow<T> : ScrollableDebuggerWindowBase where T : Object
        {
            private sealed class Sample
            {
                public Sample(string name, string type, long size)
                {
                    Name = name;
                    Type = type;
                    Size = size;
                    Highlight = false;
                }

                public string Name { get; }

                public string Type { get; }

                public long Size { get; }

                public bool Highlight { get; set; }
            }
        }
    }
}