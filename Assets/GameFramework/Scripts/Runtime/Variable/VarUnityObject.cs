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
    ///     UnityEngine.Object 变量类。
    /// </summary>
    public sealed class VarUnityObject : Variable<Object>
    {
        /// <summary>
        ///     从 UnityEngine.Object 到 UnityEngine.Object 变量类的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator VarUnityObject(Object value)
        {
            var varValue = ReferencePool.Acquire<VarUnityObject>();
            varValue.Value = value;
            return varValue;
        }

        /// <summary>
        ///     从 UnityEngine.Object 变量类到 UnityEngine.Object 的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator Object(VarUnityObject value)
        {
            return value.Value;
        }
    }
}