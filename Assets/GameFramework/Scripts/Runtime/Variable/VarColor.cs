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
    ///     UnityEngine.Color 变量类。
    /// </summary>
    public sealed class VarColor : Variable<Color>
    {
        /// <summary>
        ///     从 UnityEngine.Color 到 UnityEngine.Color 变量类的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator VarColor(Color value)
        {
            var varValue = ReferencePool.Acquire<VarColor>();
            varValue.Value = value;
            return varValue;
        }

        /// <summary>
        ///     从 UnityEngine.Color 变量类到 UnityEngine.Color 的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator Color(VarColor value)
        {
            return value.Value;
        }
    }
}