//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    ///     UnityEngine.Rect 变量类。
    /// </summary>
    public sealed class VarRect : Variable<Rect>
    {
        /// <summary>
        ///     从 UnityEngine.Rect 到 UnityEngine.Rect 变量类的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator VarRect(Rect value)
        {
            var varValue = ReferencePool.Acquire<VarRect>();
            varValue.Value = value;
            return varValue;
        }

        /// <summary>
        ///     从 UnityEngine.Rect 变量类到 UnityEngine.Rect 的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator Rect(VarRect value)
        {
            return value.Value;
        }
    }
}