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
    ///     UnityEngine.Quaternion 变量类。
    /// </summary>
    public sealed class VarQuaternion : Variable<Quaternion>
    {
        /// <summary>
        ///     从 UnityEngine.Quaternion 到 UnityEngine.Quaternion 变量类的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator VarQuaternion(Quaternion value)
        {
            var varValue = ReferencePool.Acquire<VarQuaternion>();
            varValue.Value = value;
            return varValue;
        }

        /// <summary>
        ///     从 UnityEngine.Quaternion 变量类到 UnityEngine.Quaternion 的隐式转换。
        /// </summary>
        /// <param name="value">值。</param>
        public static implicit operator Quaternion(VarQuaternion value)
        {
            return value.Value;
        }
    }
}